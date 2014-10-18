using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFIEmployeeIndex
    {
        public RFI RFI { get; set; }
        public ICollection<RFIINVITE> RFIInviteList { get; set; }
        public int VendorResponseCount { get; set; }

    }
}