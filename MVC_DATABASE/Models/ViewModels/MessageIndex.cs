using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class MessageIndex
    {
        public MESSAGE message { get; set; }
        public ICollection<MESSAGE> messagelist { get; set; }

    }
}