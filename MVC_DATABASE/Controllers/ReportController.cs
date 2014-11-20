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

        //public ReportDetails rfpDetails = new ReportDetails();
        



        //DETAILS: GOOD
        //
        //Returns the results of all submitted RFPs, displaying line costs and total costs by vendor.
        [Authorize(Roles = "Administrator, Employee")]
        public ActionResult Details(int? id)
        {
            ReportDetails details = new ReportDetails();
            //List<RFPINVITE> rfpInvitesList = new List<RFPINVITE>();

            var rfpInvites = from i in db.RFPINVITEs
                             where i.RFPID == id
                             select i;

            rfpRecord.RfpInviteList = rfpInvites.ToList();
            //rfpInvitesList = rfpInvites.ToList();

            return View(rfpRecord);
        }


        //RFP FULL DETAILS: GOOD
        //
        //Displays vendor responses for selected RFP on Index page.
        //[Authorize(Roles = "Administrator, Employee")]
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    //Get associated RFPInvites
        //    rfpDetails.InviteList = new List<RFPINVITE>();
        //    rfpDetails.VendorList = new List<VENDOR>();

        //    foreach (var r in db.RFPINVITEs.ToList())
        //    {
        //        if (r.RFPID == id)
        //        {
        //            VENDOR vendor = await db.VENDORs.FindAsync(r.Id);
                    
        //            rfpDetails.InviteList.Add(r);
        //            rfpDetails.VendorList.Add(vendor);
        //        }
        //    }

        //        //Provides message if response not provided.
        //    ViewBag.NoResponse = "Response not received.";

        //    return View(rfpDetails);
        //}




        //POP-UP WITH BEST RFP CHOICE FROM ALL: GOOD
        //
        //Return report of best RFP.
        [Authorize(Roles = "Administrator, Employee")]
        //public string RFPReport(ICollection<RFPINVITE> InviteList)
        public string RFPReport(ReportDetails report)
        {
            string reportResult = String.Empty;

            foreach (var i in report.InviteList)
            {
                string filePath = i.OFFER_PATH;

                var excelFile = new ExcelQueryFactory(filePath);
                decimal TotalCost = 0M;

                List<ReportLine> reportLineList = new List<ReportLine>();
                var lines = from l in excelFile.WorksheetRange<ReportLine>("A12", "V138", "Financial Analysis")
                            select l;

                foreach (var l in lines)
                {
                    ReportLine rfp = new ReportLine();

                    rfp.Description = l.Description;
                    rfp.AnnualUsage = l.AnnualUsage;
                    rfp.NewEachPrice = l.NewEachPrice;
                    rfp.LineCost = l.AnnualUsage * l.NewEachPrice;

                    reportLineList.Add(rfp);
                }

                foreach (var reportLine in reportLineList)
                {
                    TotalCost += reportLine.LineCost;
                }

                reportResult += "Vendor: " + report.Vendor.ORGANIZATION + System.Environment.NewLine + 
                    "Total Cost: " + TotalCost +
                    System.Environment.NewLine + System.Environment.NewLine;                                
            }

            return reportResult;
        }




        //POP-UP WITH ANALYTICS FOR A SINGLE RESPONSE
        //
        //Returns detailed report of single vendor response.
        public string RFPResponseReport(RFPINVITE invite)
        {
            string detailString = String.Empty;
            string filePath = invite.OFFER_PATH;

            var excelFile = new ExcelQueryFactory(filePath);
            decimal TotalCost = 0M;

            List<ReportLine> RFP = new List<ReportLine>();

            var lines = from l in excelFile.WorksheetRange<ReportLine>("A12", "V138", "Financial Analysis")
                        select l;

            foreach (var l in lines)
            {
                ReportLine rfp = new ReportLine();
                rfp.Description = l.Description;
                rfp.AnnualUsage = l.AnnualUsage;
                rfp.NewEachPrice = l.NewEachPrice;
                rfp.LineCost = l.AnnualUsage * l.NewEachPrice;

                RFP.Add(rfp);

                detailString += "Product Information: " + rfp.Description + System.Environment.NewLine + 
                    "Annual Usage: " + rfp.AnnualUsage + System.Environment.NewLine +
                    "New Each Price: " + rfp.NewEachPrice + System.Environment.NewLine +
                    "Total Cost of Item: " + rfp.LineCost + System.Environment.NewLine + System.Environment.NewLine;
            }

            //Used to get Total Variance for an RFP (includes all items).
            foreach (ReportLine r in RFP)
            {
                if (r.Description != null)
                    TotalCost += r.LineCost;
            }

            return "RFP ID: " + invite.RFPID + System.Environment.NewLine +
                detailString + "Total RFP Cost: " + TotalCost +
                System.Environment.NewLine + System.Environment.NewLine;
        }




        //CREATES EXCEL ANALYTICS REPORT: NEED TO ADD CONTENT TO RETURNING EXCEL SHEET
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