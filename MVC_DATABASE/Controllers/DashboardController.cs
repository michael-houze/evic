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
        // GET: Dashboard
        public ActionResult Admin()
        {
            AdminDashboard model = new AdminDashboard();

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

            return View( model );
        }
    }
}