using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class ReportRecord
    {
        public RFP Rfp { get; set; }
        public RFPINVITE RfpInvite { get; set; }
        public VENDOR Vendor { get; set; }

        public ICollection<RFP> RfpList { get; set; }
        public ICollection<RFPINVITE> RfpInviteList { get; set; }
        public ICollection<VENDOR> VendorList { get; set; }
    }
}