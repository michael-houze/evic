using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models.ViewModels
{
    public class EmployeeDashboard
    {
        // An array of RFIs with an expired status
        public List<RFISummary> rfiSummaries;

        // An array of RFPs with an expired status
        public List<RFPSummary> rfpSummaries;

        // An array of Contracts with an expired status
        public List<ContractSummary> contractSummaries;

        public string calendarEvents;
    }
}