using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFIEmployeeIndex
    {
        BaptistEntities db = new BaptistEntities();

        
        public RFI RFI { get; set; }
        public int templateId { get; set; }
        public PRODUCTCATEGORY ProductCategory { get; set; }
        public ICollection<TEMPLATE> TemplateList { get; set; }
        public ICollection<RFI> RFIList { get; set; }
        public ICollection<string> RFIInviteList { get; set; }
        public ICollection<VENDOR> AcceptedVendorsList { get; set; }
        public int VendorResponseCount { get; set; }
        

    }
}