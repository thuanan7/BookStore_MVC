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

        //Update(edit) + Insert(create) = Upsert
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                CategorySelectList = _unitOfWork.CategoryRepository.GetAll()
                .Select(o => new SelectListItem
                {
                    Text = o.Name,
                    Value = o.Id.ToString()
                }),
                Product = new Product()
            };
            if (id != null && id != 0)
            {
                productVM.Product = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            }
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    //Create
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "New Book has been created!";
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    //Update
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Book has been updated!";
                    return RedirectToAction("Index", "Product");
                }
            }
            productVM.CategorySelectList = _unitOfWork.CategoryRepository.GetAll()
                .Select(o => new SelectListItem
                {
                    Text = o.Name,
                    Value = o.Id.ToString()
                });

            return View(productVM);
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
