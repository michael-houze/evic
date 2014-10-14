using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;

namespace MVC_DATABASE.Models
{
    public class RFICreate
    {
        public RFI RFI { get; set; }
        public List<VENDOR> VendorList { get; set; }

    }
}