using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_DATABASE.Models.ViewModels;


namespace MVC_DATABASE.Controllers
{
    public class VendorContractController : Controller
    {
        private EVICEntities db = new EVICEntities();

        //
        // GET: /VendorContract/
        public ActionResult Index()
        {
            return View();
        }



        VendorContract responsemodel = new VendorContract();

        public async Task<ActionResult> VendorResponse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            responsemodel.contract = await db.CONTRACTs.FindAsync(id);

            if (responsemodel.contract == null)
            {
                return HttpNotFound();
            }

            responsemodel.contractlist = new List<CONTRACT>();
            responsemodel.vendorlist = new List<VENDOR>();

            foreach (var x in db.CONTRACTs.ToList())
            {

                if (x.CONTRACTID == id)
                {
                    VENDOR vendor = await db.VENDORs.FindAsync(x.Id);
                    responsemodel.contractlist.Add(x);
                    responsemodel.vendorlist.Add(vendor);
                }
            }

            return View(responsemodel);
        }

        public FileResult DownloadContract(string path)
        {

            //select vendors Id from CONTRACTs
            var InviteId = from x in db.CONTRACTs
                           where x.CONTRACT_PATH == path
                           select x.Id;
            //Get vendor items from Id
            CONTRACT contract = db.CONTRACTs.Find(InviteId.FirstOrDefault());
            //select CONTRACTID
            var contractId = from y in db.CONTRACTs
                             where y.CONTRACT_PATH == path
                             select y.CONTRACTID;

            string fileName = (contract.CONTRACT_PATH.ToString() + " - " + contractId.FirstOrDefault().ToString());

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

        }

        //
        //Stores the uploaded form from View VendorContract/Response
        //[HttpPost]
        //public ActionResult Respond(HttpPostedFileBase file)
        //{
        //    //Verify a file is selected.
        //    if (file != null)
        //    {
        //        //Extract the file name.
        //        var fileName = Path.GetFileName(file.FileName);
        //        //Establishes where to save the path using the extracted name.
        //        var path = Path.Combine(Server.MapPath(@"~/Content/ContractStore/TestContracts"), fileName);
        //        //Saves file.
        //        file.SaveAs(path);
        //    }
        //    //Sends the user back to their respective RFI Index page.
        //    return RedirectToAction("Index");
        //}

        VendorContract vendorContract = new VendorContract();

        public ActionResult VendorIndex()
        {
            EVICEntities db = new EVICEntities();
            var user = User.Identity.GetUserId();
            
            vendorContract.contractlist = new List<CONTRACT>();

            var vendorContractsQuery = from c in db.CONTRACTs
                                       join v in db.VENDORs on c.Id equals v.Id
                                       join t in db.TEMPLATEs on c.TEMPLATEID equals t.TEMPLATEID
                                       where c.Id == user
                                       where t.TYPE == "CONTRACT"
                                       orderby c.CONTRACTID
                                       select c;

            vendorContract.contractlist = vendorContractsQuery.ToList();

            return View(vendorContract);
        } 

        public string Details()
        {
            return "Details...";
        }
	}
}