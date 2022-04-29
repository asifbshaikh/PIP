using System;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ProjectControl : BaseEntity
    {
        public int PipSheetId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HolidayOption { get; set; }
        public int? MilestoneGroupId { get; set; }
    }
}
