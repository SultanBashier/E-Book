
using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using bulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection.Metadata.Ecma335;

namespace bulkyBookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]// Restrict access to Admins only
    public class CategoryController : Controller
    {
        private readonly IUnitOFWork _unitofwork;

        public CategoryController(IUnitOFWork unitOFWork)
        {
            this._unitofwork = unitOFWork;
        }

        // Show all categories
        public IActionResult Index()
        {
            List<Category> Categories = _unitofwork.Category.GetAll().ToList();
           
            return View(Categories);
        }

        // Show create View
        public IActionResult Create()
        {
           
            return View();
        }

        // Handle form POST for creating a category
        [HttpPost]
        public IActionResult Create(Category model)
        {
            // Custom validation: name must not match display order
            if (model.Name == model.Displayorder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _unitofwork.Category.Add(model);
                _unitofwork.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        // Show edit form
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            // Get category by id
            Category category = _unitofwork.Category.Get(x => x.Id == id);
            // Category category = dbContext.categories.FirstOrDefault(c => c.Id == id);


            if (category == null)
                return NotFound();

            return View(category);
        }

        // Handle form POST for editing a category
        [HttpPost]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.Category.Update(model);
                _unitofwork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        // Show delete confirmation
        public ActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
                return NotFound();
            Category category = _unitofwork.Category.Get(i => i.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // Handle delete post action
        [HttpPost]
        public IActionResult Delete(Category category)
        {
            _unitofwork.Category.Remove(category);
            _unitofwork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");

        }

    }
}
