using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class MonthlyData
    {
        public int WorkingDays { get; set; }
        public int TotalDays { get; set; }
        public int ActualWorkingDays { get; set; }
        public List<LocationHoliday> LocationHolidays { get; set; }
    }
}
