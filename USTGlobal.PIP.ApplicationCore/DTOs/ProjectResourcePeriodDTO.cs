namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectResourcePeriodDTO
    {
        public int UId { get; set; }
        public int BillingPeriodId { get; set; }
        public int ProjectResourcePeriodDetailsId { get; set; }
        public decimal FTEValue { get; set; }        
        public int ProjectResourceId { get; set; }
        public decimal? Revenue { get; set; }
        public decimal TotalHours { get; set; }
        public decimal? PriceAdjustment { get; set; }
        public decimal? CostHours { get; set; }
        public decimal Inflation { get; set; }
        public decimal InflatedCostHours { get; set; }
        public decimal? CappedCost { get; set; }
        public decimal? BillRate { get; set; }
        public decimal? CostRate { get; set; }
    }
}
