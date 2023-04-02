using FurnitureStore.Models;
using FurnitureStore.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
        [Authorize]
        public RedirectToRouteResult UpdateCart(int id, int txtQuantity)
        {
            var itemFind = GetShoppingCartFromSession().FirstOrDefault(m => m.Id == id);
            if (itemFind != null)
            {
                itemFind.Quatity = txtQuantity;
            }
            return RedirectToAction("Index");

        }


        public ActionResult RemoveCartItem(int? id)
        {
            List<CartItem> lstShoppingCart = GetShoppingCartFromSession();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = lstShoppingCart.FirstOrDefault(m => m.Id == id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            lstShoppingCart.Remove(cartItem);
            return RedirectToAction("Index");
        }

        public ActionResult Order(string delivery_address, decimal shippingCost)
        {
            string currentUserId = User.Identity.GetUserId();
            int newOrderNo;
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Invoice objInvoices = new Invoice()
                    {
                        OrderDate = DateTime.Now,
                        DeliveryDate = null,
                        isComplete = false,
                        isPaid = false,
                        customer_id = currentUserId
                    };
                    context.Invoices.Add(objInvoices);
                    context.SaveChanges();
                    newOrderNo = context.Database.SqlQuery<int>("SELECT TOP 1 id FROM [Invoices] ORDER BY id DESC").FirstOrDefault();

                    List<CartItem> carts = GetShoppingCartFromSession();
                    foreach (var item in carts)
                    {
                        InvoiceDetail invoiceDetail = new InvoiceDetail()
                        {
                            invoice_id = newOrderNo,
                            product_id = item.Id,
                            delivery_address = delivery_address,
                            quantity = item.Quatity,
                            price = item.Price,
                            //Total là phí ship của đơn hàng dô đặt nhằm tên trường
                            Total = shippingCost

                        };
                        context.InvoiceDetails.Add(invoiceDetail);
                        context.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return Content("Order Placement Error!" + e.Message);
                }
            }
            return RedirectToAction("ConfirmOrder", "ShoppingCart", new { newOrderNo = newOrderNo });

        }
        public ActionResult ConfirmOrder(int newOrderNo)
        {

            ViewBag.newOrderNo = newOrderNo.ToString();
            return View();
        }
    }
}