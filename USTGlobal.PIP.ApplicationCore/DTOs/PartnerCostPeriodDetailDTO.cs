namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerCostPeriodDetailDTO 
    {
        public int UId { get; set; }
        public int PartnerCostId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? Cost { get; set; }
    }
}
