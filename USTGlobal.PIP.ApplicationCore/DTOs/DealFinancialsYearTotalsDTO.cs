namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class DealFinancialsYearTotalsDTO
    {
        public int YearId { get; set; }
        public int Year { get; set; }
        public decimal ClientPrice { get; set; }
        public decimal ClientServiceFees { get; set; }
        public decimal ClientReimbursableExpense { get; set; }
        public decimal ClientPartnerFees { get; set; }
        public decimal FeeAtRisk { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal NetRevenueServiceFees { get; set; }
        public decimal NetRevenueReimbursableExp { get; set; }
        public decimal NetRevenuePartnerFees { get; set; }
        public decimal TotalProjectCost { get; set; }
        public decimal ResourceCost { get; set; }
        public decimal DirectExpenseCost { get; set; }
        public decimal PartnerCost { get; set; }
        public decimal GrossMargin { get; set; }
        public decimal SGACost { get; set; }
        public decimal Ebitda { get; set; }
        public string CorporateVerticalOverheadPercent { get; set; }
    }
}
