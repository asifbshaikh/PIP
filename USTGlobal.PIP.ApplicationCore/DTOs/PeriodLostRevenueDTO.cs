namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PeriodLostRevenueDTO
    {
        public int ProjectPeriodId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal Revenue { get; set; }
        public decimal LostRevenue { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
