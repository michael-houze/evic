using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;


namespace MVC_DATABASE.Controllers
{
    public class NEGOTIATIONsController : Controller
    {
        private EVICEntities db = new EVICEntities();
        private NegIndex negindex = new NegIndex();

        // GET: NEGOTIATIONs
        public ActionResult Index()
        {

            var negotiations = from x in db.NEGOTIATIONs
                               where x.CLOSED == false
                               select x;

            negindex.opennegs = negotiations.ToList<NEGOTIATION>();

            return View(negindex);
        }

        // GET: NEGOTIATIONs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NEGOTIATION nEGOTIATION = db.NEGOTIATIONs.Find(id);
            if (nEGOTIATION == null)
            {
                return HttpNotFound();
            }
            return View(nEGOTIATION);
        }

        // GET: NEGOTIATIONs/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.RFPID = new SelectList(db.RFPs, "RFPID", "CATEGORY");
            return View();
        }

        // POST: NEGOTIATIONs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NEGID,RFPID,Id,CLOSED")] NEGOTIATION nEGOTIATION)
        {
            if (ModelState.IsValid)
            {
                db.NEGOTIATIONs.Add(nEGOTIATION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", nEGOTIATION.Id);
            ViewBag.RFPID = new SelectList(db.RFPs, "RFPID", "CATEGORY", nEGOTIATION.RFPID);
            return View(nEGOTIATION);
        }

        // GET: NEGOTIATIONs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NEGOTIATION nEGOTIATION = db.NEGOTIATIONs.Find(id);
            if (nEGOTIATION == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", nEGOTIATION.Id);
            ViewBag.RFPID = new SelectList(db.RFPs, "RFPID", "CATEGORY", nEGOTIATION.RFPID);
            return View(nEGOTIATION);
        }

        // POST: NEGOTIATIONs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NEGID,RFPID,Id,CLOSED")] NEGOTIATION nEGOTIATION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nEGOTIATION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", nEGOTIATION.Id);
            ViewBag.RFPID = new SelectList(db.RFPs, "RFPID", "CATEGORY", nEGOTIATION.RFPID);
            return View(nEGOTIATION);
        }

        // GET: NEGOTIATIONs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NEGOTIATION nEGOTIATION = db.NEGOTIATIONs.Find(id);
            if (nEGOTIATION == null)
            {
                return HttpNotFound();
            }
            return View(nEGOTIATION);
        }

        // POST: NEGOTIATIONs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NEGOTIATION nEGOTIATION = db.NEGOTIATIONs.Find(id);
            db.NEGOTIATIONs.Remove(nEGOTIATION);
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
