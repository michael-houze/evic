using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class AccountManagement
    {
        public AspNetUser AspNetUser { get; set; }
        public VENDOR Vendor { get; set; }
        public VENDORCONTACT VendorContact { get; set; }
        public EMPLOYEE Employee { get; set; }
        public OFFEREDCATEGORY OfferedCategory { get; set; }
        public ICollection<string> CategoryList { get; set; }
        public ICollection<VENDOR> VendorList { get; set; }
        public ICollection<EMPLOYEE> EmployeeList { get; set; }
        public IList<OFFEREDCATEGORY> OfferedCategoryList { get; set; }

    }
}