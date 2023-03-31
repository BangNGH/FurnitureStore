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
        public List<ProductCategory> GetNewCategory(int count)
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
            var firstBook = context.Products.FirstOrDefault(p => p.id == id);
            if (firstBook == null)
                return HttpNotFound("Không tìm thấy mã sách này!");
            return View(firstBook);
        }

        public ActionResult GetByCategoryId(int id)
        {
            var products = context.Products.Where(p => p.category_id == id).ToList();
            var category = context.ProductCategories.Find(id);
            ViewBag.CategoryName = category.name;
            return View(products);
        }

    }
}