﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFIVendorIndex
    {
        public RFI Rfi { get; set; }
        public ICollection<RFI> RfiList { get; set; }
        
    }
}