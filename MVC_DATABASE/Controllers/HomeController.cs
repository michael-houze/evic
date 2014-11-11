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
                if (User.IsInRole("Administrator"))
                {
                    AdminDashboard adminModel = new AdminDashboard();

                    adminModel.pendingVendors = getPendingVendorCount();
                    adminModel.calendarEvents = getCalendarEvents();

                    return RedirectToAction("Admin", "Dashboard", adminModel);
                }
                else if (User.IsInRole("Employee"))
                {
                    EmployeeDashboard employeeModel = new EmployeeDashboard();

                    return RedirectToAction("Employee", "Dashboard", employeeModel);
                }
                else if (User.IsInRole("Vendor"))
                {
                    VendorDashboard vendorModel = new VendorDashboard();

                    return RedirectToAction("Vendor", "Dashboard", vendorModel);
                }
                else
                {
                    return View();
                }
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
                events.Add(new CalendarEvent("RFI #" + rfi.RFIID.ToString() + " Expires", true, rfi.EXPIRES, "#74C0D9"));
            }

            foreach (var rfp in db.RFPs)
            {
                events.Add(new CalendarEvent("RFP #" + rfp.RFPID.ToString() + " Expires", true, rfp.EXPIRES, "#9C8AB4"));
            }

            serializedEvents = JsonConvert.SerializeObject(events);

            return serializedEvents;
        }
    }
}