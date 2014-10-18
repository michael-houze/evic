using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_DATABASE.Models;

namespace MVC_DATABASE.Controllers
{
    public class QueryController : Controller
    {
        // GET: Query
        public IQueryable vendorProductsQuery()
        {
            BaptistEntities dbo = new BaptistEntities();
            var vendorProductsQuery = from v in dbo.VENDORs
                                      join c in dbo.OFFEREDCATEGORies
                                      on v.Id equals c.Id
                                      join p in dbo.PRODUCTCATEGORies
                                      on c.CATEGORY equals p.CATEGORY
                                      where c.ACCEPTED == true
                                      select new { v.ORGANIZATION };

            return vendorProductsQuery;
        }

        public IQueryable vendorRFIInviteQuery()
        {
            BaptistEntities dbo = new BaptistEntities();
            var vendorRFIInviteQuery = from v in dbo.VENDORs
                                       join i in dbo.RFIINVITEs
                                       on v.Id equals i.Id
                                       join r in dbo.RFIs
                                       on i.RFIID equals r.RFIID
                                       where i.GHX_PATH != null
                                       select new { v.ORGANIZATION };
            return vendorRFIInviteQuery;
        }
    }


}

