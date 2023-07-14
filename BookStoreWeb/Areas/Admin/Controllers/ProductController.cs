using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.ProductRepository.GetAll().ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(product);
                _unitOfWork.Save();
                TempData["success"] = "New Book has been created!";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            Product product = _unitOfWork.ProductRepository.Get(p=>p.Id == id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Book has been updated!";
                return RedirectToAction("Index", "Product");
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
            Product product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            Product? product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
            if (product != null)
            {
                _unitOfWork.ProductRepository.Remove(product);
                _unitOfWork.Save();
                TempData["success"] = "Book has been deleted!";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }
    }
}
