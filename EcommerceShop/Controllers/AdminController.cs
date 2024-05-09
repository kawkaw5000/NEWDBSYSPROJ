using EcommerceShop.DAL;
using EcommerceShop.Models;
using EcommerceShop.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EcommerceShop.Controllers
{
    [Authorize(Roles = "Manager, Admin")]
    public class AdminController : BaseController
    { 
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        public List<SelectListItem> GetMembers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var mem = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetAllRecords();
            foreach (var item in mem)
            {
                list.Add(new SelectListItem { Value = item.id.ToString(), Text = item.Username });
            }
            return list;
        }

        public List<Tbl_Category> GetCategories()
        {
         
            int memberId = GetCurrentMemberId(); 

     
            var categories = _unitOfWork.GetRepositoryInstance<Tbl_Category>()
                .GetAllRecords()
                .Where(c => c.MemberId == memberId && !c.IsDelete.GetValueOrDefault())
                .ToList();

            return categories;
        }



        //public List<SelectListItem> GetCategory()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    var cat = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords().Where(c => !c.IsDelete.GetValueOrDefault());
        //    foreach (var item in cat)
        //    {
        //        list.Add(new SelectListItem { Value = item.CategoryId.ToString(), Text = item.CategoryName });
        //    }
        //    return list;
        //}

        public ActionResult Dashboard()
        {
            return View();
        }
      
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // ADMIN USER EDIT------------------------------------------------------------
        [Authorize(Roles = "Admin")]
        public ActionResult Members()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetMembers());
        }
        public ActionResult MembersEdit(int memberId)
        {
           
            ViewBag.MembersList = GetMembers();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstorDefault(memberId));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult MembersEdit(Tbl_Members model)
        {
            if (ModelState.IsValid)
            {
                var member = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstorDefault(model.id);
                if (member != null)
                {
                    member.IsActive = model.IsActive;
                    member.IsDelete = model.IsDelete;
                    

                    _unitOfWork.GetRepositoryInstance<Tbl_Members>().Update(member);
                    _unitOfWork.SaveChanges();
                    return RedirectToAction("Members"); 
                }
                else
                {
                    ModelState.AddModelError("", "Member not found.");
                }
            }

     
            ViewBag.MembersList = GetMembers();
            return View(model);
        }

        // ADMIN CATEGORIES EDIT------------------------------------------------------------
        public ActionResult Categories()
        {
        
            int memberId = GetCurrentMemberId(); 

        
            List<Tbl_Category> userCategories = _unitOfWork.GetRepositoryInstance<Tbl_Category>()
                .GetAllRecords()
                .Where(c => c.MemberId == memberId && !c.IsDelete.GetValueOrDefault())
                .ToList();

            return View(userCategories);
        }
        //public ActionResult Categories()
        //{
        //    int memberId = GetCurrentMemberId();
        //    List<Tbl_Category> AllCategories = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecordsIQueryable().Where(i => i.IsDelete == false).ToList();
        //    return View(AllCategories);
        //}

        public ActionResult AddCategory()
        {
            var categories = GetCategories();

            ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
            return View();
        }

        //public ActionResult AddCategory()
        //{
        //    // Retrieve the currently logged-in user's ID
        //    int memberId = GetCurrentMemberId(); // Implement this method to get the member ID

        //    // Filter categories based on the logged-in user
        //    List<Tbl_Category> userCategories = _unitOfWork.GetRepositoryInstance<Tbl_Category>()
        //        .GetAllRecords()
        //        .Where(c => c.MemberId == memberId && !c.IsDelete.GetValueOrDefault())
        //        .ToList();

        //    ViewBag.CategoryList = userCategories;
        //    return View();
        //}


        [HttpPost]
        public ActionResult AddCategory(Tbl_Category tbl)
        {
       
            int memberId = GetCurrentMemberId(); 

        
            tbl.MemberId = memberId;

            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Add(tbl);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Categories");
        }

        //[HttpPost]
        //public ActionResult AddCategory(Tbl_Category tbl)
        //{
        //    int memberId = GetCurrentMemberId();
        //    _unitOfWork.GetRepositoryInstance<Tbl_Category>().Add(tbl);
        //    return RedirectToAction("Categories");

        //}

        public ActionResult CategoryEdit(int catId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstorDefault(catId));
        }
        [HttpPost]
        public ActionResult CategoryEdit(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Update(tbl);
            return RedirectToAction("Categories");
        }

        private int GetCurrentMemberId()
        {    
            string loggedInUserEmail = User.Identity.Name;       
            var user = _unitOfWork.GetRepositoryInstance<Tbl_Members>()
                                  .GetAllRecords()
                                  .FirstOrDefault(m => m.Username == loggedInUserEmail);

            if (user != null)
            {
         
                return user.id;
            }
            else
            {  
                throw new InvalidOperationException("Member not found for the logged-in user.");
            }
        }

        // ADMIN PRODUCT EDIT------------------------------------------------------------
        public ActionResult Product()
        { 
            int memberId = GetCurrentMemberId(); 

            var products = _unitOfWork.GetRepositoryInstance<Tbl_Product>()
                .GetProduct()
                .Where(p => !(p.IsDelete ?? false) && p.MemberId == memberId);

            return View(products);
        }
        //            public ActionResult Product()
        //{
        //            // Filter out products with IsDelete = true
        //            var products = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetProduct().Where(p => !(p.IsDelete ?? false));
        //            return View(products);
        //}
        //public ActionResult Product()
        //{
        //    return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetProduct());
        //}

        public ActionResult ProductEdit(int productId)
        {
            ViewBag.CategoryList = new SelectList(GetCategories(), "CategoryId", "CategoryName");
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstorDefault(productId));
        }
        [HttpPost]
        public ActionResult ProductEdit(Tbl_Product tbl, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                file.SaveAs(path);
            }
            tbl.ProductImage = file != null ? pic : tbl.ProductImage;
            tbl.ModifiedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Update(tbl);
            return RedirectToAction("Product");
        }
        public ActionResult ProductAdd()
        {
            ViewBag.CategoryList = new SelectList(GetCategories(), "CategoryId", "CategoryName");
            return View();
        }
        [HttpPost]
        public ActionResult ProductAdd(Tbl_Product tbl, HttpPostedFileBase file)
        {
          
            int memberId = GetCurrentMemberId(); 

            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                file.SaveAs(path);
            }

            tbl.ProductImage = pic;
            tbl.CreatedDate = DateTime.Now;
            tbl.MemberId = memberId; 
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Add(tbl);
            return RedirectToAction("Product");
        }

        //[HttpPost]
        //public ActionResult ProductAdd(Tbl_Product tbl, HttpPostedFileBase file)
        //{
        //    string pic = null;
        //    if (file != null)
        //    {
        //        pic = System.IO.Path.GetFileName(file.FileName);
        //        string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
        //        file.SaveAs(path);
        //    }
        //    tbl.ProductImage = pic;
        //    tbl.CreatedDate = DateTime.Now;
        //    _unitOfWork.GetRepositoryInstance<Tbl_Product>().Add(tbl);
        //    return RedirectToAction("Product");
        //}

    }
}