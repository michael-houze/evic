using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class VendorContract
    {

        public VENDOR vendor { get; set; }
        public CONTRACT contract { get; set; }
        public ICollection<CONTRACT> contractlist { get; set; }
        public ICollection<VENDOR> vendorlist { get; set; }
    }
}