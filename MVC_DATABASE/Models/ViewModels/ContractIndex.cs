using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class ContractIndex
    {
        public class ContractLabels
        {
            public CONTRACT contract { get; set; }
            public VENDOR vendor { get; set; }
            public RFP rfp { get; set; }
        }

        public class IndexType
        {
            public int contractID { get; set; }
            public int rfpID { get; set; }
            public string category { get; set; }
            public string organization { get; set; }
            public string contractPath { get; set; }
        }
    }
}