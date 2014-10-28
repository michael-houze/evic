using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;
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

        // GET: Dashboard
        public ActionResult Admin()
        {
            AdminDashboard model = new AdminDashboard();

            model.pendingVendors = getPendingVendorCount();

            return View( model );
        }

        public ActionResult Employee()
        {
            EmployeeDashboard model = new EmployeeDashboard();

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

            return 58;
        }

        private int getPendingVendorRFPCount()
        {
            // This method will return the number of RFPs with pending status from given vendor

            return 35;
        }

    }
}