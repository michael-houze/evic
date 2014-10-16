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

        [Required(ErrorMessage = "You must select at least one category.")]
        [Display(Name = "Category")]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "Must be under 50 characters.")]
        public string CATEGORY { get; set; }

        [DataType(DataType.Date)]
        public System.DateTime CREATED { get; set; }

        [Required(ErrorMessage = "You must enter an expiration date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Expires")]
        public System.DateTime EXPIRES { get; set; }

        public virtual TEMPLATE TEMPLATE { get; set; }
        public virtual ICollection<RFP> RFPs { get; set; }
    }
}
