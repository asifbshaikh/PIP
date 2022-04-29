namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CalculatedValueDTO
    {
        public int PipSheetId { get; set; }
        public decimal? ResourceHours { get; set; }
        public decimal? LaborRevenue { get; set; }
        public decimal? TotalLostRevenue { get; set; }
        public decimal? StdOverheadAmount { get; set; }
        public decimal? SeatCostEbitda { get; set; }
        public decimal? TotalDirectExpense { get; set; }
        public decimal? TotalAssetCost { get; set; }
        public decimal? TotalPartnerCost { get; set; }
        public decimal? TotalPartnerRevenue { get; set; }
        public decimal? TotalReimbursement { get; set; }
        public decimal? TotalSalesDiscount { get; set; }
        public decimal? TotalOtherPriceAdjustment { get; set; }
        public decimal? TotalCappedCost { get; set; }
        public decimal? TotalClientPrice { get; set; }
        public decimal? TotalFeesAtRisk { get; set; }
        public decimal? TotalNetEstimatedRevenue { get; set; }
        public decimal? TotalCostBeforeCap { get; set; }
        public decimal? TotalProjectCost { get; set; }
        public decimal? InflatedCostHours { get; set; }
    }
}
