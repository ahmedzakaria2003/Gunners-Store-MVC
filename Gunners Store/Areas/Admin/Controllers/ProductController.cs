using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models.Models;
using Gunner.Models.ViewModels;
using Gunner.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using ActionNameAttribute = Microsoft.AspNetCore.Mvc.ActionNameAttribute; 
using Controller = Microsoft.AspNetCore.Mvc.Controller; 
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute; 


namespace Gunners_Store.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]


    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProducts = _unitOfWork.ProductReporistory.GetAll(includeProperties: "Category").ToList();
            return View(objProducts);
        }


        #region Create
        public IActionResult Upsert(int? id)
        {



            ProductVM productVM = new()

            {
                CategoryList = _unitOfWork.CategoryReporistory.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

                Product = new Product()

            };

            if (id == 0 || id == null)
            {
                //Create
                return View(productVM);
            }

            else
            {
                //Update

                productVM.Product = _unitOfWork.ProductReporistory.Get(u => u.ProductID == id);

                return View(productVM);

            }

        }

        [HttpPost]

        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string WWWRootPath = _webHostEnvironment.WebRootPath;

                if (file is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(WWWRootPath, @"images\product");

                    // Check and delete old image if it exists
                    if (!string.IsNullOrEmpty(productVM.Product.ImgUrl))
                    {
                        var oldImagePath = Path.Combine(WWWRootPath, "images", "product", Path.GetFileName(productVM.Product.ImgUrl));
                        Console.WriteLine("Old Image Path: " + oldImagePath);  // Log the old image path

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                            Console.WriteLine("Old Image Deleted");
                        }
                        else
                        {
                            Console.WriteLine("Old Image Not Found");
                        }
                    }



                    // Save the new image
                    string filePath = Path.Combine(productPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                        productVM.Product.ImgUrl = @"\images\product\" + fileName;
                    }
                }



                if (productVM.Product.ProductID == 0)
                {
                    _unitOfWork.ProductReporistory.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductReporistory.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }

            productVM.CategoryList = _unitOfWork.CategoryReporistory.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(productVM);
        }




        #endregion

        #region Edit

        public IActionResult Edit(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }
            var productFromDb = _unitOfWork.ProductReporistory.Get(u => u.ProductID == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        [HttpPost]

        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {


                _unitOfWork.ProductReporistory.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Product Updated Successfully";
                return RedirectToAction("Index");
            }

            return View(product);
        }



        #endregion




        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {

            List<Product> objProductList = _unitOfWork.ProductReporistory.GetAll(includeProperties: "Category").ToList();

            return Json(new { data = objProductList });

        }


        public IActionResult Delete(int? id)
        {

            var productToBeDeleted = _unitOfWork.ProductReporistory.Get(u => u.ProductID == id);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, Message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "product", Path.GetFileName(productToBeDeleted.ImgUrl));
            Console.WriteLine("Old Image Path: " + oldImagePath);  // Log the old image path

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
                Console.WriteLine("Old Image Deleted");
            }
            else
            {
                Console.WriteLine("Old Image Not Found");
            }


            _unitOfWork.ProductReporistory.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, Message = "Delete Successuful" });



            #endregion



        }
    }
}

