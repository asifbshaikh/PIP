namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectYearTotalDTO
    {
        public int YearId { get; set; }
        public int Year { get; set; }
        public decimal CappedCost { get; set; }
        public decimal Inflation { get; set; }
        public decimal AssetSubTotalExpense { get; set; }
        public decimal PartnerCost { get; set; }
        public decimal CapitalCharge { get; set; }
        public decimal NetEstimatedRevenue { get; set; }
        public decimal EbitdaSeatCost { get; set; }
        public decimal CostContingency { get; set; }

    }
}
