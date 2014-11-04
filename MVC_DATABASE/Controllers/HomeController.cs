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
    public class HomeController : Controller
    {
        private EVICEntities db = new EVICEntities();

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                AdminDashboard model = new AdminDashboard();

                model.pendingVendors = getPendingVendorCount();
                model.calendarEvents = getCalendarEvents();

                return View(model);
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private int getPendingVendorCount()
        {
            var pendingVendors = db.VENDORs.Where(model => model.VENDSTATUS == "PENDING").Count();

            return pendingVendors;
        }

        private string getCalendarEvents()
        {
            string serializedEvents;
            List<CalendarEvent> events = new List<CalendarEvent>();
            CalendarEvent tempEvent = new CalendarEvent();

            foreach (var rfi in db.RFIs)
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