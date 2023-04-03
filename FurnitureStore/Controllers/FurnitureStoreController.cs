using FurnitureStore.Models;
using FurnitureStore.ViewModels;
using System.Collections.Generic;
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

        public ActionResult GetAllProducts()
        {
            var products = context.Products.ToList();
            var viewModel = new ProductViewModel
            {
                Products = products
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

        public ActionResult Search(string searchString)
        {
            var rs = (from m in context.Products where m.name.Contains(searchString) select m);
            var viewModel = new ProductViewModel
            {
                Products = rs
            };
            if (rs.Count() > 0)
            {
                return View("GetAllProducts", viewModel);
            }
            return HttpNotFound("Not found product!");
        }

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
                return View("Index");
            }
        }



    }
}