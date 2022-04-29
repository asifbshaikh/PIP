using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ResourcePlanningPipSheetDTO
    {
        public int ProjectBillingTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HolidayOption { get; set; }
    }
}
