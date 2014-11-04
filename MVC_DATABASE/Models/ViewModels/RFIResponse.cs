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
using System.IO;
using System.Web.Hosting;

namespace MVC_DATABASE.Models.ViewModels
{

    public class RFIResponse
    {

            public RFI rfi { get; set; }            
            public RFIINVITE rfiinvite { get; set; }
            public VENDOR vendor { get; set; }
            public ICollection<RFIINVITE> inviteList { get; set; }
            public ICollection<VENDOR> vendorlist { get; set; }
        

    }
}
 
