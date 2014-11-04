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
        private EVICEntities db = new EVICEntities();
        private RFIEmployeeIndex rFIEmployeeIndex = new RFIEmployeeIndex();

        // GET: RFIs
        public ActionResult Index()
        {
            
            rFIEmployeeIndex.RFIList = new List<RFI>();
            rFIEmployeeIndex.RFIList = db.RFIs.ToList<RFI>();
            return View(rFIEmployeeIndex);
        }

       

        // GET: RFIs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rFIEmployeeIndex.RFI = await db.RFIs.FindAsync(id);
            if (rFIEmployeeIndex.RFI == null)
            {
                return HttpNotFound();
            }

            var result = from r in db.VENDORs
                         join p in db.RFIINVITEs
                     on r.Id equals p.Id
                         where p.RFIID == id
                         select r;

            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");

            return View(rFIEmployeeIndex);
        }

        // GET: RFIs/Create
        public ActionResult Create()
        {
            var template = from x in db.TEMPLATEs
                           where x.TYPE == "GHX"
                           select x;

            ViewBag.TEMPLATEID = new SelectList(template, "TEMPLATEID", "TYPE");

            var result = from r in db.OFFEREDCATEGORies
                         where r.ACCEPTED == true
                         select r.CATEGORY;
            IQueryable<string> acceptedCategories = result.Distinct();
            ViewBag.CATEGORY = acceptedCategories;
            ViewBag.AcceptedVendors = new MultiSelectList(db.VENDORs, "Id", "ORGANIZATION");

            return View(rFIEmployeeIndex);
        }

        // POST: RFIs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RFIEmployeeIndex model)
        {

            if (ModelState.IsValid)
            {
                var rfi = new RFI { TEMPLATEID = model.templateId, CATEGORY = model.RFI.CATEGORY, CREATED = DateTime.Now, EXPIRES = model.RFI.EXPIRES };
                db.RFIs.Add(rfi);
                if (model.RFIInviteList != null)
                {
                    foreach (var x in model.RFIInviteList)
                    {
                        var rfiinvite = new RFIINVITE { RFIID = rfi.RFIID, Id = x };

                        db.RFIINVITEs.Add(rfiinvite);
                    }
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE");

            var result = from r in db.OFFEREDCATEGORies
                         where r.ACCEPTED == true
                         select r.CATEGORY;


            IQueryable<string> acceptedCategories = result.Distinct();
            ViewBag.CATEGORY = acceptedCategories;
            ViewBag.AcceptedVendors = new MultiSelectList(db.VENDORs, "Id", "ORGANIZATION");
            return View(model);
        }

        public JsonResult GetAcceptedVendors(string ProductCategory)
        {

            EVICEntities dbo = new EVICEntities();
            
            var vendorProductsQuery = from v in dbo.VENDORs
                                      join c in dbo.OFFEREDCATEGORies
                                      on v.Id equals c.Id
                                      join p in dbo.PRODUCTCATEGORies
                                      on c.CATEGORY equals p.CATEGORY
                                      where c.ACCEPTED == true
                                      where c.CATEGORY.ToString() == ProductCategory
                                      select new { v };
            

            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE");
            var result = from r in db.OFFEREDCATEGORies
                         where r.ACCEPTED == true
                         select r.CATEGORY;

            IQueryable<string> acceptedCategories = result.Distinct();
            ViewBag.CATEGORY = acceptedCategories;
            
            ViewBag.AcceptedVendors = vendorProductsQuery;

            return Json(vendorProductsQuery, JsonRequestBehavior.AllowGet);
        } 

        // GET: RFIs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rFIEmployeeIndex.RFI = await db.RFIs.FindAsync(id);
            if (rFIEmployeeIndex.RFI == null)
            {
                return HttpNotFound();
            }
            rFIEmployeeIndex.EditRFIInviteList = new List<RFIINVITE>();
         

            var vendorProductsQuery = from v in db.VENDORs
                                      join c in db.OFFEREDCATEGORies
                                      on v.Id equals c.Id
                                      where c.ACCEPTED == true
                                      where c.CATEGORY == rFIEmployeeIndex.RFI.CATEGORY
                                      select  v;

                var result = from r in db.VENDORs
                             join p in db.RFIINVITEs
                         on r.Id equals p.Id
                         where p.RFIID == id
                         select r;
                vendorProductsQuery = vendorProductsQuery.Where(x => !result.Contains(x));

            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            ViewBag.SelectVendors = new MultiSelectList(vendorProductsQuery, "Id", "Organization");
            return View(rFIEmployeeIndex);
        }

        // POST: RFIs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RFIEmployeeIndex model)
        {
            if (ModelState.IsValid)
            {
                var rfi = new RFI { RFIID = model.RFI.RFIID, TEMPLATEID = model.RFI.TEMPLATEID, CATEGORY = model.RFI.CATEGORY, CREATED = model.RFI.CREATED, EXPIRES = model.RFI.EXPIRES};

                if (model.RFIInviteList != null)
                {
                    foreach (var x in model.RFIInviteList)
                    {
                        var RFIInvite = new RFIINVITE { RFIID = rfi.RFIID, Id = x, GHX_PATH = string.Empty };
                        db.RFIINVITEs.Add(RFIInvite);
                    }
                }

                await db.SaveChangesAsync();

                return RedirectToAction("Details", "RFIs", new { id = rfi.RFIID});
          
            }

            var vendorProductsQuery = from v in db.VENDORs
                                      join c in db.OFFEREDCATEGORies
                                      on v.Id equals c.Id
                                      join p in db.PRODUCTCATEGORies
                                      on c.CATEGORY equals p.CATEGORY
                                      where c.ACCEPTED == true
                                      where c.CATEGORY == rFIEmployeeIndex.RFI.CATEGORY
                                      select v;

            var result = from r in db.VENDORs
                         join p in db.RFIINVITEs
                     on r.Id equals p.Id
                         where p.RFIID == model.RFI.RFIID
                         select r;

            List<VENDOR> nonselectedvendors = new List<VENDOR>();
            nonselectedvendors = vendorProductsQuery.ToList();
            nonselectedvendors.RemoveAll(x => result.Contains(x));

            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            ViewBag.SelectVendors = new MultiSelectList(nonselectedvendors, "Id", "Organization");
            return View(model);
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

        RFIResponse responsemodel = new RFIResponse();
       
        public async Task<ActionResult> VendorResponse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            responsemodel.rfi = await db.RFIs.FindAsync(id);

            if (responsemodel.rfi == null)
            {
                return HttpNotFound();
            }

            responsemodel.inviteList = new List<RFIINVITE>();
            responsemodel.vendorlist = new List<VENDOR>();
            foreach (var x in db.RFIINVITEs.ToList())
            {

                if (x.RFIID == id)
                {
                    VENDOR vendor = await db.VENDORs.FindAsync(x.Id);
                        responsemodel.inviteList.Add(x);
                        responsemodel.vendorlist.Add(vendor);
                }
            }



            return View(responsemodel);
        }
        
        public FileResult DownloadGHX(string path)
        {
         
                //select vendors Id from RFIINVITE
                var InviteId = from x in db.RFIINVITEs
                               where x.GHX_PATH == path
                               select x.Id;
                //Get vendor items from Id
                VENDOR vendor = db.VENDORs.Find(InviteId.FirstOrDefault());
                //select RFIID
                var rfiId = from y in db.RFIINVITEs
                            where y.GHX_PATH == path
                            select y.RFIID;

                string fileName = (vendor.ORGANIZATION.ToString() + " - " + rfiId.FirstOrDefault().ToString());

                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            
            
        }

        public ActionResult VendorIndex(string id)
        {
            EVICEntities dbo = new EVICEntities();

            var user_id = User.Identity.GetUserId();

            var vendorInvitedRFIs = from i in dbo.RFIINVITEs
                                    join v in dbo.VENDORs on i.Id equals v.Id
                                    join r in dbo.RFIs on i.RFIID equals r.RFIID
                                    where i.Id == user_id
                                    orderby i.RFIID
                                    select new VendorRFI { rfi = r, rfiInvite = i, vendor = v };

            return View(vendorInvitedRFIs);
        }

        public string Respond()
        {
            return "Respond to the RFI by uploading their GHX.";
        }

        public string stringDetails()
        {
            return "Check what they submitted.";
        }


    }
}
