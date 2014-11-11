using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class VendorContractIndex
    {
        public int contractID { get; set; }
        public int rfpID { get; set; }
        public string category { get; set; }
        public string contractPath { get; set; }
    }
}