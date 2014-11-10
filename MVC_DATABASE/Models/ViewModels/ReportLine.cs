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
        [ExcelColumn("Commodity Name")]
        public string Description { get; set; }
        [ExcelColumn("Current Price (EA)")]
        public decimal CurrentEachPrice { get; set; }
        [ExcelColumn("New EA Price")]
        public decimal NewEachPrice { get; set; }
        [ExcelColumn("Annual Usage (by EA)")]
        public decimal AnnualUsage { get; set; }
        [ExcelColumn("Annual Spend Current Product")]
        public decimal CurrentAnnualSpend { get; set; }
        [ExcelColumn("Annual Spend New Product")]
        public decimal NewAnnualSpend { get; set; }
        [ExcelColumn("Variance (Current - New)")]
        public decimal Variance { get; set; }

    }
}