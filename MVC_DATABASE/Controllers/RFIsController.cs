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
using System.IO;

namespace MVC_DATABASE.Controllers
{
    public class RFIsController : Controller
    {
        private EVICEntities db = new EVICEntities();
        private RFIEmployeeIndex rFIEmployeeIndex = new RFIEmployeeIndex();
        private ApplicationUserManager _userManager;

        public RFIsController()
        {
        }

        public RFIsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }


        // GET: RFIs
        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult Index()
        {   //Allows employee or admin to open RFIs
            var open = from x in db.RFIs
                       where x.EXPIRES > DateTime.Now
                       select x;

            rFIEmployeeIndex.OpenRFIList = new List<RFI>();
            rFIEmployeeIndex.OpenRFIList = open.ToList<RFI>();

            return View(rFIEmployeeIndex);
        }

        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult ExpiredIndex()
        {   //Allows admins and employees to see expired RFIs
            var expired = from x in db.RFIs
                          where x.EXPIRES <= DateTime.Now
                          select x;

            rFIEmployeeIndex.ExpiredRFIList = new List<RFI>();
            rFIEmployeeIndex.ExpiredRFIList = expired.ToList<RFI>();

            return View(rFIEmployeeIndex);
        }

        // GET: RFIs/Details/5

        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Details(int? id)
        {   //Tells user if request for details on RFI is valid
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
        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult Create()
        {   //create RFI if user is employee or admin
            DateTime date = DateTime.Now;
            TimeSpan time = new TimeSpan(365, 0, 0, 0);
            DateTime combined = date.Add(time);
            ViewBag.combined = combined;
            //pulls template info from DB
            var template = from x in db.TEMPLATEs
                           where x.TYPE == "GHX"
                           select x;
            
            ViewBag.TEMPLATEID = new SelectList(template, "TEMPLATEID", "TYPE");
            
            var result = from r in db.OFFEREDCATEGORies
                         where r.ACCEPTED == true
                         select r.CATEGORY;
            IQueryable<string> acceptedCategories = result.Distinct();
            ViewBag.CATEGORY = acceptedCategories;

            var vendors = from r in db.VENDORs
                          join c in db.OFFEREDCATEGORies
                          on r.Id equals c.Id
                          where c.CATEGORY.ToString() == acceptedCategories.FirstOrDefault()
                          where c.ACCEPTED == true
                          select r;

            ViewBag.AcceptedVendors = new MultiSelectList(vendors, "Id", "ORGANIZATION");

            return View(rFIEmployeeIndex);
        }

        // POST: RFIs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Create(RFIEmployeeIndex model)
        {   //creates new RFI

            if (ModelState.IsValid)
            {
                var rfi = new RFI { TEMPLATEID = model.templateId, CATEGORY = model.RFI.CATEGORY, CREATED = model.RFI.CREATED, EXPIRES = model.RFI.EXPIRES };
                db.RFIs.Add(rfi);
                if (model.RFIInviteList != null)
                {
                    foreach (var x in model.RFIInviteList)
                    {
                        
                        var rfiinvite = new RFIINVITE { RFIID = rfi.RFIID, Id = x };
                        string subject = "Invitation from Baptist Health Supply Chain Management";
                        string body = "Baptist Health SCM invites you to participate in an upcoming RFI for " + model.RFI.CATEGORY + ". Open participation for this RFI will begin on " + model.RFI.CREATED +" EST. To participate, please sign into the Baptist Health Supply Chain Management system.";
                        await UserManager.SendEmailAsync(x, subject, body);

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
        {   //pulls info on which vendors are approved to sell which category
            EVICEntities dbo = new EVICEntities();
            
            var vendorProductsQuery = from v in dbo.VENDORs
                                      join c in dbo.OFFEREDCATEGORies
                                      on v.Id equals c.Id
                                      join p in dbo.PRODUCTCATEGORies
                                      on c.CATEGORY equals p.CATEGORY
                                      where c.ACCEPTED == true
                                      where c.CATEGORY.ToString() == ProductCategory
                                      select new { v.Id, v.ORGANIZATION };
            

            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE");
            var result = from r in db.OFFEREDCATEGORies
                         where r.ACCEPTED == true
                         select r.CATEGORY;

            IQueryable<string> acceptedCategories = result.Distinct();
            ViewBag.CATEGORY = acceptedCategories;
            
            ViewBag.AcceptedVendors = vendorProductsQuery;
            //sends info to multiselect listbox.
            return Json(vendorProductsQuery, JsonRequestBehavior.AllowGet);
        } 

        // GET: RFIs/Edit/5
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Edit(int? id)
        {   //allows admin or employee to edit RFI
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
            //shows which vendors are accepted to sell this product and allow the user to select vendors
            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            ViewBag.SelectVendors = new MultiSelectList(vendorProductsQuery, "Id", "Organization");
            return View(rFIEmployeeIndex);
        }

        // POST: RFIs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> Edit(RFIEmployeeIndex model)
        {   //Email sent to vendor if they've been invited to participate in an RFI.
            if (ModelState.IsValid)
            {
                var rfi = new RFI { RFIID = model.RFI.RFIID, TEMPLATEID = model.RFI.TEMPLATEID, CATEGORY = model.RFI.CATEGORY, CREATED = model.RFI.CREATED, EXPIRES = model.RFI.EXPIRES};

                if (model.RFIInviteList != null)
                {
                    foreach (var x in model.RFIInviteList)
                    {
                        var RFIInvite = new RFIINVITE { RFIID = rfi.RFIID, Id = x, GHX_PATH = string.Empty };
                        string subject = "Invitation from Baptist Health Supply Chain Management";
                        string body = "Baptist Health SCM invites you to participate in a RFI for " + model.RFI.CATEGORY + ". Open participation for this RFI begins on " + model.RFI.CREATED + " EST. To participate, please sign into the Baptist Health Supply Chain Management system.";
                        await UserManager.SendEmailAsync(x, subject, body);
                        db.RFIINVITEs.Add(RFIInvite);
                    }
                }

                await db.SaveChangesAsync();

                return RedirectToAction("Details", "RFIs", new { id = rfi.RFIID});
          
            }
            //pulls vendor list from DB
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
            //makes sure the email only goes to the proper vendors and not all vendors
            List<VENDOR> nonselectedvendors = new List<VENDOR>();
            nonselectedvendors = vendorProductsQuery.ToList();
            nonselectedvendors.RemoveAll(x => result.Contains(x));

            ViewBag.AcceptedVendors = new MultiSelectList(result, "Id", "Organization");
            ViewBag.SelectVendors = new MultiSelectList(nonselectedvendors, "Id", "Organization");
            return View(model);
        }

        // GET: RFIs/Delete/5
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {   //allows admins and employees to delete RFIs if necessary.
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

       [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult> VendorResponse(int? id)
        {   //Allows vendor to respond to RFI.
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            responsemodel.rfi = await db.RFIs.FindAsync(id);

            if (responsemodel.rfi == null)
            {
                return HttpNotFound();
            }
           //Makes list of vendors that were invited and vendors that responded.
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
        {//download GHX
         
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

                string fileName = (vendor.ORGANIZATION.ToString() + " - " + rfiId.FirstOrDefault().ToString() + ".xlsx");

                return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            
            
        }


        //
        //Collects list of Vendor's RFIs and displays
        [Authorize(Roles = "Vendor")]
        public ActionResult VendorIndex()
        {
            //Establish DB connection.
            EVICEntities dbo = new EVICEntities();
            //Establish List of vendor's RFIs to transfer to View
            VendorRFI vendorRfi = new VendorRFI();
            //Gets user information
            var user_id = User.Identity.GetUserId();
            //List containing VendorRFIs
            vendorRfi.rfiList = new List<RFI>();

            DateTime date = DateTime.Now;

            //Query for Vendor's specifi RFIs
            var vendorInvitedRFIs = from i in dbo.RFIINVITEs
                                    join v in dbo.VENDORs on i.Id equals v.Id
                                    join r in dbo.RFIs on i.RFIID equals r.RFIID
                                    where i.Id == user_id
                                    where r.CREATED.CompareTo(date) <= 0
                                    where r.EXPIRES.CompareTo(date) > 0
                                    orderby i.RFIID
                                    select r; //new VendorRFI { rfi = r, rfiInvite = i, vendor = v };

            //Adds queried to list
            vendorRfi.rfiList = vendorInvitedRFIs.ToList();

            return View(vendorRfi);
        }

        RFIVendorRespond.RFIList respondModel = new RFIVendorRespond.RFIList();

        [Authorize(Roles = "Vendor")]
        [HttpGet]
        public async Task<ActionResult> Respond(int Id)
        {//Vendor response model allows vendors to respond to RFI's they are invited to

            responsemodel.rfi = await db.RFIs.FindAsync(Id);

            if(responsemodel.rfi.CREATED > DateTime.Now)
            {
                return RedirectToAction("VendorIndex", "RFIs");
            }

            var userID = User.Identity.GetUserId();

            var rfiinvite = from x in db.RFIINVITEs
                            where x.RFIID == Id
                            where x.Id == userID
                            select x;

            responsemodel.rfiinvite = (RFIINVITE)rfiinvite.FirstOrDefault();

            if (!string.IsNullOrEmpty(rfiinvite.FirstOrDefault().GHX_PATH))
            {//redirects vendor back to RFI page if all categories are not filled in.
                return RedirectToAction("VendorIndex", "RFIs");
            }

            return View(responsemodel);
        }

        //
        //Stores the uploaded form from View VendorRFI/Respond
        [Authorize(Roles = "Vendor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Respond(RFIResponse model)
        {
            if (ModelState.IsValid)
            {
                //Verify a file is selected.
                if (model.File != null)
                {
                    VENDOR vendor = await db.VENDORs.FindAsync(model.rfiinvite.Id);
                    RFIINVITE rfiinvite = await db.RFIINVITEs.FindAsync(model.rfiinvite.PRIMARYKEY);
                    string organization = vendor.ORGANIZATION + ".xlsx";
                    string rfiid = rfiinvite.RFIID.ToString();
                    string ghxpath = "~/Content/RFIs/" + rfiid + "/" + organization;


                    //Extract the file name.
                    var fileName = Path.GetFileName(model.File.FileName);
                    //Establishes where to save the path using the extracted name.
                    var path = Path.Combine(Server.MapPath("~/Content/RFIs/" + rfiid + "/"), organization);
                    rfiinvite.GHX_PATH = ghxpath;

                    //checks to see if file path exists, if it doesn't it creates
                    var mappath = Server.MapPath("~/Content/RFIs/" + rfiid + "/");
                    if (!System.IO.Directory.Exists(mappath))
                        System.IO.Directory.CreateDirectory(mappath);

                    //Saves file.
                    model.File.SaveAs(path);

                    //If Catalog is uploaded 
                    if (model.Catalog != null)
                    {

                        //Extract file name
                        var catalogName = Path.GetFileName(model.Catalog.FileName);

                        //Establishes where to save the path using the extracted name.
                        var thecatalogPath = Path.Combine(Server.MapPath("~/Content/RFIs/Catalogs/" + rfiid + "/" + vendor.ORGANIZATION+"/"), catalogName);
                        rfiinvite.CATALOGPATH = "~/Content/RFIs/Catalogs/" + rfiid + "/" + vendor.ORGANIZATION + "/" + catalogName;

                        //checks to see if file path exists, if it doesn't it creates
                        var catalogmappath = Server.MapPath("~/Content/RFIs/Catalogs/" + rfiid + "/" + vendor.ORGANIZATION + "/");
                        if (!System.IO.Directory.Exists(catalogmappath))
                            System.IO.Directory.CreateDirectory(catalogmappath);


                        //Saves file
                        model.Catalog.SaveAs(thecatalogPath);
                        await db.SaveChangesAsync();
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("VendorIndex", "RFIs");

                }
            }
            //Sends the user back to their respective RFI Index page.
            return View(model);

        }

        public async Task<ActionResult> ViewDetails(int Id)
        {//allows vendor to view the details of the RFI
            responsemodel.rfi = await db.RFIs.FindAsync(Id);
            if (responsemodel.rfi.CREATED > DateTime.Now)
            {
                return RedirectToAction("VendorIndex", "RFIs");
            }
            var userID = User.Identity.GetUserId();

            var rfiinvite = from x in db.RFIINVITEs
                            where x.RFIID == Id
                            where x.Id == userID
                            select x;

            responsemodel.rfiinvite = (RFIINVITE)rfiinvite.FirstOrDefault();

            return View(responsemodel);
        }

        public FileResult DownloadResponse(string path)
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


            string ext = Path.GetExtension(path);

            string fileName = (vendor.ORGANIZATION.ToString() + " - " + rfiId.FirstOrDefault().ToString() + ext);

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public FileResult DownloadCatalog(string catalogpath)
        {

            //select vendors Id from RFIINVITE
            var InviteId = from x in db.RFIINVITEs
                           where x.CATALOGPATH == catalogpath
                           select x.Id;
            //Get vendor items from Id
            VENDOR vendor = db.VENDORs.Find(InviteId.FirstOrDefault());
            //select RFIID
            var rfiId = from y in db.RFIINVITEs
                        where y.CATALOGPATH == catalogpath
                        select y.RFIID;

            string ext = Path.GetExtension(catalogpath);

            string fileName = (vendor.ORGANIZATION.ToString() + " - " + rfiId.FirstOrDefault().ToString() + ext);

            return File(catalogpath, GetMimeType(catalogpath), fileName);

        }

        public FileResult DownloadTemplate(string path)
        {//Allows vendors to download the BH templates for response

            string ext = Path.GetExtension(path);

            string fileName = "Baptist RFI Template" + ext;

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
