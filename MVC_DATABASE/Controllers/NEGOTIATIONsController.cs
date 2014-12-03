using System.Security.Principal;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System;
using System.IO;
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
    public class NEGOTIATIONsController : Controller
    {
        private EVICEntities db = new EVICEntities();
        private NegIndex negindex = new NegIndex();
        private ApplicationUserManager _userManager;

        public NEGOTIATIONsController()
        {
        }

        public NEGOTIATIONsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: NEGOTIATIONs
        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult Index()
        {
            if (db.VENDORs.Find(User.Identity.GetUserId()) != null)
            {
                return RedirectToAction("VendorIndex");
            }
            var negotiations = from x in db.NEGOTIATIONs
                               where x.CLOSED == false
                               select x;

            negindex.opennegs = negotiations.ToList<NEGOTIATION>();

            return View(negindex);
        }

        // GET: Closed NEGOTIATIONs
        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult ExpiredIndex()
        {
            if (db.VENDORs.Find(User.Identity.GetUserId()) != null)
            {
                return RedirectToAction("VendorIndex");
            }
            var negotiations = from x in db.NEGOTIATIONs
                               where x.CLOSED == true
                               select x;

            negindex.opennegs = negotiations.ToList<NEGOTIATION>();

            return View(negindex);
        }



        // GET: NEGOTIATIONs/Details/5
        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult Details(int? id)
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
            else if (negindex.negotiation.CLOSED == false)
            {
                return RedirectToAction("Edit", new { id = id });
            }

            var responses = from x in db.RESPONSEs
                            where x.NEGID == negindex.negotiation.NEGID
                            orderby x.CREATED ascending
                            select x;

            negindex.responselist = responses.ToList<RESPONSE>();

            return View(negindex);
        }

        // GET: NEGOTIATIONs/Create
        [Authorize(Roles = "Administrator,Employee")]
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

            NEGOTIATION neg = new NEGOTIATION();
            neg.CLOSED = false;

            return View(neg);
        }

        // POST: NEGOTIATIONs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Employee")]
        public ActionResult Create([Bind(Include = "NEGID,RFPID,Id,CLOSED")] NEGOTIATION nEGOTIATION)
        {
            if (ModelState.IsValid)
            {
                NEGOTIATION neg = new NEGOTIATION { RFPID = nEGOTIATION.RFPID, Id = nEGOTIATION.Id, CLOSED = false };

                
                RFP rfpmes = db.RFPs.Find(nEGOTIATION.RFPID);

                string messageBody = "You have been invited to negotiate over your proposal for " + rfpmes.CATEGORY + ". Please visit the negotiation portal to review our offer. For instructions on how to use the negotiation portal, refer to the help section and select video tutorials. From there, you can view our short tutorial video on proper utilization of the negotiation portal.";
                
                MESSAGE vendorMessage = new MESSAGE { TO = nEGOTIATION.Id, FROM = null, SUBJECT = "Invitation to Negotiate", BODY = messageBody, READ = false, SENT = DateTime.Now};
                
                string emailBody = "Baptist Health has sent you a message. Please log into the Baptist Health Supply Chain Management system for further details.";
                string messageSubject = "New Message from Baptist Health";

                UserManager.SendEmail(nEGOTIATION.Id, messageSubject, emailBody);
                db.MESSAGEs.Add(vendorMessage);
                db.NEGOTIATIONs.Add(neg);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            //If we got this far something failed, reload page

            var rfps = from x in db.RFPs
                       where x.EXPIRES <= DateTime.Now
                       select x;

            RFP rfp = db.RFPs.Find(nEGOTIATION.RFPID);

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
        [HttpGet]
        [Authorize(Roles = "Administrator,Employee,Vendor")]
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
        [Authorize(Roles = "Administrator,Employee,Vendor")]
        public ActionResult Edit(NegIndex model)
        {
            if (ModelState.IsValid)
            {
                
                if (model.file != null)
                {
                    var responses = from x in db.RESPONSEs
                                    where x.NEGID == model.negotiation.NEGID
                                    select x;

                    model.responselist = responses.ToList<RESPONSE>();

                    int count;

                    if(model.responselist == null)
                    {
                        count = 1;
                    }
                    else
                    {
                        count = model.responselist.Count + 1;
                    }

                    //Extract the file name.
                    var fileName = Path.GetFileName(model.file.FileName);                   
                    //Establishes where to save the path using the extracted name.
                    var path = Path.Combine(Server.MapPath("~/Content/Negotiation/" + model.negotiation.NEGID + "/" + count + "/"), fileName);
                    //Saves file.
                    string responsepath = "~/Content/Negotiation/" + model.negotiation.NEGID + "/" + count + "/" + fileName;
                    

                    RESPONSE newResponse = new RESPONSE { NEGID = model.negotiation.NEGID, Id = User.Identity.GetUserId(), CREATED = DateTime.Now, PATH = responsepath};
                    

                    //checks to see if file path exists, if it doesn't it creates
                    var folderpath = Server.MapPath("~/Content/Negotiation/" + model.negotiation.NEGID + "/" + count + "/");
                    if (!System.IO.Directory.Exists(folderpath))
                    {
                        System.IO.Directory.CreateDirectory(folderpath);
                    }

                    model.file.SaveAs(path);
                    db.RESPONSEs.Add(newResponse);
                    
                    

                }

                if (User.IsInRole("Administrator") || User.IsInRole("Employee"))
                {
                    if (model.negotiation.CLOSED == true)
                    {

                        NEGOTIATION neg = db.NEGOTIATIONs.Find(model.negotiation.NEGID);
                        neg.CLOSED = true;
                        
                        var ResponsePK = from x in db.RESPONSEs
                                      where x.NEGID == model.negotiation.NEGID
                                      where x.AspNetUser.VENDOR == null
                                      orderby x.PK descending
                                      select x;

                        RESPONSE conResponse = (RESPONSE)ResponsePK.FirstOrDefault();
                        if (conResponse != null)
                        {
                            TEMPLATE newContract = new TEMPLATE { TYPE = "CONTRACT", PATH = conResponse.PATH, NEGID = model.negotiation.NEGID };

                            db.TEMPLATEs.Add(newContract);

                            db.SaveChanges();
                        }
                        else
                        {
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        

                        //INTIALIZE NEG VALUES for passing to create
                        model.negotiation = db.NEGOTIATIONs.Find(model.negotiation.NEGID);
                        return RedirectToAction("NegCreate", "CONTRACTs", new { Id = model.negotiation.Id, negid = model.negotiation.NEGID, rfpid = model.negotiation.RFPID });
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
                
            }
            //if we got this far something failed, reload page
            negindex.negotiation = db.NEGOTIATIONs.Find(model.negotiation.NEGID);
            var responsecount = from x in db.RESPONSEs
                            where x.NEGID == negindex.negotiation.NEGID
                            orderby x.CREATED ascending
                            select x;

            negindex.responselist = responsecount.ToList<RESPONSE>();
            return View(model);
        }

        [Authorize(Roles = "Administrator,Employee,Vendor")]
        public FileResult DownloadResponse(string path, int id)
        {

            string ext = Path.GetExtension(path);

            string fileName = "Offer " + id + ext;

            return File(path, GetMimeType(path) , fileName);
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

        //// GET: NEGOTIATIONs/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    NEGOTIATION nEGOTIATION = db.NEGOTIATIONs.Find(id);
        //    if (nEGOTIATION == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(nEGOTIATION);
        //}

        //// POST: NEGOTIATIONs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    NEGOTIATION nEGOTIATION = db.NEGOTIATIONs.Find(id);
        //    db.NEGOTIATIONs.Remove(nEGOTIATION);
        //    db.SaveChanges();
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


        //Vendor
        [Authorize(Roles="Vendor")]
        public ActionResult VendorIndex()
        {
            var userId = User.Identity.GetUserId();

            var negotiations = from x in db.NEGOTIATIONs
                               where x.CLOSED == false
                               where x.Id == userId
                               select x;

            negindex.opennegs = negotiations.ToList<NEGOTIATION>();

            return View(negindex);
        }
    }
}
