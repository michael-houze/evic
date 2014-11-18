using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqToExcel.Attributes;
using Remotion;

namespace MVC_DATABASE.Models.ViewModels
{
    public class ReportLine
    {
        [ExcelColumn("Current Description")]
        public string Description { get; set; }
        
        [ExcelColumn("New EA Price")]
        public decimal NewEachPrice { get; set; }
        
        [ExcelColumn("Annual Usage (by EA)")]
        public decimal AnnualUsage { get; set; }

        //Total Cost of Item (cost * quantity used)
        public decimal LineCost { get; set; }
    }
}