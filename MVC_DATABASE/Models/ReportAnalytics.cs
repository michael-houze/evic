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
    public class ReportAnalytics
    {
        [Key, Column(Order= 0)]
        public int RFPID { get; set; }

        [Key, Column(Order= 1)]
        [ExcelColumn("Commodity Code #")]
        public int CommodityCode { get; set; }

        public string Vendor { get; set; }

        [ExcelColumn("New Vendor")]
        public string ItemVendor { get; set; }
        
        [ExcelColumn("Commodity Name")]
        public string CommodityName { get; set; }

        public string Category { get; set; }

        [ExcelColumn("Current Description")]
        public string Description { get; set; }
        
        [ExcelColumn("Current Price (EA)")]
        public decimal PreviousPriceEach { get; set; }
        
        [ExcelColumn("New EA Price")]
        public decimal NewPriceEach { get; set; }
    }
}