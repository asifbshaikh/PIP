namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class HeaderEbitdaDTO
    {
        public decimal TotalClientPrice { get; set; }
        public decimal FeesAtRisk { get; set; }
        public decimal TotalNetEstimatedRevenue { get; set; }
        public decimal TotalProjectCost { get; set; }
        public decimal EstimatedGrossProfit { get; set; }
        public decimal ProjectGPMPercent { get; set; }
        public decimal ProjectEBITDAPercent { get; set; }
        public decimal SeatCostEbitda { get; set; }
    }
}
