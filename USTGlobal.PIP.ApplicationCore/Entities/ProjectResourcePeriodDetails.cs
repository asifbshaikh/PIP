namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ProjectResourcePeriodDetails : BaseEntity
    {
        public int ProjectResourcePeriodDetailsId { get; set; }
        public int ProjectResourceId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal FTEValue { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? TotalHours { get; set; }
        public decimal? PriceAdjustment { get; set; }
        public decimal? CostHours { get; set; }

    }
}
