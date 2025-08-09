using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bulkyBookweb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOFWork _unitOFWork;

        public ShoppingCartViewComponent(IUnitOFWork unitOFWork)
        {
            _unitOFWork = unitOFWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {

                    HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOFWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);

            }


        }
    }
}
