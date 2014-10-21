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
        public ActionResult Index()
        {
            rFIEmployeeIndex.RFIInviteList = new List<string>();
            rFIEmployeeIndex.VendorResponseCount = 0;
            rFIEmployeeIndex.TemplateList = new List<TEMPLATE>();
            
            foreach(var item in db.TEMPLATEs)
            {
                if (item.TYPE == "GHX")
                {
                    rFIEmployeeIndex.TemplateList.Add(item);
                }
            }

            ViewBag.Category = new SelectList(db.PRODUCTCATEGORies, "CATEGORY", "CATEGORY");
            ViewBag.Templates = new SelectList(rFIEmployeeIndex.TemplateList, "TEMPLATEID", "TEMPLATEID");
            return View(rFIEmployeeIndex);
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
            ViewBag.AcceptedVendors = new MultiSelectList(db.VENDORs, "Id", "ORGANIZATION");
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
            ViewBag.CATEGORY = new SelectList(db.PRODUCTCATEGORies, "CATEGORY", "CATEGORY");
            return View(rFI);
        }

        [HttpGet]
        public ActionResult GetAcceptedVendors()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAcceptedVendors(SelectListItem ProductCategory)
        {

            BaptistEntities dbo = new BaptistEntities();
            
            var vendorProductsQuery = from v in dbo.VENDORs
                                      join c in dbo.OFFEREDCATEGORies
                                      on v.Id equals c.Id
                                      join p in dbo.PRODUCTCATEGORies
                                      on c.CATEGORY equals p.CATEGORY
                                      where c.ACCEPTED == true
                                      where c.CATEGORY.ToString() == ProductCategory.ToString()
                                      select new { v };
            ICollection<VENDOR> AcceptedVendors = new List<VENDOR>();
            AcceptedVendors = (List<VENDOR>)vendorProductsQuery;

            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE");
            ViewBag.CATEGORY = new SelectList(db.PRODUCTCATEGORies, "CATEGORY", "CATEGORY");
            ViewBag.AcceptedVendors = new MultiSelectList(AcceptedVendors, "Id", "ORGANIZATION");

            return View();
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
