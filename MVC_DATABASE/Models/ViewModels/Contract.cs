using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class Contract
    {
        public string Id { get; set; }
        public int templateId { get; set; }
        public int rfpId { get; set; }
        public string CONTRACT_PATH { get; set; }
    }
}