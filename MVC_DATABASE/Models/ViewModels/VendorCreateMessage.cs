using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class VendorCreateMessage
    {
        [Required]
        [Display(Name = "Subject")]
        public string SUBJECT { get; set; }

        [Required]
        [Display(Name = "Body")]
        public string BODY { get; set; }
    }
}