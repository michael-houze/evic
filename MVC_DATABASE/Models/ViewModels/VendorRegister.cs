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
    public class VendorRegister
    {
       public RegisterViewModel RegisterViewModel{get; set;}
       public VENDOR VENDOR { get; set; }
       public VENDORCONTACT VENDORCONTACT { get; set; }
       public ICollection<string> CategoryList { get; set; }
    }


}