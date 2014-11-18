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
            else if (nEGOTIATION.CLOSED == false)
            {
                return RedirectToAction("Edit", new { id = id });
            }
            return View(nEGOTIATION);
        }

        // GET: NEGOTIATIONs/Create
        public ActionResult Create()
        {
            var rfps = from x in db.RFPs
                       where x.EXPIRES <= DateTime.Now
                       select x;

            RFP rfp = rfps.FirstOrDefault();

            var vendors = from x in db.VENDORs
                          join y in db.RFPINVITEs
                          on x.Id equals y.Id
                          where y.RFPID == rfp.RFPID
                          where !string.IsNullOrEmpty(y.OFFER_PATH)
                          select x;

            ViewBag.Id = new SelectList(vendors,"Id", "ORGANIZATION");
            ViewBag.RFPID = rfps;
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
                NEGOTIATION neg = new NEGOTIATION { RFPID = nEGOTIATION.RFPID, Id = nEGOTIATION.Id, CLOSED = false };

                db.NEGOTIATIONs.Add(neg);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var rfps = from x in db.RFPs
                       where x.EXPIRES <= DateTime.Now
                       select x;

            RFP rfp = rfps.FirstOrDefault();

            var vendors = from x in db.VENDORs
                          join y in db.RFPINVITEs
                          on x.Id equals y.Id
                          where y.RFPID == rfp.RFPID
                          where !string.IsNullOrEmpty(y.OFFER_PATH)
                          select x;

            ViewBag.Id = new SelectList(vendors, "Id", "ORGANIZATION");
            ViewBag.RFPID = rfps;

            return View(nEGOTIATION);
        }

        public JsonResult GetAcceptedVendors(int RFPID)
        {
            var vendorQuery = from v in db.VENDORs
                                      join y in db.RFPINVITEs
                                      on v.Id equals y.Id
                                      where y.RFPID == RFPID
                                      where !string.IsNullOrEmpty(y.OFFER_PATH)
                                      select new { v.Id, v.ORGANIZATION };

            ViewBag.Id = vendorQuery;

            return Json(vendorQuery, JsonRequestBehavior.AllowGet);
        } 


        // GET: NEGOTIATIONs/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            negindex.negotiation = db.NEGOTIATIONs.Find(id);
            if (negindex.negotiation == null)
            {
                return HttpNotFound();
            }
            else if (negindex.negotiation.CLOSED == true)
            {
                return RedirectToAction("Details", new { id = id });
            }

            var responses = from x in db.RESPONSEs
                            where x.NEGID == negindex.negotiation.NEGID
                            orderby x.CREATED ascending
                            select x;

            negindex.responselist = responses.ToList<RESPONSE>();

            return View(negindex);
        }

        // POST: NEGOTIATIONs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NegIndex model)
        {
            if (ModelState.IsValid)
            {
                if(model.negotiation.CLOSED == true)
                {
                    NEGOTIATION neg = db.NEGOTIATIONs.Find(model.negotiation.NEGID);
                    neg.CLOSED = true;
                }
                if (model.file != null)
                { }
            }
            //ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", nEGOTIATION.Id);
            //ViewBag.RFPID = new SelectList(db.RFPs, "RFPID", "CATEGORY", nEGOTIATION.RFPID);
            return View();
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
