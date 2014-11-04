using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFPCreate
    {
        public RFP rfp { get; set; }
        public int templateid { get; set; }
        public int rfiid { get; set; }
        public RFPINVITE rfpinvite { get; set; }

        [Display(Name= "Available Vendors")]
        public ICollection<string> RFPInviteList { get; set; }

        [Display(Name= "Invited Vendors")]
        public ICollection<string> RFPInvitedVendors { get; set; }
    }
}