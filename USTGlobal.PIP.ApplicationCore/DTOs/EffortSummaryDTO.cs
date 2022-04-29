using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class EffortSummaryDTO
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal Weeks { get; set; }
        public decimal Months { get; set; }
        public decimal TotalStaffHours { get; set; }
        public decimal FTEAvgPerMonth { get; set; }
    }
}
