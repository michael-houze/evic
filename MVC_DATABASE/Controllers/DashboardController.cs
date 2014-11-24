using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_DATABASE.Controllers
{
    public class DashboardController : Controller
    {
        private EVICEntities db = new EVICEntities();

        [Authorize(Roles = "Administrator")]
        public ActionResult Admin()
        {
            AdminDashboard model = new AdminDashboard();

            model.pendingVendors = getPendingVendorCount();
            model.calendarEvents = getEmployeeCalendarEvents();
            model.contractSummaries = getExpiredContracts();

            return View( model );
        }

        [Authorize(Roles = "Administrator, Employee")]
        public ActionResult Employee()
        {
            EmployeeDashboard model = new EmployeeDashboard();

            model.rfiSummaries = getExpiredRFIs();
            model.rfpSummaries = getExpiredRFPs();
            model.contractSummaries = getExpiredContracts();
            model.calendarEvents = getEmployeeCalendarEvents();

            return View( model );
        }

        [Authorize(Roles = "Vendor")]
        public ActionResult Vendor()
        {
            VendorDashboard model = new VendorDashboard();

            model.pendingRFIs = getPendingVendorRFICount();
            model.pendingRFPs = getPendingVendorRFPCount();
            model.calendarEvents = getCalendarEvents();

            return View( model );
        }

        private int getPendingVendorCount()
        {
            var pendingVendors = db.VENDORs.Where(model => model.VENDSTATUS == "PENDING").Count();

            return pendingVendors;
        }

        // This method will return the number of RFIs with pending status from given vendor
        private int getPendingVendorRFICount()
        {
            int responseCount = 0;

            var currentUser = User.Identity.Name;       
            string currentUserName = currentUser;

            foreach(var rfi in db.RFIs)
            {
                foreach(var response in rfi.RFIINVITEs)
                {
                    if ((currentUserName == response.AspNetUser.UserName) &&
                        (string.IsNullOrWhiteSpace(response.GHX_PATH)) && 
                        rfi.CREATED <= DateTime.Now &&
                        rfi.EXPIRES > DateTime.Now)
                    {
                        responseCount++;
                    }
                }
            }

            return responseCount;
        }

        // This method will return the number of RFPs with pending status from given vendor
        private int getPendingVendorRFPCount()
        {
            int responseCount = 0;

            var currentUser = User.Identity.Name;
            string currentUserName = currentUser;

            foreach (var rfp in db.RFPs)
            {
                foreach (var response in rfp.RFPINVITEs)
                {
                    if ((currentUserName == response.AspNetUser.UserName) &&
                        (string.IsNullOrWhiteSpace(response.OFFER_PATH)) &&
                        rfp.CREATED <= DateTime.Now &&
                        rfp.EXPIRES > DateTime.Now)
                    {
                        responseCount++;
                    }
                }
            }

            return responseCount;
        }

        // This method will returns a list of RFISummary objects built from expired RFIs
        private List<RFISummary> getExpiredRFIs()
        {

            List<RFISummary> summaries = new List<RFISummary>();
            int responseCount = 0;

            foreach (var rfi in db.RFIs.Where(model => model.EXPIRES <= DateTime.Now))
            {
                if (rfi.EXPIRES >= DateTime.Now.AddDays(-8))
                {
                    responseCount = 0;

                    foreach (var response in rfi.RFIINVITEs)
                    {
                        if (!string.IsNullOrWhiteSpace(response.GHX_PATH))
                        {
                            responseCount++;
                        }
                    }

                    summaries.Add(new RFISummary(rfi.RFIID, responseCount));
                }
            }

            return summaries;
        }

        // This method will return a list of RFPSummary objects built from expired RFPs
        private List<RFPSummary> getExpiredRFPs()
        {

            List<RFPSummary> summaries = new List<RFPSummary>();
            int responseCount = 0;

            foreach (var rfp in db.RFPs.Where(model => model.EXPIRES <= DateTime.Now))
            {
                if (rfp.EXPIRES >= DateTime.Now.AddDays(-8))
                {
                    responseCount = 0;
                    foreach (var response in rfp.RFPINVITEs)
                    {
                        if (!string.IsNullOrWhiteSpace(response.OFFER_PATH))
                        {
                            responseCount++;
                        }
                    }

                    summaries.Add(new RFPSummary(rfp.RFPID, responseCount));
                }
            }
            return summaries;
        }

        private List<ContractSummary> getExpiredContracts()
        {
            List<ContractSummary> summaries = new List<ContractSummary>();
            

            foreach (var con in db.CONTRACTs.Where(model => model.EXPIRES <= DateTime.Now))
            {
                if (con.EXPIRES >= DateTime.Now.AddDays(-8))
                {
                    summaries.Add(new ContractSummary(con.RFPID));
                }
            }
            return summaries;
        }

        private string getCalendarEvents()
        {
            string serializedEvents;
            List<CalendarEvent> events = new List<CalendarEvent>();

            foreach( var rfi in db.RFIs)
            {
                events.Add(new CalendarEvent("RFI " + rfi.CATEGORY.ToString().Substring(4) + " Expires", true, rfi.EXPIRES, "#4DB3D0"));
            }

            foreach (var rfp in db.RFPs)
            {
                events.Add(new CalendarEvent("RFP " + rfp.CATEGORY.ToString().Substring(4) + " Expires", true, rfp.EXPIRES, "#613E82"));
            }

            serializedEvents = JsonConvert.SerializeObject(events);

            return serializedEvents;
        }

        private string getEmployeeCalendarEvents()
        {
            string serializedEvents;
            List<CalendarEvent> events = new List<CalendarEvent>();

            foreach( var rfi in db.RFIs)
            {
                string url = Url.Action("Details", "RFIs", new { id = rfi.RFIID.ToString() });
                events.Add(new CalendarEvent("RFI " + rfi.CATEGORY.ToString().Substring(4) + " Expires", true, rfi.EXPIRES, "#4DB3D0", url));
            }

            foreach (var rfp in db.RFPs)
            {
                string url = Url.Action("Details", "RFPs", new { id = rfp.RFPID.ToString() });
                events.Add(new CalendarEvent("RFP " + rfp.CATEGORY.ToString().Substring(4) + " Expires", true, rfp.EXPIRES, "#613E82", url));
            }

            serializedEvents = JsonConvert.SerializeObject(events);

            return serializedEvents;
        }

    }
}