namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class GrossProfitDTO
    {
        public decimal TotalClientPrice { get; set; }
        public decimal FeesAtRisk { get; set; }
        public decimal NetEstimatedRevenue { get; set; }
        public decimal TotalProjectCost { get; set; }
        public decimal EstimatedGrossProfit { get; set; }
        public decimal? ProjectGPMPercent { get; set; }
        public decimal? ProjectEBITDAPercent { get; set; }
        public decimal  TotalTargetMargin { get; set; }
        public decimal TotalRevenue { get; set; }

        public decimal? TargetEBITDAPercent { get; set; }
        public decimal? Variance { get; set; }

        public decimal TotalSeatCost { get; set; }
    }
}
