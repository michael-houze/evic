using System;
using System.Collections.Generic;
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
        public ICollection<string> RFPInviteList { get; set; }
        public ICollection<string> RFPInvitedVendors { get; set; }
    }
}