using MVC_DATABASE.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MVC_DATABASE.Controllers
{
    public class VendorContractController : Controller
    {
        //
        // GET: /VendorContract/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Respond(string Id)
        {
            var vresponse = new Contract { Id = Id };

            return View();
        }

        //
        //Stores the uploaded form from View VendorContract/Response
        [HttpPost]
        public ActionResult Respond(HttpPostedFileBase file)
        {
            //Verify a file is selected.
            if (file != null)
            {
                //Extract the file name.
                var fileName = Path.GetFileName(file.FileName);
                //Establishes where to save the path using the extracted name.
                var path = Path.Combine(Server.MapPath(@"~/Content/ContractStore/TestContracts"), fileName);
                //Saves file.
                file.SaveAs(path);
            }
            //Sends the user back to their respective RFI Index page.
            return RedirectToAction("Index");
        }

        //public ActionResult Download(int? id, string name)
        //{


        //    string fileName = name;

        //    var uploads = (from u in MVC_DATABASE.Contract
        //                   where u.Id == id
        //                   select u.CONTRACT_PATH).FirstOrDefault();


        //    if (uploads != null)
        //    {

        //        string folder = Path.GetFullPath(uploads);
        //        //HttpContext.Response.AddHeader("content-dispostion", "attachment; filename=" + fileName);
        //        return File(new FileStream(folder + "/" + fileName, FileMode.Open), "content-dispostion", fileName);
        //    }

        //    throw new ArgumentException("Invalid file name of file not exist");
        //}

        public string Details()
        {
            return "Details...";
        }
	}
}