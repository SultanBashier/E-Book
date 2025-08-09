using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using bulkyBook.Models.ViewModel;
using bulkyBook.Utility;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace bulkyBookweb.Areas.Customer.Controllers
{
    // Controller in the Customer area, requires the user to be authenticated
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOFWork _unitOFWork;

        // Property to bind the view model automatically in POST actions
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        // Constructor injection of UnitOfWork to access the database
        public CartController(IUnitOFWork unitOFWork)
        {
            _unitOFWork = unitOFWork;
        }

        // Displays the shopping cart for the current user
        public IActionResult Index()
        {
            var cliamIdentity = (ClaimsIdentity)User.Identity;
            var userId = cliamIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Initialize the view model
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOFWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == userId,
                    includeProperties: "Product"
                ),
                OrderHeader = new()
            };

            // Load all product images
            IEnumerable<ProductImage> ProductImage = _unitOFWork.ProductImage.GetAll().ToList();

            // Calculate the price based on quantity and total
            foreach (var model in ShoppingCartVM.ShoppingCartList)
            {
                model.Product.productImages.Where(p => p.ProductId == model.Product.Id); // This line seems to do nothing
                model.price = GetPriceBasedOnQuantity(model);
                ShoppingCartVM.OrderHeader.OrderTotal += model.price * model.Count;
            }

            return View(ShoppingCartVM);
        }

        #region GetPriceBasedOnQuantity
        // Determines the price based on the quantity in cart
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
                return shoppingCart.Product.Price;
            else if (shoppingCart.Count <= 100)
                return shoppingCart.Product.Price50;
            else
                return shoppingCart.Product.Price100;
        }
        #endregion

        // Increases quantity of a cart item
        public IActionResult Plus(int cartId)
        {
            var cartDb = _unitOFWork.ShoppingCart.Get(u => u.Id == cartId);
            cartDb.Count += 1;
            _unitOFWork.ShoppingCart.Update(cartDb);
            _unitOFWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // Decreases quantity or removes item if quantity is 1
        public IActionResult Minus(int cartId)
        {
            var cartDb = _unitOFWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            if (cartDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOFWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartDb.ApplicationUserId).Count() - 1);
                _unitOFWork.ShoppingCart.Remove(cartDb);
            }
            else
            {
                cartDb.Count -= 1;
                _unitOFWork.ShoppingCart.Update(cartDb);
            }
            _unitOFWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // Deletes an item from the cart
        public IActionResult Delete(int cartId)
        {
            var cartDb = _unitOFWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            if (cartDb == null)
                return RedirectToAction(nameof(Index));

            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOFWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartDb.ApplicationUserId).Count() - 1);

            _unitOFWork.ShoppingCart.Remove(cartDb);
            _unitOFWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // Shows the order summary before placing the order
        public IActionResult Summary()
        {
            var cliamIdentity = (ClaimsIdentity)User.Identity;
            var userId = cliamIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOFWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == userId,
                    includeProperties: "Product"
                ),
                OrderHeader = new()
            };

            // Manually populate order header from application user
            ShoppingCartVM.OrderHeader.ApllicationUser = _unitOFWork.ApplicationUser.Get(u => u.Id == userId);
            var user = ShoppingCartVM.OrderHeader.ApllicationUser;

            ShoppingCartVM.OrderHeader.Name = user.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAdrress = user.StreetAdrress;
            ShoppingCartVM.OrderHeader.City = user.City;
            ShoppingCartVM.OrderHeader.state = user.state;
            ShoppingCartVM.OrderHeader.Postalcode = user.Postalcode;

            // Calculate total
            foreach (var model in ShoppingCartVM.ShoppingCartList)
            {
                model.price = GetPriceBasedOnQuantity(model);
                ShoppingCartVM.OrderHeader.OrderTotal += model.price * model.Count;
            }

            return View(ShoppingCartVM);
        }

        // Submits the order (POST)
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var cliamIdentity = (ClaimsIdentity)User.Identity;
            var userId = cliamIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOFWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product"
            );

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            var user = _unitOFWork.ApplicationUser.Get(u => u.Id == userId);

            // Calculate total again
            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                item.price = GetPriceBasedOnQuantity(item);
                ShoppingCartVM.OrderHeader.OrderTotal += item.price * item.Count;
            }

            // Set order and payment status based on user type
            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                // Regular user, needs online payment
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                // Company account, payment deferred
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOFWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOFWork.Save();

            // Create order details from cart items
            foreach (var cartItem in ShoppingCartVM.ShoppingCartList)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = cartItem.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cartItem.price,
                    Count = cartItem.Count
                };
                _unitOFWork.OrderDetail.Add(orderDetail);
            }

            _unitOFWork.Save();

            // Stripe payment setup for individual customers
            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:7254/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation/{ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var lineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(lineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                // Save Stripe session and payment intent IDs
                _unitOFWork.OrderHeader.UpdateStripePaymentId(
                    ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId
                );
                _unitOFWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        // Called after payment is complete
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOFWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApllicationUser");

            // If the order was placed by a customer (not company), verify Stripe payment
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOFWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOFWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOFWork.Save();
                }
            }

            // Clear shopping cart after successful order
            List<ShoppingCart> ShoppingCart = _unitOFWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOFWork.ShoppingCart.RemoveRange(ShoppingCart);
            _unitOFWork.Save();

            return View(id);
        }
    }
}
