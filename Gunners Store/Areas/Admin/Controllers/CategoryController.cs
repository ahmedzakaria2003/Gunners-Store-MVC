using Gunner.DataAccess.Data;
using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models.Models;
using Gunner.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gunners_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitOfWork.CategoryReporistory.GetAll().Select(category => new
            {
                id = category.Id,
                name = category.Name,
                displayOrder = category.DisplayOrder,
            }).ToList();
            return Json(new { data = categoryList });
        }

        [HttpPost]
        public IActionResult ToggleStatus([FromBody] int id)
        {
            var category = _unitOfWork.CategoryReporistory.Get(u => u.Id == id);
            if (category == null)
            {
                return Json(new { success = false, message = "Category not found" });
            }

            _unitOfWork.CategoryReporistory.Update(category);
            _unitOfWork.Save();
            return Json(new { success = true });
        }

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryReporistory.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        #endregion

        #region Edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _unitOfWork.CategoryReporistory.Get(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryReporistory.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _unitOfWork.CategoryReporistory.Get(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePost(Category obj)
        {
            Category? category = _unitOfWork.CategoryReporistory.Get(u => u.Id == obj.Id);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.CategoryReporistory.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
        #endregion
    }
}