using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();

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
                //if update
                productVM.Product = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            }
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string path = Path.Combine(wwwRootPath, @"images\product");
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    if (!String.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        string oldFilePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }


                    using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }

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

        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Error while delecting" });
            }

            Product productToDelete = _unitOfWork.ProductRepository.Get(o => o.Id == id);
            if (productToDelete == null)
            {
                return Json(new { success = false, message = "Error while delecting" });
            }

            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath,
                productToDelete.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            _unitOfWork.ProductRepository.Remove(productToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfully!" });
        }
        #endregion
    }
}
