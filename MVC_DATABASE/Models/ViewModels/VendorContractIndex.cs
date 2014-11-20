using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class VendorContractIndex
    {
        [Display(Name="Contract Id")]
        public int contractID { get; set; }
        [Display(Name = "RFP Id")]
        public int rfpID { get; set; }
        [Display(Name = "Category")]
        public string category { get; set; }
        [Display(Name = "Contract")]
        public string contractPath { get; set; }
        [Display(Name = "Template Id")]
        public string templateID { get; set;}
        public CONTRACT contract { get; set; }
    }
}