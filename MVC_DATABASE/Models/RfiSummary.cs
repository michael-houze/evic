using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_DATABASE.Models
{
    public class RFISummary
    {
        public int RFINumber;
        public int ResponseCount;

        public RFISummary( int id, int count)
        {
            RFINumber = id;
            ResponseCount = count;
        }
    }
}