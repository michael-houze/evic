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
    public class RFPsController : Controller
    {
        private BaptistEntities db = new BaptistEntities();

        // GET: RFPs
        public async Task<ActionResult> Index()
        {
            var rFPs = db.RFPs.Include(r => r.RFI).Include(r => r.TEMPLATE);
            return View(await rFPs.ToListAsync());
        }

        // GET: RFPs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RFP rFP = await db.RFPs.FindAsync(id);
            if (rFP == null)
            {
                return HttpNotFound();
            }
            return View(rFP);
        }

        // GET: RFPs/Create
        public ActionResult Create()
        {
            var result = from r in db.OFFEREDCATEGORies
                         where r.ACCEPTED == true
                         select r.CATEGORY;


            IQueryable<string> acceptedCategories = result.Distinct();
            ViewBag.CATEGORY = acceptedCategories;
            ViewBag.RFIID = new SelectList(db.RFIs, "RFIID", "CATEGORY");
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE");
            return View();
        }
        public RFPCreate rfpcreate = new RFPCreate();
        // POST: RFPs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RFPCreate model)
        {
            if (ModelState.IsValid)
            {
                db.RFPs.Add(model.rfp);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RFIID = new SelectList(db.RFIs, "RFIID", "CATEGORY", model.rfp.RFIID);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", model.rfp.TEMPLATEID);
            return View(model);
        }

        // GET: RFPs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RFP rFP = await db.RFPs.FindAsync(id);
            if (rFP == null)
            {
                return HttpNotFound();
            }
            ViewBag.RFIID = new SelectList(db.RFIs, "RFIID", "CATEGORY", rFP.RFIID);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", rFP.TEMPLATEID);
            return View(rFP);
        }

        // POST: RFPs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RFPID,RFIID,CATEGORY,TEMPLATEID,CREATED,EXPIRES")] RFP rFP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rFP).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.RFIID = new SelectList(db.RFIs, "RFIID", "CATEGORY", rFP.RFIID);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", rFP.TEMPLATEID);
            return View(rFP);
        }

        // GET: RFPs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RFP rFP = await db.RFPs.FindAsync(id);
            if (rFP == null)
            {
                return HttpNotFound();
            }
            return View(rFP);
        }

        // POST: RFPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RFP rFP = await db.RFPs.FindAsync(id);
            db.RFPs.Remove(rFP);
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

        //Vendor RFPs

        // GET: RFPs/Edit/5
        public async Task<ActionResult> VendorEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RFP rFP = await db.RFPs.FindAsync(id);
            if (rFP == null)
            {
                return HttpNotFound();
            }
            ViewBag.RFIID = new SelectList(db.RFIs, "RFIID", "CATEGORY", rFP.RFIID);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", rFP.TEMPLATEID);
            return View(rFP);
        }

        // POST: RFPs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VendorEdit([Bind(Include = "RFPID,RFIID,CATEGORY,TEMPLATEID,CREATED,EXPIRES")] RFP rFP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rFP).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.RFIID = new SelectList(db.RFIs, "RFIID", "CATEGORY", rFP.RFIID);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", rFP.TEMPLATEID);
            return View(rFP);
        }

    }
}
