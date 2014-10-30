using MVC_DATABASE.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MVC_DATABASE.Models;


namespace MVC_DATABASE.Controllers
{
    public class VendorRFIController : Controller
    {
        // GET: VendorRFI
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Respond(string Id)
        {
            var response = new RFIINVITE { Id = Id };

            return View();
        }

        //
        //Stores the uploaded form from View VendorRFI/Respond
        [HttpPost]
        public ActionResult Respond(HttpPostedFileBase file)
        {   
            //Verify a file is selected.
            if (file != null)
            {
                //Extract the file name.
                var fileName = Path.GetFileName(file.FileName);
                //Establishes where to save the path using the extracted name.
                var path = Path.Combine(Server.MapPath(@"~/Content/RFIs/TestFolder"), fileName);
                //Saves file.
                file.SaveAs(path);
            }
            //Sends the user back to their respective RFI Index page.
            return RedirectToAction("Index");
        }


        public string Details()
        {
            return "At VendorRFI/Details()";
        }
    }
}