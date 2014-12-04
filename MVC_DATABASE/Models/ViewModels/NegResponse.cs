using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class NegResponse
    {
        public ICollection<RESPONSE> responselist { get; set; }
        public NEGOTIATION negotiation { get; set; }
        public RESPONSE response { get; set; }

        public HttpPostedFileBase file { get; set; }
    }
}