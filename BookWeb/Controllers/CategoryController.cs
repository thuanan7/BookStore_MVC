using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository context)
        {
            _categoryRepo = context;
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _categoryRepo.GetAll().ToList();
            return View(categoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            //if (category.Name == category.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("", "The DisplayOrder cannot exactly match the Name.");
            //}
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(category);
                _categoryRepo.Save();
                TempData["success"] = "Category has been created!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            Category? category = _categoryRepo.Get(c => c.Id == id);
            //Category? category2 = _context.Categories.FirstOrDefault(c => c.Id == id);
            //Category? category3 = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Update(category);
                _categoryRepo.Save();
                TempData["success"] = "Category has been updated!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        [ActionName("Delete")]
        public IActionResult DeleteConfirm(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            Category? category = _categoryRepo.Get(c => c.Id == id);
            return View(category);
        }
        [HttpPost]
        public IActionResult Delete(int? id)
        {
            Category? category = _categoryRepo.Get(c => c.Id == id);
            if (category != null)
            {
                _categoryRepo.Remove(category);
                _categoryRepo.Save();
                TempData["success"] = "Category has been deleted!";
                return RedirectToAction("Index", "Category");
            }
            return NotFound();
        }
    }
}
