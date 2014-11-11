using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_DATABASE.Controllers
{
    public class DashboardController : Controller
    {
        private EVICEntities db = new EVICEntities();

        public ActionResult Admin()
        {
            AdminDashboard model = new AdminDashboard();

            model.pendingVendors = getPendingVendorCount();
            model.calendarEvents = getCalendarEvents();

            return View( model );
        }

        public ActionResult Employee()
        {
            EmployeeDashboard model = new EmployeeDashboard();

            model.rfiSummaries = getExpiredRFIs();
            model.rfpSummaries = getExpiredRFPs();
            model.calendarEvents = getCalendarEvents();

            return View( model );
        }

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

        private int getPendingVendorRFICount()
        {
            // This method will return the number of RFIs with pending status from given vendor
            // This is just test data
            int pendingRFICount = 0;

            // var pendingRFIs = db.RFIs.
            foreach( var rfi in db.RFIs.Where(model => model.EXPIRES <= DateTime.Now))
            {
                if(rfi.RFPs == null)
                {
                    pendingRFICount++;
                }
            }
            return 99;
        }

        private int getPendingVendorRFPCount()
        {
            // This method will return the number of RFPs with pending status from given vendor
            // This is just test data

            return 35;
        }

        // This method will returns a list of RFISummary objects built from expired RFIs
        private List<RFISummary> getExpiredRFIs()
        {

            List<RFISummary> summaries = new List<RFISummary>();
            int responseCount = 0;

            foreach (var rfi in db.RFIs.Where(model => model.EXPIRES <= DateTime.Now))
            {
                responseCount = 0;

                foreach (var response in rfi.RFIINVITEs)
                {
                    if(!string.IsNullOrWhiteSpace(response.GHX_PATH))
                    {
                        responseCount++;
                    }
                }

                summaries.Add(new RFISummary(rfi.RFIID, responseCount));
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
                foreach (var response in rfp.RFPINVITEs)
                {
                    if (!string.IsNullOrWhiteSpace(response.OFFER_PATH))
                    {
                        responseCount++;
                    }
                }

                summaries.Add(new RFPSummary(rfp.RFPID, responseCount));
            }

            return summaries;
        }

        private string getCalendarEvents()
        {
            string serializedEvents;
            List<CalendarEvent> events = new List<CalendarEvent>();
            CalendarEvent tempEvent = new CalendarEvent();

            foreach( var rfi in db.RFIs)
            {
                events.Add(new CalendarEvent("RFI #" + rfi.RFIID.ToString() + " Expires", true, rfi.EXPIRES, "#4DB3D0"));
            }

            foreach (var rfp in db.RFPs)
            {
                events.Add(new CalendarEvent("RFP #" + rfp.RFPID.ToString() + " Expires", true, rfp.EXPIRES, "#613E82"));
            }

            serializedEvents = JsonConvert.SerializeObject(events);

            return serializedEvents;
        }

    }
}