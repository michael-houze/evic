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
    public class ReportController : Controller
    {
        private EVICEntities db = new EVICEntities();
        public ReportRecord rfpRecord = new ReportRecord();

        //
        //Return a list of all RFPs. Following the link will display the results.
        [Authorize(Roles = "Administrator, Employee")]
        public ActionResult Index()
        {
            //Get collection of all RFPs
            var allRFPs = from x in db.RFPs
                          select x;

            rfpRecord.RfpList = allRFPs.ToList<RFP>();

            return View(rfpRecord);
        }

        //
        //Displays vendor responses for selected RFP on Index page.
        [Authorize(Roles = "Administrator, Employee")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Get associated RFPInvites
            rfpRecord.RfpInviteList = new List<RFPINVITE>();
            rfpRecord.VendorList = new List<VENDOR>();

            foreach (var r in db.RFPINVITEs.ToList())
            {
                if (r.RFPID == id)
                {
                    VENDOR vendor = await db.VENDORs.FindAsync(r.Id);
                    rfpRecord.RfpInviteList.Add(r);
                    rfpRecord.VendorList.Add(vendor);
                }
            }

            return View(rfpRecord);
        }


    }
}