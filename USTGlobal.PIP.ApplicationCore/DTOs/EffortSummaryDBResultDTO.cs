using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class EffortSummaryDBResultDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Weeks { get; set; }
        public decimal Months { get; set; }
        public decimal TotalStaffHours { get; set; }
        public decimal FTEAvgPerMonth { get; set; }
    }
}
