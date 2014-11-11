using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models
{
    public class RFPSummary
    {
        public int RFPNumber;
        public int ResponseCount;

        public RFPSummary( int id, int responseCount)
        {
            RFPNumber = id;
            ResponseCount = responseCount;
        }
    }
}