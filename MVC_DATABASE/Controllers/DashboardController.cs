using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_DATABASE.Controllers
{
    public class DashboardController : Controller
    {
        private BaptistEntities db = new BaptistEntities();

        public ActionResult Admin()
        {
            AdminDashboard model = new AdminDashboard();

            model.pendingVendors = getPendingVendorCount();

            return View( model );
        }

        public ActionResult Employee()
        {
            EmployeeDashboard model = new EmployeeDashboard();

            model.rfiSummaries = getExpiredRFIs();
            model.rfpSummaries = getExpiredRFPs();
            model.contractSummaries = getExpiredContracts();
            model.calendarEvents = getCalendarEvents();

            return View( model );
        }

        public ActionResult Vendor()
        {
            VendorDashboard model = new VendorDashboard();

            model.pendingRFIs = getPendingVendorRFICount();
            model.pendingRFPs = getPendingVendorRFPCount();

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

            return 58;
        }

        private int getPendingVendorRFPCount()
        {
            // This method will return the number of RFPs with pending status from given vendor
            // This is just test data

            return 35;
        }

        private List<RFISummary> getExpiredRFIs()
        {
            // This method will returne a list of RFISummary objects built from expired RFIs
            // This is just test data

            List<RFISummary> summaries = new List<RFISummary>();
            
            RFISummary sum1 = new RFISummary();
            RFISummary sum2 = new RFISummary();
            RFISummary sum3 = new RFISummary();

            sum1.RFINumber = 4276;
            sum1.ResponseCount = 3;

            sum2.RFINumber = 8311;
            sum2.ResponseCount = 5;

            sum3.RFINumber = 1661;
            sum3.ResponseCount = 1;

            summaries.Add(sum1);
            summaries.Add(sum2);
            summaries.Add(sum3);

            return summaries;
        }

        private List<RFPSummary> getExpiredRFPs()
        {
            // This method will returne a list of RFPSummary objects built from expired RFPs
            // This is just test data

            List<RFPSummary> summaries = new List<RFPSummary>();

            RFPSummary sum1 = new RFPSummary();
            RFPSummary sum2 = new RFPSummary();

            sum1.RFPNumber = 862;
            sum1.ResponseCount = 9;

            sum2.RFPNumber = 611;
            sum2.ResponseCount = 4;

            summaries.Add(sum1);
            summaries.Add(sum2);

            return summaries;
        }

        private List<ContractSummary> getExpiredContracts()
        {
            // This method will returne a list of ContractSummary objects built from expired contracts
            // This is just test data

            List<ContractSummary> summaries = new List<ContractSummary>();

            ContractSummary sum1 = new ContractSummary();
            ContractSummary sum2 = new ContractSummary();

            sum1.ContractNumber = 126;
            sum1.ResponseCount = 1;

            sum2.ContractNumber = 210;
            sum2.ResponseCount = 0;

            summaries.Add(sum1);
            summaries.Add(sum2);

            return summaries;
        }

        private string getCalendarEvents()
        {
            string calendarEvents = "";
            List<CalendarEvent> moreEvents = new List<CalendarEvent>();
            CalendarEvent[] evenMoreEvents = new CalendarEvent[2];

            CalendarEvent event1 = new CalendarEvent();
            event1.title = "RFI #1234 Expires";
            event1.allDay = true;
            event1.start = DateTime.Today;
            event1.color = "#66FF33";
            moreEvents.Add(event1);

            CalendarEvent event2 = new CalendarEvent();
            event2.title = "RFP #4321 Expires";
            event2.allDay = true;
            event2.start = DateTime.Today;
            event2.color = "CC00CC";
            moreEvents.Add(event2);

            calendarEvents = JsonConvert.SerializeObject(moreEvents);

            System.Diagnostics.Debug.WriteLine(calendarEvents);

            return calendarEvents;
        }

    }
}