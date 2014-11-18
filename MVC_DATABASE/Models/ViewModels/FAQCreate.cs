using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC_DATABASE.Models.ViewModels
{
    public class FAQCreate
    {
        
        public FAQ FAQ { get; set; }
        public ICollection<FAQ> FAQList { get; set; }


    }
}