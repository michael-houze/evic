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
using System.ComponentModel.DataAnnotations;

namespace MVC_DATABASE.Models
{
    
    public partial class RFI
    {
        public RFI()
        {
            this.RFPs = new HashSet<RFP>();
        }
    
        public int RFIID { get; set; }
        public int TEMPLATEID { get; set; }
        public string CATEGORY { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime CREATED { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime EXPIRES { get; set; }
        public List<VENDOR> vendorList { get; set; }
    
        public virtual TEMPLATE TEMPLATE { get; set; }
        public virtual ICollection<RFP> RFPs { get; set; }
    }
}
