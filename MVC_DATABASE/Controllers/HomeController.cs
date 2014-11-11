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
                    return RedirectToAction("Admin", "Dashboard");
                }
                else if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("Employee", "Dashboard");
                }
                else if (User.IsInRole("Vendor"))
                {
                    return RedirectToAction("Vendor", "Dashboard");
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
    }
}