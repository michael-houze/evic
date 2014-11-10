using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFPVendorIndex
    {
        public RFP Rfp { get; set; }
        public ICollection<RFP> RfpList { get; set; }

    }
}