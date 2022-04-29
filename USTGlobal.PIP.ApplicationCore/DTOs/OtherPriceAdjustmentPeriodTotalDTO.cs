namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class OtherPriceAdjustmentPeriodTotalDTO
    {
        public int ProjectPeriodId { get; set; }
        public int PipSheetId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? AdjustedRevenue { get; set; }
        public decimal? PriceAdjustmentEntry { get; set; }
        public decimal? FeeAfterAdjustment { get; set; }
    }
}
