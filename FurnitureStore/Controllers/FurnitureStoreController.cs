﻿using FurnitureStore.Models;
using FurnitureStore.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace FurnitureStore.Controllers
{
    public class FurnitureStoreController : Controller
    {
        // GET: FurnitureStore
        private FurnitureDB context = new FurnitureDB();
        private IEnumerable<Product> NewProduct(int count)
        {
            return context.Products.OrderByDescending(z => z.id).Take(count).ToList();
        }
        public IEnumerable<ProductCategory> GetNewCategory(int count)
        {
            return context.ProductCategories.OrderByDescending(z => z.id).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var newProducts = NewProduct(4);
            var newCategory = GetNewCategory(4);
            var viewModel = new ProductViewModel
            {
                Products = newProducts,
                Categories = newCategory
            };
            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            var detailsBook = context.Products.Where(p => p.id == id).ToList();
            if (detailsBook == null)
                return HttpNotFound("Không tìm thấy mã sách này!");
            var viewModel = new ProductViewModel
            {
                Products = detailsBook
            };
            return View(viewModel);
        }

        public ActionResult GetByCategoryId(int id)
        {
            var products = context.Products.Where(p => p.category_id == id).ToList();
            var category = context.ProductCategories.Find(id);
            var viewModel = new ProductViewModel
            {
                Products = products
            };
            ViewBag.CategoryName = category.name;
            return View("GetAllProducts", viewModel);
        }

        public ActionResult GetAllProducts(int? page)
        {
            int pageSize = 4;
            int pageIndex = page.HasValue ? page.Value : 1;
            var products = context.Products.ToList();
            var pagedListProducts = products.ToPagedList(pageIndex, pageSize);
            ViewBag.CurrentPage = pageIndex;
            var viewModel = new ProductViewModel
            {
                Products = products,
                pagedList = pagedListProducts
            };

            return View(viewModel);
        }

        public ActionResult AboutMe()
        {
            var products = context.Products.ToList();
            var viewModel = new ProductViewModel
            {
                Products = products
            };
            return View(viewModel);
        }
        public ActionResult ContactUs()
        {
            var products = context.Products.ToList();
            var viewModel = new ProductViewModel
            {
                Products = products
            };
            return View(viewModel);
        }

        public ActionResult Search(string searchString, int? page)
        {
            var rs = (from m in context.Products where m.name.Contains(searchString) || m.manufacturer.Contains(searchString) select m).ToList();
            if (rs.Count() > 0)
            {
                int pageSize = 4;
                int pageIndex = page.HasValue ? page.Value : 1;
                var result = rs.ToPagedList(pageIndex, pageSize);
                ViewBag.CurrentPage = pageIndex;
                var viewModel = new ProductViewModel
                {
                    Products = rs,
                    pagedList = result

                };
                ViewBag.CurrentPage = pageIndex;
                return View("GetAllProducts", viewModel);
            }
            return HttpNotFound("Not found product!");
        }
        [HttpPost]
        public ActionResult SendContact(string name, string email, string phone, string message)
        {
            if (ModelState.IsValid)
            {
                ContactReceive sendContact = new ContactReceive();
                sendContact.Name = name;
                sendContact.Email = email;
                sendContact.Phone = phone;
                sendContact.Message = message;
                context.ContactReceives.Add(sendContact);
                context.SaveChanges();
                return View();
            }
            else
            {
                Debug.WriteLine("Validation errors:");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Debug.WriteLine(error.ErrorMessage);
                    }
                }
                return View("Index");
            }
        }
        public ActionResult GiveFeedback(int id)
        {
            var products = context.Products.ToList();

            var viewModel = new ProductViewModel
            {
                Products = products,

            };
            ViewBag.ProductId = id.ToString();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult SaveFeedBack(int rating, string feedback)
        {
            var currentUserId = User.Identity.GetUserId();
            var productId = int.Parse(Request.Form["productId"]);
            var newFeedback = new feedback
            {
                userId = currentUserId,
                product_id = productId,
                rate = rating,
                Message = feedback,
            };
            context.feedbacks.Add(newFeedback);
            try
            {
                context.SaveChanges();
                return RedirectToAction("Index", "FurnitureStore");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var errors in ex.EntityValidationErrors)
                {
                    foreach (var error in errors.ValidationErrors)
                    {
                        // Hiển thị lỗi xác thực
                        Debug.WriteLine("Property: " + error.PropertyName + " Error: " + error.ErrorMessage);
                    }
                }
                // Không lưu và trả về một trang lỗi
                return View("Error");
            }


        }

        public ActionResult SeeFeedback(int id)
        {
            var feedback_product = context.feedbacks.Where(m => m.product_id == id).ToList();

            return View(feedback_product);
        }



    }

}
