
using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository;
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using bulkyBook.Models.ViewModel;
using bulkyBook.Utility;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace bulkyBookweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]// Only accessible by admin users

    public class ProductController : Controller
    {
        private readonly IUnitOFWork _unitofwork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOFWork unitOFWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitOFWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            // Fetch all products along with their category details
            List<Product> Product = _unitofwork.Product.GetAll(includeProperties: "Category").ToList();

            return View(Product);
        }
        public IActionResult Upsert(int? Id) //This Action For Create and Update Data
        {
            // Initialize ViewModel with category dropdown and empty product
            ProductVM productVM = new()
            {
                CategoryListItems = _unitofwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (Id == null || Id == 0)
            { //Create--insert View

                return View(productVM);


            }
            else
            {
                //update View
                productVM.Product = _unitofwork.Product.Get(u => u.Id == Id, includeProperties: "productImages");
                return View(productVM);

            }

        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {//This Product Not Found And We Will Create new Product
                    _unitofwork.Product.Add(productVM.Product);
                    TempData["success"] = "Product Created successfully";
                }
                else
                {
                    //This Product Is Already Exist, And We Will Update Poduct
                    _unitofwork.Product.Update(productVM.Product);
                    TempData["success"] = "Product Updated Successfully";
                }

                _unitofwork.Save();

                // Handle file upload if there are any images submitted
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {

                        // Generate unique file name and create directory for product images
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);


                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        // Save image file to server
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        // Create new ProductImage entity and add to product
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.productImages == null)
                            productVM.Product.productImages = new List<ProductImage>();

                        productVM.Product.productImages.Add(productImage);

                    }
                    // Save new images to database
                    _unitofwork.Product.Update(productVM.Product);
                    _unitofwork.Save();

                }

                return RedirectToAction("Index");
            }
            else
            {
                // Reload dropdown data if validation fails
                productVM.CategoryListItems = _unitofwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        public IActionResult DeleteImage(int imageId)
        {
            // Get image from database By Id
            var ImageFromDb = _unitofwork.ProductImage.Get(i => i.Id == imageId);
            int ProductId = ImageFromDb.ProductId;

            if (ImageFromDb != null)
            {
                // Delete image file from server if exists
                if (!string.IsNullOrEmpty(ImageFromDb.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   ImageFromDb.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                }
                // Remove image record from database
                _unitofwork.ProductImage.Remove(ImageFromDb);
                _unitofwork.Save();
            }
            TempData["success"] = "The Image Deleted Successfully";
            return RedirectToAction(nameof(Upsert), new { Id = ProductId });
        }



        #region API Calls
        [HttpGet]
        public IActionResult Getall()
        {
            // Return all products with category info as JSON (for DataTables or AJAX)
            List<Product> objProduct = _unitofwork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProduct });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var productToBeDeleted = _unitofwork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // Delete associated image files and folder
            string ProductPath = @"images\products\product-" + id;
            string finalpath = Path.Combine(_webHostEnvironment.WebRootPath, ProductPath);
            if (Directory.Exists(finalpath))
            {
                var filesPaths = Directory.GetFiles(finalpath);
                foreach (var filepath in filesPaths)
                {
                    System.IO.File.Delete(filepath);
                }
                Directory.Delete(finalpath);
            }


            // Remove product from database
            _unitofwork.Product.Remove(productToBeDeleted);
            _unitofwork.Save();

            return Json(new { success = true, message = "Delete Successful" });

        }


        #endregion
    }
}

