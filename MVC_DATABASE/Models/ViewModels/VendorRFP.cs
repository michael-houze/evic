using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class VendorRFP
    {
        public VENDOR vendor { get; set; }
        public RFP RFP { get; set; }
        public ICollection<RFP> RFPList { get; set; }
        public RFPINVITE RFPInvite { get; set; }

    }
}