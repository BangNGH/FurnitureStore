using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FurnitureStore.Models;

namespace FurnitureStore.Areas.Admin.Controllers
{
    public class ContactReceivesController : Controller
    {
        private FurnitureDB db = new FurnitureDB();

        // GET: Admin/ContactReceives
        public ActionResult Index()
        {
            return View(db.ContactReceives.ToList());
        }

        // GET: Admin/ContactReceives/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactReceive contactReceive = db.ContactReceives.Find(id);
            if (contactReceive == null)
            {
                return HttpNotFound();
            }
            return View(contactReceive);
        }

        // GET: Admin/ContactReceives/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ContactReceives/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Email,Phone,Message")] ContactReceive contactReceive)
        {
            if (ModelState.IsValid)
            {
                db.ContactReceives.Add(contactReceive);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactReceive);
        }

        // GET: Admin/ContactReceives/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactReceive contactReceive = db.ContactReceives.Find(id);
            if (contactReceive == null)
            {
                return HttpNotFound();
            }
            return View(contactReceive);
        }

        // POST: Admin/ContactReceives/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Email,Phone,Message")] ContactReceive contactReceive)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactReceive).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactReceive);
        }

        // GET: Admin/ContactReceives/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactReceive contactReceive = db.ContactReceives.Find(id);
            if (contactReceive == null)
            {
                return HttpNotFound();
            }
            return View(contactReceive);
        }

        // POST: Admin/ContactReceives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactReceive contactReceive = db.ContactReceives.Find(id);
            db.ContactReceives.Remove(contactReceive);
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
    }
}
