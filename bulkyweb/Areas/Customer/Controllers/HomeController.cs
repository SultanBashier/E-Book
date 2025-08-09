
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using bulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Security.Claims;

namespace bulkyBookweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOFWork _unitOFWork;

        public HomeController(ILogger<HomeController> logger, IUnitOFWork unitOFWork)
        {
            _logger = logger;
            _unitOFWork = unitOFWork;
        }

        public IActionResult Index()
        {
            
            IEnumerable<Product> productList = _unitOFWork.Product.GetAll(includeProperties: "Category,productImages");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart Cart = new()
            {
                Product = _unitOFWork.Product.Get(p => p.Id == productId, includeProperties: "Category,productImages"),
                Count = 1,
                ProductId = productId
            };
            return View(Cart);
        }
        //This Action For Add To Card
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            // Get the current user's identity from the claims
            var cliamIdentity = (ClaimsIdentity)User.Identity;
            // Retrieve the user's unique identifier (UserId) from the claims

            var userId = cliamIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            // Assign the current user's UserId to the ShoppingCart object

            shoppingCart.ApplicationUserId = userId;
            // Search for an existing shopping cart entry for this user and product in the database

            ShoppingCart cartFromDb = _unitOFWork.ShoppingCart.Get(
                u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
            if (cartFromDb != null)
            {//if the cart is already in the database, update the count
                cartFromDb.Count += shoppingCart.Count;
                _unitOFWork.ShoppingCart.Update(cartFromDb);
                _unitOFWork.Save();
                TempData["success"] = "Cart Update Successfully";

            }
            else
            {
                //if the cart is not in the database, add it
                _unitOFWork.ShoppingCart.Add(shoppingCart); 
                _unitOFWork.Save();

                HttpContext.Session.SetInt32(SD.SessionCart,                              
                _unitOFWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

                TempData["success"] = "Product Added To Card Successfully";

            }



            return RedirectToAction(nameof(Index));

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
