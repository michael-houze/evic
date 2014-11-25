using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqToExcel.Attributes;
using Remotion;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_DATABASE.Models
{
    public class ANALYTICS
    {
        public string Category { get; set; }

        public int RFPID { get; set; }

        public string Id { get; set; }

        [ExcelColumn("Current MMIS #")]
        public string MMIS { get; set; }

        [ExcelColumn("Current Description")]
        public string Description { get; set; }

        [ExcelColumn("New EA Price")]
        public decimal NewPriceEach { get; set; }

        [ExcelColumn("Annual Usage (by EA)")]
        public int Quantity { get; set; }  
    }
}