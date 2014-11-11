using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class VendorContract
    {

        public int contractID { get; set; }
        public int rfpID { get; set; }
        public string category { get; set; }
        public string expires { get; set; }
        public string organization { get; set; }
        public string contractPath { get; set; }
        public CONTRACT contract { get; set; }
        public ICollection<CONTRACT> contractlist { get; set; }
        public ICollection<VENDOR> vendorlist { get; set; }
    }
}