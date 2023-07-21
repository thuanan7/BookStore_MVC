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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> Companys = _unitOfWork.CompanyRepository.GetAll().ToList();

            return View(Companys);
        }

        //Update(edit) + Insert(create) = Upsert
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }

            //update
            Company company = _unitOfWork.CompanyRepository.Get(u=>u.Id == id);
            return View(company);
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(company);
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(company);
                }

                _unitOfWork.Save();
                TempData["success"] = "New Company has been created!";

                return RedirectToAction("Index", "Company");
            }

            return View(company);
        }

        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companies = _unitOfWork.CompanyRepository.GetAll().ToList();
            return Json(new { data = Companies });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Error while delecting" });
            }

            Company CompanyToDelete = _unitOfWork.CompanyRepository.Get(o => o.Id == id);
            if (CompanyToDelete == null)
            {
                return Json(new { success = false, message = "Error while delecting" });
            }

            _unitOfWork.CompanyRepository.Remove(CompanyToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfully!" });
        }
        #endregion
    }
}
