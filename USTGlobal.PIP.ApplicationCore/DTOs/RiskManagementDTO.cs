namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class RiskManagementDTO : BaseDTO
    {
        public int RiskManagementId { get; set; }
        public int PipSheetId { get; set; }
        public bool? IsContingencyPercent { get; set; }
        public decimal? CostContingencyRisk { get; set; }
        public decimal? FeesAtRisk { get; set; }
        public bool IsFixedBid { get; set; }
        public decimal? FixBidRiskAmount { get; set; }
        public decimal? TotalAssesedRiskOverrun { get; set; }
        public bool IsOverride { get; set; }
        public decimal? RiskCostSubTotal { get; set; }
        public int? ProjectDeliveryTypeID { get; set; }
        public bool IsMarginSet { get; set; }
        public decimal? CostContingencyPercent { get; set; }
        public bool IsOverrideUpdated { get; set; }
    }
}
