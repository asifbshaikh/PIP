namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectLocationDTO
    {
        public int LocationId { get; set; }
        public int PipSheetId { get; set; }
        public decimal? HoursPerDay { get; set; }
        public decimal? HoursPerMonth { get; set; }
        public bool IsOverride { get; set; }
    }
}
