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
    public class RFPsController : Controller
    {
        private EVICEntities db = new EVICEntities();
        public RFPCreate rfpcreate = new RFPCreate();

        // GET: RFPs
        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult Index()
        {
            var expired = from x in db.RFPs
                          where x.EXPIRES <= DateTime.Now
                          select x;
            var open = from x in db.RFPs
                       where x.EXPIRES > DateTime.Now
                       select x;

            rfpcreate.ExpiredRFPList = new List<RFP>();
            rfpcreate.ExpiredRFPList = expired.ToList<RFP>();
            rfpcreate.OpenRFPList = new List<RFP>();
            rfpcreate.OpenRFPList = open.ToList<RFP>();


            return View(rfpcreate);
        }

        // GET: RFPs/Details/5
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rfpcreate.rfp = await db.RFPs.FindAsync(id);
            if (rfpcreate.rfp == null)
            {
                return HttpNotFound();
            }
            var result = from r in db.VENDORs
                         join p in db.RFPINVITEs
                         on r.Id equals p.Id
                         where p.RFPID == id
                         select r;

            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            return View(rfpcreate);
        }

        // GET: RFPs/Create
        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult Create()
        {
            var template = from x in db.TEMPLATEs
                           where x.TYPE == "RFP"
                           select x;
            var expired = from y in db.RFIs
                          where y.EXPIRES <= DateTime.Now
                          select y;

            var vendor = from z in db.VENDORs
                         join c in db.RFIINVITEs
                         on z.Id equals c.Id
                         where c.RFIID == expired.FirstOrDefault().RFIID
                         where !string.IsNullOrEmpty(c.GHX_PATH)
                         select z;

            ViewBag.RFIID = new SelectList(expired, "RFIID", "RFIID");
            ViewBag.TEMPLATEID = new SelectList(template, "TEMPLATEID", "TYPE");
            ViewBag.AcceptedVendors = new MultiSelectList(vendor, "Id", "ORGANIZATION");
            return View();
        }

    

        // POST: RFPs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Create(RFPCreate model)
        {
            if (ModelState.IsValid)
            {
                model.rfp.RFI = await db.RFIs.FindAsync(model.rfp.RFIID);
                var rFP = new RFP { RFIID = model.rfp.RFIID, CATEGORY = model.rfp.RFI.CATEGORY, TEMPLATEID = model.templateid, CREATED = DateTime.Now, EXPIRES = model.rfp.EXPIRES};
                db.RFPs.Add(rFP);
                if (model.RFPInviteList != null)
                {
                    foreach (var x in model.RFPInviteList)
                    {
                        var rfpinvite = new RFPINVITE { RFPID = rFP.RFPID, Id = x };

                        db.RFPINVITEs.Add(rfpinvite);
                    }
                }

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        //if we got this far something failed, reload form
            var template = from x in db.TEMPLATEs
                           where x.TYPE == "RFP"
                           select x;
            var expired = from y in db.RFIs
                          where y.EXPIRES <= DateTime.Now
                          select y;
            ViewBag.RFIID = new SelectList(expired, "RFIID", "RFIID");
            ViewBag.TEMPLATEID = new SelectList(template, "TEMPLATEID", "TYPE");
            ViewBag.AcceptedVendors = new MultiSelectList(db.VENDORs, "Id", "ORGANIZATION");
            return View(model);
        }


        // GET: RFPs/Edit/5
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            rfpcreate.rfp = await db.RFPs.FindAsync(id);
            if (rfpcreate.rfp == null)
            {
                return HttpNotFound();
            }
            var vendorRFIQuery = from v in db.VENDORs
                                 join c in db.RFIINVITEs
                                 on v.Id equals c.Id
                                 where c.RFIID == (int)rfpcreate.rfp.RFIID
                                 where !string.IsNullOrEmpty(c.GHX_PATH)
                                 select v;

            var result = from r in db.VENDORs
                         join p in db.RFPINVITEs
                         on r.Id equals p.Id
                         where p.RFPID == id
                         select r;

            vendorRFIQuery = vendorRFIQuery.Where(x => !result.Contains(x));
            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            ViewBag.SelectVendors = new MultiSelectList(vendorRFIQuery, "Id", "Organization");

            return View(rfpcreate);
        }

        // POST: RFPs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Edit(RFPCreate model)
        {
            if (ModelState.IsValid)
            {
                var rFP = model.rfp;

                if (model.RFPInviteList != null)
                {
                    foreach (var x in model.RFPInviteList)
                    {
                        var RFPInvite = new RFPINVITE { RFPID = rFP.RFPID, Id = x, OFFER_PATH = string.Empty };
                        db.RFPINVITEs.Add(RFPInvite);
                    }
                }

                await db.SaveChangesAsync();

                return RedirectToAction("Details", "RFPs", new { id = rFP.RFPID});
            }
            //if we got this far something failed, reload form
            var vendorRFIQuery = from v in db.VENDORs
                                 join c in db.RFIINVITEs
                                 on v.Id equals c.Id
                                 where c.RFIID == (int)rfpcreate.rfp.RFIID
                                 select v;

            var result = from r in db.VENDORs
                         join p in db.RFPINVITEs
                         on r.Id equals p.Id
                         where p.RFPID == model.rfp.RFPID
                         select r;
            vendorRFIQuery = vendorRFIQuery.Where(x => !result.Contains(x));
            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            ViewBag.SelectVendors = new MultiSelectList(vendorRFIQuery, "Id", "Organization");

            return View(rfpcreate);
        }

        RFPResponse responsemodel = new RFPResponse();

        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> VendorResponse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            responsemodel.rfp = await db.RFPs.FindAsync(id);

            if (responsemodel.rfp == null)
            {
                return HttpNotFound();
            }

            responsemodel.inviteList = new List<RFPINVITE>();
            responsemodel.vendorlist = new List<VENDOR>();
            foreach (var x in db.RFPINVITEs.ToList())
            {

                if (x.RFPID == id)
                {
                    VENDOR vendor = await db.VENDORs.FindAsync(x.Id);
                    responsemodel.inviteList.Add(x);
                    responsemodel.vendorlist.Add(vendor);
                }
            }
            return View(responsemodel);
        }


        [Authorize(Roles = "Administrator,Employee")]
        public FileResult DownloadOffer(string path)
        {

            //select vendors Id from RFPINVITE
            var InviteId = from x in db.RFPINVITEs
                           where x.OFFER_PATH == path
                           select x.Id;
            //Get vendor items from Id
            VENDOR vendor = db.VENDORs.Find(InviteId.FirstOrDefault());
            //select RFPID
            var rfpId = from y in db.RFPINVITEs
                        where y.OFFER_PATH == path
                        select y.RFPID;

            string fileName = (vendor.ORGANIZATION.ToString() + " - " + rfpId.FirstOrDefault().ToString());

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);


        }

        //// GET: RFPs/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RFP rFP = await db.RFPs.FindAsync(id);
        //    if (rFP == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(rFP);
        //}

        //// POST: RFPs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    RFP rFP = await db.RFPs.FindAsync(id);
        //    db.RFPs.Remove(rFP);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}



        //Vendor RFPs

        RFPVendorRespond.RFPList respondmodel = new RFPVendorRespond.RFPList();

        RFPVendorIndex rfpvendorindex = new RFPVendorIndex();

        //Collects list of Vendor's RFPs and displays
        [Authorize(Roles = "Vendor")]
        public ActionResult VendorIndex()
        {
            //Establish DB connection.
            EVICEntities dbo = new EVICEntities();
            //Establish List of vendor's RFPs to transfer to View
            RFPVendorIndex vendorRfp = new RFPVendorIndex();
            //Gets user information
            var user_id = User.Identity.GetUserId();
            //List containing VendorRFPs
            vendorRfp.RFPList = new List<RFP>();

            //Query for Vendor's specifi RFPs
            var vendorInvitedRFPs = from i in dbo.RFPINVITEs
                                    join v in dbo.VENDORs on i.Id equals v.Id
                                    join r in dbo.RFPs on i.RFPID equals r.RFPID
                                    where i.Id == user_id
                                    orderby i.RFPID
                                    select r; //new VendorRFP { rfp = r, rfpInvite = i, vendor = v };

            //Adds queried to list
            vendorRfp.RFPList = vendorInvitedRFPs.ToList();

            return View(vendorRfp);
        }

        RFPVendorRespond.RFPList respondModel = new RFPVendorRespond.RFPList();

        [Authorize(Roles = "Vendor")]
        public ActionResult Respond(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            respondModel.rfp = new RFP();
            respondModel.rfp = db.RFPs.Find(id);

            if (respondModel.rfp == null)
            {
                return HttpNotFound();
            }

            respondModel.rfpInviteList = new List<RFPINVITE>();

            foreach (var x in db.RFPINVITEs.ToList())
            {
                if (x.RFPID == id)
                {
                    respondModel.rfpInviteList.Add(x);
                }
            }

            return View(respondModel);
        }

        [Authorize(Roles = "Vendor")]
        public string VendorDetails()
        {
            return "Check what they submitted.";
        }

        public string Download()
        {
            return "Download";
        }

        public string Upload()
        {
            return "Upload.";
        }

        public JsonResult GetAcceptedVendors(string RFIID)
        {
            EVICEntities dbo = new EVICEntities();


            var vendorProductsQuery = from v in dbo.VENDORs
                                      join c in dbo.RFIINVITEs
                                      on v.Id equals c.Id
                                      where c.RFIID.ToString() == RFIID
                                      where !string.IsNullOrEmpty(c.GHX_PATH)
                                      select new { v.Id, v.ORGANIZATION };

            var template = from x in db.TEMPLATEs
                           where x.TYPE == "RFP"
                           select x;
            var expired = from y in db.RFIs
                          where y.EXPIRES <= DateTime.Now
                          select y;

            ViewBag.RFIID = new SelectList(expired, "RFIID", "RFIID");
            ViewBag.TEMPLATEID = new SelectList(template, "TEMPLATEID", "TYPE");

            ViewBag.AcceptedVendors = vendorProductsQuery;

            return Json(vendorProductsQuery, JsonRequestBehavior.AllowGet);
        } 
    }
}