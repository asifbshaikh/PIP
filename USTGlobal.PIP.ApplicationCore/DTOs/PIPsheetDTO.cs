using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PipSheetDTO : BaseDTO
    {
        public int PIPSheetId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HolidayOption { get; set; }
        public int? MilestoneGroupId { get; set; }
        public int ProjectId { get; set; }
        public int VersionNumber { get; set; }
    }
}
