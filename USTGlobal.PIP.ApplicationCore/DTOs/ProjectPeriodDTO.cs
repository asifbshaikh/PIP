namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectPeriodDTO : BaseDTO
    {
        public int ProjectPeriodId { get; set; }
        public int PipSheetId { get; set; }
        public int BillingPeriodId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal? CappedCost { get; set; }
        public decimal? Inflation { get; set; }
    }
}
