﻿using EcommerceShop.DAL;
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
        public List<Tbl_Brand> GetBrand()
        {

            int memberId = GetCurrentMemberId();


            var brand = _unitOfWork.GetRepositoryInstance<Tbl_Brand>()
                .GetAllRecords()
                .Where(c => c.MemberId == memberId && !c.IsDelete.GetValueOrDefault())
                .ToList();

            return brand;
        }

        public List<Tbl_Store> GetStore()
        {

            int memberId = GetCurrentMemberId();


            var store = _unitOfWork.GetRepositoryInstance<Tbl_Store>()
                .GetAllRecords()
                .Where(c => c.MemberId == memberId && !c.IsDelete.GetValueOrDefault())
                .ToList();

            return store;
        }

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

        public ActionResult AddCategory()
        {
            var categories = GetCategories();

            ViewBag.CategoryList = new SelectList(categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(Tbl_Category tbl)
        {
       
            int memberId = GetCurrentMemberId(); 

        
            tbl.MemberId = memberId;
            tbl.IsActive = true;
            tbl.IsDelete = false;
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Add(tbl);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Categories");
        }

        public ActionResult CategoryEdit(int catId)
        {
            int memberId = GetCurrentMemberId();

            var category = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords().FirstOrDefault(p => p.CategoryId == catId && p.MemberId == memberId);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        public ActionResult CategoryEdit(Tbl_Category tbl)
        {
            int memberId = GetCurrentMemberId();

        
            var existingCategory = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstorDefault(tbl.CategoryId);
            if (existingCategory == null || existingCategory.MemberId != memberId)
            {
                return HttpNotFound(); 
            }

            existingCategory.CategoryName = tbl.CategoryName;
            existingCategory.IsActive = tbl.IsActive;
            existingCategory.IsDelete = tbl.IsDelete;


            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Update(existingCategory);
            _unitOfWork.SaveChanges(); 

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



        public ActionResult Brand()
        {

            int memberId = GetCurrentMemberId();


            List<Tbl_Brand> userBrand = _unitOfWork.GetRepositoryInstance<Tbl_Brand>()
                .GetAllRecords()
                .Where(c => c.MemberId == memberId && !c.IsDelete.GetValueOrDefault())
                .ToList();

            return View(userBrand);
        }

        public ActionResult AddBrand()
        {
            var brand = GetBrand();

            ViewBag.BrandList = new SelectList(brand, "BrandId", "BrandName");
            return View();
        }

        [HttpPost]
        public ActionResult AddBrand(Tbl_Brand tbl)
        {

            int memberId = GetCurrentMemberId();


            tbl.MemberId = memberId;
            tbl.IsActive = true;
            tbl.IsDelete = false;
            _unitOfWork.GetRepositoryInstance<Tbl_Brand>().Add(tbl);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Brand");
        }

        public ActionResult BrandEdit(int brandId)
        {
            int memberId = GetCurrentMemberId(); 

            var brand = _unitOfWork.GetRepositoryInstance<Tbl_Brand>().GetAllRecords().FirstOrDefault(p => p.BrandId == brandId && p.MemberId == memberId);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        [HttpPost]
        public ActionResult BrandEdit(Tbl_Brand tbl)
        {
            int memberId = GetCurrentMemberId();

     
            var existingBrand = _unitOfWork.GetRepositoryInstance<Tbl_Brand>().GetFirstorDefault(tbl.BrandId);
            if (existingBrand == null || existingBrand.MemberId != memberId)
            {
                return HttpNotFound(); 
            }

            existingBrand.BrandName = tbl.BrandName;
            existingBrand.IsActive = tbl.IsActive;
            existingBrand.IsDelete = tbl.IsDelete;
    
      
            _unitOfWork.GetRepositoryInstance<Tbl_Brand>().Update(existingBrand);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Brand");
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

        public ActionResult ProductEdit(int productId)
        {
            int memberId = GetCurrentMemberId();

            ViewBag.CategoryList = new SelectList(GetCategories(), "CategoryId", "CategoryName");
            ViewBag.BrandList = new SelectList(GetBrand(), "BrandId", "BrandName");
            var product = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetAllRecords().FirstOrDefault(p => p.ProductId == productId && p.MemberId == memberId);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult ProductEdit(Tbl_Product tbl, HttpPostedFileBase file)
        {
            int memberId = GetCurrentMemberId();

    
            var existingProduct = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstorDefault(tbl.ProductId);
            if (existingProduct == null || existingProduct.MemberId != memberId)
            {
                return HttpNotFound();
            }

            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                file.SaveAs(path);
            }
            existingProduct.ProductName = tbl.ProductName;
            existingProduct.Description = tbl.Description;
            existingProduct.Price = tbl.Price;
            existingProduct.BrandId = tbl.BrandId;
            existingProduct.Quantity = tbl.Quantity;
            existingProduct.CategoryId = tbl.CategoryId;
            existingProduct.IsFeatured = tbl.IsFeatured;
            existingProduct.IsActive = tbl.IsActive;
            existingProduct.IsDelete = tbl.IsDelete;

        
            if (file != null)
            {
                existingProduct.ProductImage = pic;
            }

            existingProduct.ModifiedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Update(existingProduct);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Product");
        }


        public ActionResult ProductAdd()
        {
            ViewBag.CategoryList = new SelectList(GetCategories(), "CategoryId", "CategoryName");
            ViewBag.BrandList = new SelectList(GetBrand(), "BrandId", "BrandName");
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
            tbl.IsActive = true;
            tbl.IsDelete = false;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Add(tbl);
            return RedirectToAction("Product");
        }

        public ActionResult Store()
        {

            int memberId = GetCurrentMemberId();


            List<Tbl_Store> userStore = _unitOfWork.GetRepositoryInstance<Tbl_Store>()
                .GetAllRecords()
                .Where(c => c.MemberId == memberId && !c.IsDelete.GetValueOrDefault())
                .ToList();

            return View(userStore);
        }

        public ActionResult AddStore()
        {
            var store = GetStore();

            ViewBag.BrandList = new SelectList(store, "StoreId", "StoreName");
            return View();
        }

        [HttpPost]
        public ActionResult AddStore(Tbl_Store tbl)
        {

            int memberId = GetCurrentMemberId();


            tbl.MemberId = memberId;
            tbl.IsActive = true;
            tbl.IsDelete = false;
            _unitOfWork.GetRepositoryInstance<Tbl_Store>().Add(tbl);
            _unitOfWork.SaveChanges();
            return RedirectToAction("Store");
        }

        public ActionResult StoreEdit(int storeId)
        {
            int memberId = GetCurrentMemberId();

            var store = _unitOfWork.GetRepositoryInstance<Tbl_Store>().GetAllRecords().FirstOrDefault(p => p.StoreId == storeId && p.MemberId == memberId);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        [HttpPost]
        public ActionResult StoreEdit(Tbl_Store tbl)
        {
            int memberId = GetCurrentMemberId();


            var existingStore = _unitOfWork.GetRepositoryInstance<Tbl_Store>().GetFirstorDefault(tbl.StoreId);
            if (existingStore == null || existingStore.MemberId != memberId)
            {
                return HttpNotFound();
            }

            existingStore.storeName = tbl.storeName;
            existingStore.phone = tbl.phone;
            existingStore.email = tbl.email;
            existingStore.street = tbl.street;
            existingStore.city = tbl.city;
            existingStore.states = tbl.states;
            existingStore.zip_code = tbl.zip_code;
            existingStore.IsActive = tbl.IsActive;
            existingStore.IsDelete = tbl.IsDelete;


            _unitOfWork.GetRepositoryInstance<Tbl_Store>().Update(existingStore);
            _unitOfWork.SaveChanges();

            return RedirectToAction("Store");
        }
    }
}