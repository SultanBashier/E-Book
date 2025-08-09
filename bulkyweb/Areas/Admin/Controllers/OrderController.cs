using bulkyBook.DataAccess.Repository;
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using bulkyBook.Models.ViewModel;
using bulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using Stripe;
using System.Collections.Generic;
using System.Security.Claims;

namespace bulkyBookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]//// Restrict access to Admin And Employee Role
    public class OrderController : Controller
    {
        private readonly IUnitOFWork _unitofwork;
        // Automatically bind OrderVM during requests
        [BindProperty]
        public OrderVM orderVM { get; set; }

        // Constructor: inject UnitOfWork
        public OrderController(IUnitOFWork unitOFWork)
        {
            this._unitofwork = unitOFWork;
        }

        // Display Orders Index page 
        public IActionResult Index()
        {
            return View();
        }

        // Show order details page for given orderId
        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new OrderVM()
            {
                orderHeader = _unitofwork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApllicationUser"),
                orderDetail = _unitofwork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "product")
            };
            return View(orderVM);
        }

        // Update editable details of the order 
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            //First Get The Order-Header From DataBase
            var OrderHeaderFromDB = _unitofwork.OrderHeader.Get(u => u.Id == orderVM.orderHeader.Id);
            // Update customer and shipping details
            //Second Do Manual Mapping (I Will Do Auto Mapper)
            OrderHeaderFromDB.Name = orderVM.orderHeader.Name;
            OrderHeaderFromDB.PhoneNumber = orderVM.orderHeader.PhoneNumber;
            OrderHeaderFromDB.StreetAdrress = orderVM.orderHeader.StreetAdrress;
            OrderHeaderFromDB.City = orderVM.orderHeader.City;
            OrderHeaderFromDB.state = orderVM.orderHeader.state;
            OrderHeaderFromDB.Postalcode = orderVM.orderHeader.Postalcode;

            //  Update carrier info if provided
            if (!string.IsNullOrEmpty(orderVM.orderHeader.Carrier))
            {
                OrderHeaderFromDB.Carrier = orderVM.orderHeader.Carrier;
            }

            //  Update tracking number
            if (!string.IsNullOrEmpty(orderVM.orderHeader.TrackingNumber))
            {
                OrderHeaderFromDB.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            }
            _unitofwork.OrderHeader.Update(OrderHeaderFromDB);
            _unitofwork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Index), new { orderId = OrderHeaderFromDB.Id });

        }

        // Set order status to "In Process"
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitofwork.OrderHeader.UpdateStatus(orderVM.orderHeader.Id, SD.StatusInProcess);
            _unitofwork.Save();
            TempData["Success"] = "Your Orders Are Start Processing";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });

        }

        // Ship the order and update tracking, carrier, status, and dates
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            //First Get The Order-Header From Databse
            var OrderFromDb = _unitofwork.OrderHeader.Get(u => u.Id == orderVM.orderHeader.Id);
            //Second Do Manual Mapping(I Will Do Auto Mapper)
            OrderFromDb.Carrier = orderVM.orderHeader.Carrier;
            OrderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            OrderFromDb.OrderStatus = SD.StatusShipped;
            OrderFromDb.ShippingDate = DateOnly.FromDateTime(DateTime.Now);

            // If payment is delayed, set due date to 30 days from now

            if (OrderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                OrderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitofwork.OrderHeader.Update(OrderFromDb);

            _unitofwork.Save();
            TempData["Success"] = "Your Orders Are  Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });

        }

        // Cancel order and process refund if payment was approved
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitofwork.OrderHeader.Get(u => u.Id == orderVM.orderHeader.Id);

            // If payment was successful, initiate a refund
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntenId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitofwork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.Statusrefunded);
            }
            else
            {
                // Otherwise, just cancel without refund
                _unitofwork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitofwork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });

        }


        [HttpGet]

        // API: Get all orders with optional status filter
        public IActionResult GetAll(string status)
        {

            List<OrderHeader> OrderHeader;

            // Admin or Employee: fetch all orders
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                OrderHeader = _unitofwork.OrderHeader.GetAll(includeProperties: "ApllicationUser").ToList();
            }
            else
            {
                // Otherwise: fetch orders for the logged-in user only
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var UserId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                OrderHeader = _unitofwork.OrderHeader.GetAll(u => u.ApplicationUserId == UserId, includeProperties: "ApllicationUser").ToList();

            }

            // Filter by status if provided
            switch (status)
            {
                case "pending":
                    OrderHeader = OrderHeader.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment).ToList();
                    break;
                case "inproccess":
                    OrderHeader = OrderHeader.Where(u => u.OrderStatus == SD.StatusInProcess).ToList();
                    break;
                case "completed":
                    OrderHeader = OrderHeader.Where(u => u.OrderStatus == SD.StatusShipped).ToList();
                    break;
                case "approved":
                    OrderHeader = OrderHeader.Where(u => u.OrderStatus == SD.StatusApproved).ToList();
                    break;

                default:
                    break;
            }

            return Json(new { data = OrderHeader });
        }




    }


}
