using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class RFIResponse
    {
        public RFI rfi { get; set; }
        public RFIINVITE rfiinvite { get; set; }
        public ICollection<RFIINVITE> inviteList { get; set; }
    }
}