using FurnitureStore.Models;
using FurnitureStore.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FurnitureStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        private FurnitureDB context = new FurnitureDB();
        public ActionResult Index()
        {
            List<CartItem> lstShoppingCart = GetShoppingCartFromSession();
            if (lstShoppingCart.Count == 0)
                return RedirectToAction("Index", "FurnitureStore");
            var products = context.Products.ToList();
            var viewModel = new ProductViewModel
            {
                CartItems = lstShoppingCart,
                Products = products
            };
            ViewBag.Quatity = lstShoppingCart.Sum(x => x.Quatity);
            ViewBag.Total = lstShoppingCart.Sum(x => x.Price * x.Quatity);
            return View(viewModel);
        }
        public List<CartItem> GetShoppingCartFromSession()
        {
            var lstShoppingCart = Session["ShoppingCart"] as List<CartItem>;
            if (lstShoppingCart == null)
            {
                lstShoppingCart = new List<CartItem>();
                Session["ShoppingCart"] = lstShoppingCart;
            }
            return lstShoppingCart;
        }

        [Authorize]
        public RedirectToRouteResult AddToCart(int id)
        {
            List<CartItem> lstShoppingCart = GetShoppingCartFromSession();
            CartItem findCart = lstShoppingCart.FirstOrDefault(m => m.Id == id);
            if (findCart == null)
            {
                var findProduct = context.Products.FirstOrDefault(m => m.id == id);
                var categoryId = context.ProductCategories.Find(findProduct.category_id);
                CartItem newItem = new CartItem()
                {
                    Id = findProduct.id,
                    Name = findProduct.name,
                    CategoryName = categoryId.name,
                    Quatity = 1,
                    Image = findProduct.Image,
                    Price = findProduct.price
                };
                lstShoppingCart.Add(newItem);
            }
            else
            {
                findCart.Quatity++;
            }
            return RedirectToAction("Index", "ShoppingCart");
        }

        public ActionResult CartSummary()
        {
            ViewBag.CartCount = GetShoppingCartFromSession().Count();
            return PartialView("CartSummary");
        }

    }
}