using FurnitureStore.Models;
using FurnitureStore.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FurnitureStore.Areas.Admin.Controllers
{
    public class InvoiceDetailsController : Controller
    {
        private FurnitureDB db = new FurnitureDB();

        // GET: Admin/InvoiceDetails
        public ActionResult Index()
        {
            return View(db.InvoiceDetails.ToList());
        }

        // GET: Admin/InvoiceDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceDetail invoiceDetail = db.InvoiceDetails.Find(id);
            if (invoiceDetail == null)
            {
                return HttpNotFound();
            }
            return View(invoiceDetail);
        }

        // GET: Admin/InvoiceDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/InvoiceDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "invoice_id,product_id,delivery_address,quantity,price,shipping_cost")] InvoiceDetail invoiceDetail)
        {
            if (ModelState.IsValid)
            {
                db.InvoiceDetails.Add(invoiceDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invoiceDetail);
        }

        // GET: Admin/InvoiceDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceDetail invoiceDetail = db.InvoiceDetails.Find(id);
            if (invoiceDetail == null)
            {
                return HttpNotFound();
            }
            return View(invoiceDetail);
        }

        // POST: Admin/InvoiceDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "invoice_id,product_id,delivery_address,quantity,price,shipping_cost")] InvoiceDetail invoiceDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoiceDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invoiceDetail);
        }

        // GET: Admin/InvoiceDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceDetail invoiceDetail = db.InvoiceDetails.Find(id);
            if (invoiceDetail == null)
            {
                return HttpNotFound();
            }
            return View(invoiceDetail);
        }

        // POST: Admin/InvoiceDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvoiceDetail invoiceDetail = db.InvoiceDetails.Find(id);
            db.InvoiceDetails.Remove(invoiceDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult TotalRevenue()
        {
            decimal totalRevenue = 0;
            var invoiceDetails = db.InvoiceDetails.ToList();
            var invoices = from invoice in db.Invoices
                           where invoice.DeliveryDate == null
                           select invoice;
            foreach (var invoice_details in invoiceDetails)
            {
                totalRevenue += invoice_details.price;
            }
            ViewBag.TotalRevenue = ((long)totalRevenue).ToString("N0");
            ViewBag.Pending = invoices.Count();
            return View();
        }


        public ActionResult BestSellingProducts()
        {
            // Lấy danh sách id của những sản phẩm bán chạy
            var bestSellingProductsId = db.InvoiceDetails
                .OrderByDescending(p => p.quantity)
                .Take(5)
                .Select(p => p.product_id)
                .ToList();

            var revenueList = db.InvoiceDetails
    .Where(p => bestSellingProductsId.Contains(p.product_id))
    .GroupBy(p => p.product_id)
    .Select(g => new
    {
        ProductId = g.Key,
        Revenue = g.Sum(p => p.price)
    })
    .ToList();

            var revenueViewModelList = new List<ProductRevenue>();
            foreach (var revenue in revenueList)
            {
                revenueViewModelList.Add(new ProductRevenue
                {
                    ProductId = revenue.ProductId,
                    Revenue = revenue.Revenue
                });
            }


            var productRevenueList = new List<ProductRevenue>();
            foreach (var item in revenueList)
            {
                productRevenueList.Add(new ProductRevenue
                {
                    ProductId = item.ProductId,
                    Revenue = item.Revenue
                });
            }


            // Tìm những sản phẩm trong bảng Product có id nằm trong danh sách bestSellingProductsId để lưu vào biến bestSellingProducts
            var bestSellingProducts = db.Products
                .Where(p => bestSellingProductsId.Contains(p.id))
                .ToList();


            var viewModel = new ProductViewModel
            {
                Products = bestSellingProducts,
                RevenueList = revenueViewModelList
            };

            // Trả về view hiển thị danh sách sản phẩm
            return View(viewModel);
        }

    }
}
