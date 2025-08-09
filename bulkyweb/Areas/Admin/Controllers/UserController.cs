
using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository;
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using bulkyBook.Models.ViewModel;
using bulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // Restrict access to Admin users only
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOFWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, IUnitOFWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Load the default User Management view
        public IActionResult Index()
        {
            return View();
        }

        // Display the role management view for a selected user
        public IActionResult RoleManagment(string userId)
        {
            // Create and populate the view model
            RoleManagmentVM RMVM = new RoleManagmentVM()
            {
                apllicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            // Get the user's current role
            RMVM.apllicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId))
                    .GetAwaiter().GetResult().FirstOrDefault();
            return View(RMVM);
        }

        // Handle POST request for updating user roles and company
        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM RMVM)
        {
            // Get the user's current role from the database
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == RMVM.apllicationUser.Id))
                    .GetAwaiter().GetResult().FirstOrDefault();

            ApllicationUser appUserFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == RMVM.apllicationUser.Id);

            // Check if the role has changed
            if (!(RMVM.apllicationUser.Role == oldRole))
            {
                // Assign CompanyId if new role is "Company"
                if (RMVM.apllicationUser.Role == SD.Role_Company)
                {
                    appUserFromDb.CompanyId = RMVM.apllicationUser.CompanyId;
                }
                // Remove CompanyId if switching away from "Company" role
                if (oldRole == SD.Role_Company)
                {
                    appUserFromDb.CompanyId = null;
                }

                // Save changes and update roles
                _unitOfWork.ApplicationUser.Update(appUserFromDb);
                _unitOfWork.Save();

                // Remove old role and assign new role
                _userManager.RemoveFromRoleAsync(appUserFromDb, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(appUserFromDb, RMVM.apllicationUser.Role).GetAwaiter().GetResult();

            }
            else
            {
                // If the role didn’t change, only update the company if needed
                if (oldRole == SD.Role_Company && appUserFromDb.CompanyId != RMVM.apllicationUser.CompanyId)
                {
                    appUserFromDb.CompanyId = RMVM.apllicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(appUserFromDb);
                    _unitOfWork.Save();
                }
            }
            TempData["success"] = "The Permission Updated  Successfully ";
            return RedirectToAction("Index");
        }


        #region API CALLS

        // Return all users (with their roles and companies) as JSON data
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApllicationUser> AppUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in AppUserList)
            {
                // Set the user role
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                // If company is null, assign an empty company object to prevent errors
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = AppUserList });
        }

        // Lock or unlock a user based on current status
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {

            var AppUserFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (AppUserFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            // If the user is currently locked, unlock them
            if (AppUserFromDb.LockoutEnd != null && AppUserFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                AppUserFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {// user is currently Unlocked and we will be   lock them
                AppUserFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            // Save changes
            _unitOfWork.ApplicationUser.Update(AppUserFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}