using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class NegIndex
    {
        public ICollection<NEGOTIATION> opennegs { get; set; }
        public ICollection<NEGOTIATION> closednegs { get; set; }
        public ICollection<RESPONSE> responselist { get; set; }
        public NEGOTIATION negotiation { get; set; }
        public RESPONSE response { get; set; }
        public VENDOR vendor { get; set; }
        public EMPLOYEE employee { get; set; }

        public HttpPostedFileBase file { get; set; }

    }
}