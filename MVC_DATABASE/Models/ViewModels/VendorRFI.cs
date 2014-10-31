using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class VendorRFI
    {
 
    public VENDOR vendor { get; set; }
    public RFI rfi { get; set; }
    public RFIINVITE rfiInvite { get; set; }

    }
}