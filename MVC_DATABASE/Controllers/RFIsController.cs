using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_DATABASE.Models.ViewModels;

namespace MVC_DATABASE.Controllers
{
    public class RFIsController : Controller
    {
        private BaptistEntities db = new BaptistEntities();
        private RFIEmployeeIndex rFIEmployeeIndex = new RFIEmployeeIndex();
        // GET: RFIs
        public async Task<ActionResult> Index()
        {
            rFIEmployeeIndex.VendorResponseCount = new int();

            var rFIs = db.RFIs.Include(r => r.TEMPLATE);
            return View(await rFIs.ToListAsync());
        }

        // GET: RFIs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RFI rFI = await db.RFIs.FindAsync(id);
            if (rFI == null)
            {
                return HttpNotFound();
            }
            return View(rFI);
        }

        // GET: RFIs/Create
        public ActionResult Create()
        {
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE");
            ViewBag.CATEGORY = new SelectList(db.PRODUCTCATEGORies, "CATEGORY", "CATEGORY");
            return View();
        }

        // POST: RFIs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RFIID,TEMPLATEID,CATEGORY,CREATED,EXPIRES")] RFI rFI)
        {
            if (ModelState.IsValid)
            {
                db.RFIs.Add(rFI);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", rFI.TEMPLATEID);
            return View(rFI);
        }

        // GET: RFIs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RFI rFI = await db.RFIs.FindAsync(id);
            if (rFI == null)
            {
                return HttpNotFound();
            }
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", rFI.TEMPLATEID);
            return View(rFI);
        }

        // POST: RFIs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RFIID,TEMPLATEID,CATEGORY,CREATED,EXPIRES")] RFI rFI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rFI).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", rFI.TEMPLATEID);
            return View(rFI);
        }

        // GET: RFIs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RFI rFI = await db.RFIs.FindAsync(id);
            if (rFI == null)
            {
                return HttpNotFound();
            }
            return View(rFI);
        }

        // POST: RFIs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RFI rFI = await db.RFIs.FindAsync(id);
            db.RFIs.Remove(rFI);
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
