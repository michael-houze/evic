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
            model.contractSummaries = getExpiredContracts();
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
            string serializedEvents;
            List<CalendarEvent> events = new List<CalendarEvent>();
            CalendarEvent tempEvent = new CalendarEvent();

            foreach( var rfi in db.RFIs)
            {
                events.Add(new CalendarEvent("RFI #" + rfi.RFIID.ToString() + " Expires", true, rfi.EXPIRES, "#4DB3D0"));
            }

            foreach (var rfp in db.RFPs)
            {
                events.Add(new CalendarEvent("RFP #" + rfp.RFPID.ToString() + " Expires", true, rfp.EXPIRES, "#4A176D"));
            }

            serializedEvents = JsonConvert.SerializeObject(events);

            return serializedEvents;
        }

    }
}