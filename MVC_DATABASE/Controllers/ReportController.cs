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
using LinqToExcel;
using Remotion.Data.Linq;
using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;

namespace MVC_DATABASE.Controllers
{
    public class ReportController : Controller
    {
        private EVICEntities db = new EVICEntities();
        public ReportRecord rfpRecord = new ReportRecord();

        //INDEX: GOOD
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

        public ReportDetails rfpDetails = new ReportDetails();
        
        //RFP FULL DETAILS: GOOD
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
            rfpDetails.InviteList = new List<RFPINVITE>();
            rfpDetails.VendorList = new List<VENDOR>();

            foreach (var r in db.RFPINVITEs.ToList())
            {
                if (r.RFPID == id)
                {
                    VENDOR vendor = await db.VENDORs.FindAsync(r.Id);
                    
                    rfpDetails.InviteList.Add(r);
                    rfpDetails.VendorList.Add(vendor);
                }
            }

            //Provides message if response not provided.
            ViewBag.NoResponse = "Response not received.";

            return View(rfpDetails);
        }

        //POP-UP WITH BEST RFP CHOICE FROM ALL: NEED TO ADD CONTENT AND POP-UP VIEW
        //
        //Return report of best RFP.
        [Authorize(Roles = "Administrator, Employee")]
        public string  RFPReport(string path)
        {
            return "";
        }

        //POP-UP WITH ANALYTICS FOR A SINGLE RESPONSE: NEED TO ADD DATA IN POPUP
        //
        //Returns detailed report of single vendor response.
        public string RFPResponseReport(string path)
        {
            string filePath = path;

            var excelFile = new ExcelQueryFactory(filePath);
            decimal TotalVariance = 0M;

            List<ReportLine> RFP = new List<ReportLine>();

            var lines = from l in excelFile.WorksheetRange<ReportLine>("A12", "AA24", "Financial Analysis")
                        select l;

            foreach (var l in lines)
            {
                ReportLine rfp = new ReportLine();
                rfp.Description = l.Description;
                rfp.AnnualUsage = l.AnnualUsage;
                rfp.CurrentEachPrice = l.CurrentEachPrice;
                rfp.NewEachPrice = l.NewEachPrice;
                rfp.CurrentAnnualSpend = l.CurrentAnnualSpend;
                rfp.NewAnnualSpend = l.NewAnnualSpend;
                rfp.Variance = l.Variance;

                RFP.Add(rfp);
            }

            //Used to get Total Variance for an RFP (includes all items).
            foreach (ReportLine r in RFP)
            {
                if (r.Description != null)
                    TotalVariance += r.Variance;
            }

                // ! ! !
            //With test data, need to change the actual return information.
            return "Total Variance: " + TotalVariance;
        }

        //CREATES EXCEL ANALYTICS REPORT: NEED TO ADD CONTENT TO RETURN SHEET
        //
        //Returns Analytics Report.
        [Authorize(Roles = "Administrator, Employee")]
        public ActionResult ItemReport()
        {
            //! ! ! Return Database results in excel format

            return View();
        }
    }
}