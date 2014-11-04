using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;

namespace MVC_DATABASE.Controllers
{
    public class RFPsController : Controller
    {
        private EVICEntities db = new EVICEntities();

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

        RFPVendorRespond.RFPList respondmodel = new RFPVendorRespond.RFPList();

        [HttpGet]
        public async Task<ActionResult> VendorRespond(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            respondmodel.rfp = await db.RFPs.FindAsync(id);

            if (respondmodel.rfp == null)
            {
                return HttpNotFound();
            }

            respondmodel.inviteList = new List<RFPINVITE>();

            foreach (var x in db.RFPINVITEs.ToList())
            {

                if (x.RFPID == id)
                {

                    respondmodel.inviteList.Add(x);

                }
            }
            return View(respondmodel);
        }

        RFPVendorIndex rfpvendorindex = new RFPVendorIndex();

        [HttpGet]
        public ActionResult VendorIndex(int? id)
        {

            EVICEntities dbo = new EVICEntities();

            var user_ids = User.Identity.GetUserId();

            var VendorRFPIDQuery = from r in dbo.RFPs
                                   join i in dbo.RFPINVITEs
                                   on r.RFPID equals i.RFPID
                                   where i.Id == user_ids
                                   orderby r.RFPID
                                   select new RFPVendorIndex { VendorRFP = r, VendorRFPInvite = i };

            return View(VendorRFPIDQuery);
        }
    }
}