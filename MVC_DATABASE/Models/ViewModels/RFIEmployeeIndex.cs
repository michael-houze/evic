using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFIEmployeeIndex
    {
        EVICEntities db = new EVICEntities();

        
        public RFI RFI { get; set; }
        public int templateId { get; set; }
        public string CreateCategory { get; set; }
        public PRODUCTCATEGORY ProductCategory { get; set; }
        public ICollection<VENDOR> VendorList { get; set; }
        public ICollection<TEMPLATE> TemplateList { get; set; }
        public ICollection<RFI> OpenRFIList { get; set; }

        public ICollection<RFI> ExpiredRFIList { get; set; }

        [Required(ErrorMessage = "Please select at least one item")]
        [Display(Name= "Available Vendors")]
        public ICollection<string> RFIInviteList { get; set; }

        [Required(ErrorMessage = "Please select at least one item")]
        [Display(Name= "Invited Vendors")]
        public ICollection<string> RFIInviteVendorList { get; set; }


        public ICollection<RFIINVITE> EditRFIInviteList { get; set; }

        public ICollection<VENDOR> AcceptedVendorsList { get; set; }
        public int VendorResponseCount { get; set; }
        

    }
}