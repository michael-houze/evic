using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class ContractIndex
    {
            [Display(Name = "Contract Id")]
            public int contractID { get; set; }
            [Display(Name = "RFP Id")]
            public int rfpID { get; set; }
            [Display(Name = "Category")]
            public string category { get; set; }
            [Display(Name = "Organization")]
            public string organization { get; set; }
            [Display(Name = "Contract")]
            public string contractPath { get; set; }
            [Display(Name = "Template Id")]
            public string templateID { get; set; }
            [Display(Name = "Created")]
            public DateTime CREATED { get; set; }
            [Display(Name = "Expiration Date")]
            public DateTime EXPIRES { get; set; }
        
    }
}