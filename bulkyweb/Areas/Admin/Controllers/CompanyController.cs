
using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using bulkyBook.Models.ViewModel;
using bulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bulkyBookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]// Restrict access to Admin role only

    public class CompanyController : Controller
    {
        private readonly IUnitOFWork _unitofwork;

        // This Constructor For Inject UnitOfWork dependency
        public CompanyController(IUnitOFWork unitOFWork)
        {
            _unitofwork = unitOFWork;           
        }

        // Display list of all companies
        public IActionResult Index()
        {
            List<Company> Companies = _unitofwork.Company.GetAll().ToList();

            return View(Companies);
        }


        public IActionResult Upsert(int? id) //This Action For Create and Update Data Based on id
        {
            

            if (id == null || id == 0)
            { 
                // Create mode
                return View(new Company());
            }
            else
            {
                //update Mode
               // load company data by id
                Company companyobj = _unitofwork.Company.Get(u => u.Id == id);
                return View(companyobj);

            }

        }

        //Handle create or update form submission
       [HttpPost]
        public IActionResult Upsert(Company companyobj)
        {
            if (ModelState.IsValid)
            {
                //to upload the image in the wwwroot\image\product
               

                if (companyobj.Id == 0)
                {// Create new company
                    TempData["success"] = "Company Create Successfully";

                    _unitofwork.Company.Add(companyobj);
                }
               
                else
                { //This for Update Data
                    TempData["success"] = "Company Updated Successfully";

                    _unitofwork.Company.Update(companyobj);
                }

                _unitofwork.Save();// Save changes to database
                return RedirectToAction("Index");
            }
            else
            {
                // Validation failed: return the form with errors
                return View(companyobj);
            }

        }




        #region API Calls

        // API: Return all companies in JSON format 
        [HttpGet]
        public IActionResult Getall()
        {
            List<Company> companies = _unitofwork.Company.GetAll().ToList();
            return Json(new { data = companies } );
        }

        // API: Delete a company by ID
        [HttpDelete]
         public IActionResult Delete(int? id)
        {
            var CompanyToDelete = _unitofwork.Company.Get(u => u.Id == id);
            if(CompanyToDelete == null)
            {
                // Company not found
                return Json(new { success = false, message = "Error While deleteing " });
            }
            
            _unitofwork.Company.Remove(CompanyToDelete);
            _unitofwork.Save();
            return Json(new { success = true,message="Deleted successfully" } );
        }

        

        #endregion
    }
}

