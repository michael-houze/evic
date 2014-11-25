using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LinqToExcel;
using Remotion.Data.Linq;
using MVC_DATABASE.Models;
using MVC_DATABASE.Models.ViewModels;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Configuration;


namespace MVC_DATABASE.Controllers
{
    public class ReportController : Controller
    {
        private EVICEntities db = new EVICEntities();
        public ReportRecord rfpRecord = new ReportRecord();




        //INDEX: GOOD
        //
        //Return a list of all RFPs. Following the link will display the results.
        [Authorize(Roles = "Administrator, Employee")]
        public ActionResult Index()
        {
            //Get collection of all RFPs
            var allRFPs = from x in db.RFPs
                          select x;

            rfpRecord.RfpList = allRFPs.ToList<RFP>();

            return View(rfpRecord);
        }

        //public ReportDetails rfpDetails = new ReportDetails();
        



        //DETAILS: GOOD
        //
        //Returns the results of all submitted RFPs, displaying line costs and total costs by vendor.
        [Authorize(Roles = "Administrator, Employee")]
        public ActionResult Details(int? id)
        {
            ReportDetails details = new ReportDetails();
            //List<RFPINVITE> rfpInvitesList = new List<RFPINVITE>();

            var rfpInvites = from i in db.RFPINVITEs
                             where i.RFPID == id
                             select i;

            rfpRecord.RfpInviteList = rfpInvites.ToList();
            //rfpInvitesList = rfpInvites.ToList();

            return View(rfpRecord);
        }



        
        //CREATES EXCEL ANALYTICS REPORT: NEED TO ADD CONTENT TO RETURNING EXCEL SHEET
        //
        //Returns Analytics Report.
        [Authorize(Roles = "Administrator, Employee")]
        public ActionResult ItemReport()
        {
            //! ! ! Return Database results in excel format

            //string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //using (SqlConnection con = new SqlConnection(constr))
            //{
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM db.ANALYTIC"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        //cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);

                            string csv = string.Empty;

                            foreach (DataColumn column in dt.Columns)
                            {
                                csv += column.ColumnName + ',';
                            }

                            csv += "\r\n";

                            foreach (DataRow row in dt.Rows)
                            {
                                foreach (DataColumn column in dt.Columns)
                                {

                                    csv += row[column.ColumnName].ToString().Replace(",", ";") + ',';
                                }

                                csv += "\r\n";
                            }

                            Response.Clear();
                            Response.Buffer = true;
                            Response.AddHeader("content-disposition", "attachment;filename=SqlExport.csv");
                            Response.Charset = "";
                            Response.ContentType = "application/text";
                            Response.Output.Write(csv);
                            Response.Flush();
                            Response.End();
                        }

                    }

              //  }

            }




            return RedirectToAction("Index");
        }
    }
}