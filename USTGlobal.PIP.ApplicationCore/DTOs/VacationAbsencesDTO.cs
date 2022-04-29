namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class VacationAbsencesDTO : BaseDTO
    {
        public int PIPSheetId { get; set; }
        public decimal TotalRevenue { get; set; }
        public bool IsPercent { get; set; }
        public bool IsMarginSet { get; set; }
        public bool IsOverride { get; set; }
        public decimal? Amount { get; set; }
        public decimal? LostRevenue { get; set; }
        public bool IsOverrideUpdated { get; set; }
    }
}
