using FurnitureStore.Models;
using FurnitureStore.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
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

        public ActionResult PayAfter(int id)
        {
            var order = context.Invoices.FirstOrDefault(p => p.id == id);
            order.DeliveryDate = DateTime.Now.AddDays(10);
            context.SaveChanges();
            Session["ShoppingCart"] = null;
            return View();
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
            FurnitureDB context = new FurnitureDB();
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
                        customer_id = currentUserId,
                    };
                    context.Invoices.Add(objInvoices);
                    context.SaveChanges();
                    newOrderNo = objInvoices.id;
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
                            shipping_cost = shippingCost

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
            return RedirectToAction("ConfirmOrder", "ShoppingCart", new { newOrderNo = newOrderNo, shippingCost = shippingCost });

        }
        public ActionResult ConfirmOrder(int newOrderNo, decimal shippingCost)
        {

            ViewBag.newOrderNo = newOrderNo;
            ViewBag.shippingCost = shippingCost.ToString();
            return View();
        }

        public ActionResult Payment(int id, decimal shippingCost)
        {
            List<CartItem> lstShoppingCart = GetShoppingCartFromSession();
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán đơn hàng FurnitureStore";
            string returnUrl = "https://385c-203-205-32-22.ap.ngrok.io/ShoppingCart/ConfirmPaymentClient";
            string notifyurl = "https://385c-203-205-32-22.ap.ngrok.io/ShoppingCart/ConfirmPaymentClient";
            decimal price = lstShoppingCart.Sum(x => x.Price * x.Quatity);
            string amount = ((long)(price + shippingCost)).ToString();
            string orderid = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();

            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = id.ToString();

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
                                      {
                                          { "partnerCode", partnerCode },
                                          { "accessKey", accessKey },
                                          { "requestId", requestId },
                                          { "amount", amount },
                                          { "orderId", orderid },
                                          { "orderInfo", orderInfo },
                                          { "returnUrl", returnUrl },
                                          { "notifyUrl", notifyurl },
                                          { "extraData", extraData },
                                          { "requestType", "captureMoMoWallet" },
                                          { "signature", signature }

                                      };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        public ActionResult ConfirmPaymentClient(Models.Result result)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            Invoice order = new Invoice();
            int orderNo = int.Parse(result.extraData);
            order = context.Invoices.FirstOrDefault(p => p.id == orderNo);
            order.isPaid = true;
            order.DeliveryDate = DateTime.Now.AddDays(10);
            context.SaveChanges();
            Session["ShoppingCart"] = null;
            ViewBag.Message = rMessage;
            ViewBag.OrderId = rOrderId;
            ViewBag.ErrorCode = rErrorCode;
            /*            var orderDetails = context.InvoiceDetails.FirstOrDefault(m => m.invoice_id == order.id);
                        string content = System.IO.File.ReadAllText(Server.MapPath("~/template/FeedBack.html"));
                        content = content.Replace("{{CustomerId}}", order.customer_id);
                        content = content.Replace("{{OrderDate}}", order.OrderDate.ToString());
                        content = content.Replace("{{DeliveryDate}}", order.DeliveryDate.ToString());
                        string state;
                        if (order.isPaid == false)
                        {
                            state = "Chưa thanh toán";
                        }
                        else state = "Đã thanh toán";
                        content = content.Replace("{{ispaid}}", state);
                        content = content.Replace("{{Address}}", orderDetails.delivery_address);

                        var totalprice = context.InvoiceDetails.Where(m => m.invoice_id == order.id).Sum(m => m.price);


                        content = content.Replace("{{Total}}", totalprice.ToString("N0"));
                        var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                        var userId = User.Identity.GetUserName();
                        var emailUser = context.AspNetUsers.Where(m => m.Id == userId).Select(m => m.Email).FirstOrDefault();
                        content = content.Replace("{{Email}}", emailUser);
                        new MailHelper().SendMail(toEmail, "Đơn hàng mới từ FurnitureStore", content);
                        new MailHelper().SendMail(emailUser, "Đơn hàng mới từ FurnitureStore", content);*/
            return View();

        }

    }
}