namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class OtherPriceAdjustmentPeriodDetailDTO
    {
        public int UId { get; set; }
        public int OtherPriceAdjustmentId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? Revenue { get; set; }
    }
}
