using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class ReportRFP
    {
        public int rfpId { get; set; }
        public int rfiId { get; set; }
        public string category { get; set; }
        public int templateId { get; set; }
        public DateTime created { get; set; }
        public DateTime expires { get; set; }
    }
}