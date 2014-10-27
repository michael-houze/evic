using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class VendorDashboard
    {
        // The number of RFIs awaiting response from vendor
        public int pendingRFIs;

        // The number of RFPs awaiting response from vendor
        public int pendingRFPs;
    }
}