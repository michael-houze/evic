using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class UploadViewModel
    {
        [Required(ErrorMessage = "Please select a file")]
        public HttpPostedFileBase File { get; set; }
    }
}