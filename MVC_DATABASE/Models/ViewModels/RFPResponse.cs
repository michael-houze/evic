using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFPResponse
    {
        public RFP rfp { get; set; }
        public RFPINVITE rfpinvite { get; set; }
        public VENDOR vendor { get; set; }
        public ICollection<RFPINVITE> inviteList { get; set; }
        public ICollection<VENDOR> vendorlist { get; set; }
    }
}