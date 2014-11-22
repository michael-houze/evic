//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_DATABASE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class MESSAGE
    {
        public int PK { get; set; }

        [Display(Name = "To")]
        public string TO { get; set; }

        [Display(Name = "From")]
        public string FROM { get; set; }

        [Display(Name = "Subject")]
        [StringLength(50, ErrorMessage = "Subject Max Length is 50")]
        public string SUBJECT { get; set; }

        [Display(Name = "Body")]
        [StringLength(3000, ErrorMessage = "Body Max Length is 3000")]
        public string BODY { get; set; }

        public bool READ { get; set; }

        [Display(Name = "Date")]
        public System.DateTime SENT { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
    }
}