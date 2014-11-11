using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;

namespace MVC_DATABASE.Controllers
{
    public class CONTRACTsController : Controller
    {
        private EVICEntities db = new EVICEntities();
        private ContractIndex contractindex = new ContractIndex();

        // GET: CONTRACTs
        public ActionResult Index()
        {


            var indexview = from x in db.CONTRACTs
                            join y in db.RFPs
                            on x.RFPID equals y.RFPID
                            join z in db.VENDORs
                            on x.Id equals z.Id
                            select new ContractIndex { contractID = x.CONTRACTID, rfpID = y.RFPID, category = y.CATEGORY, contractPath = x.CONTRACT_PATH, organization = z.ORGANIZATION };



            
            
            return View(indexview.ToList<ContractIndex>());
        }

        // GET: CONTRACTs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONTRACT cONTRACT = await db.CONTRACTs.FindAsync(id);
            if (cONTRACT == null)
            {
                return HttpNotFound();
            }
            return View(cONTRACT);
        }

        public JsonResult GetAcceptedVendors(int RFPID)
        {
            EVICEntities dbo = new EVICEntities();

            var vendorProductsQuery = from v in dbo.VENDORs
                                      join c in dbo.RFPINVITEs
                                      on v.Id equals c.Id
                                      join r in dbo.RFPs
                                      on c.RFPID equals r.RFPID
                                      where c.OFFER_PATH != string.Empty
                                      where r.RFPID == RFPID
                                      select new {v.Id, v.ORGANIZATION };

            ViewBag.AcceptedVendors = vendorProductsQuery;

            return Json(vendorProductsQuery, JsonRequestBehavior.AllowGet);
        } 

        // GET: CONTRACTs/Create
        public ActionResult Create()
        {
            var rfpidquery = from x in db.RFPs
                             where x.EXPIRES <= DateTime.Now
                             select x.RFPID;

            ViewBag.rfpid = rfpidquery;

            var templates = from x in db.TEMPLATEs
                            where x.TYPE == "CONTRACT"
                            select x;

            ViewBag.templates = new SelectList(templates, "TEMPLATEID", "TEMPLATEID");

            var vendorProductsQuery = from v in db.VENDORs
                                      join c in db.RFPINVITEs
                                      on v.Id equals c.Id
                                      join r in db.RFPs
                                      on c.RFPID equals r.RFPID
                                      where c.OFFER_PATH != string.Empty
                                      where r.RFPID == rfpidquery.FirstOrDefault()
                                      select new { v.Id, v.ORGANIZATION };

            ViewBag.AcceptedVendors = vendorProductsQuery;

            return View();
        }

        // POST: CONTRACTs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CONTRACTID,Id,TEMPLATEID,RFPID,CONTRACT_PATH")] CONTRACT cONTRACT)
        {
            if (ModelState.IsValid)
            {
                db.CONTRACTs.Add(cONTRACT);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", cONTRACT.Id);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", cONTRACT.TEMPLATEID);
            return View(cONTRACT);
        }

        // GET: CONTRACTs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONTRACT cONTRACT = await db.CONTRACTs.FindAsync(id);
            if (cONTRACT == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", cONTRACT.Id);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", cONTRACT.TEMPLATEID);
            return View(cONTRACT);
        }

        // POST: CONTRACTs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CONTRACTID,Id,TEMPLATEID,RFPID,CONTRACT_PATH")] CONTRACT cONTRACT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cONTRACT).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", cONTRACT.Id);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", cONTRACT.TEMPLATEID);
            return View(cONTRACT);
        }

        // GET: CONTRACTs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONTRACT cONTRACT = await db.CONTRACTs.FindAsync(id);
            if (cONTRACT == null)
            {
                return HttpNotFound();
            }
            return View(cONTRACT);
        }

        // POST: CONTRACTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CONTRACT cONTRACT = await db.CONTRACTs.FindAsync(id);
            db.CONTRACTs.Remove(cONTRACT);
            await db.SaveChangesAsync();
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
