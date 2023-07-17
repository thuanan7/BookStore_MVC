using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            IEnumerable<SelectListItem> CategorySelectList = _unitOfWork.CategoryRepository.GetAll()
                .Select(o => new SelectListItem
                {
                    Text = o.Name,
                    Value = o.Id.ToString()
                });

            //ViewBag will internally insert data with the key to ViewDataDictionary so must not set ViewData and ViewBag have the same key.
            //ViewBag.CategorySelectList = CategorySelectList;
            //ViewData["CategorySelectList"] = CategorySelectList;

            //Avoid using ViewBag and ViewData
            //ViewModel
            ProductVM productVM = new ProductVM()
            {
                CategorySelectList = CategorySelectList,
                Product = new Product()
            };
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "New Book has been created!";
                return RedirectToAction("Index", "Product");
            }
            productVM.CategorySelectList = _unitOfWork.CategoryRepository.GetAll()
                .Select(o => new SelectListItem
                {
                    Text = o.Name,
                    Value = o.Id.ToString()
                });
            return View(productVM);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            Product product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
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
