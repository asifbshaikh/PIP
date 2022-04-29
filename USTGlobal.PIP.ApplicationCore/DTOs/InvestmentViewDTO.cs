namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class InvestmentViewDTO
    {
        public int PipSheetId { get; set; }
        public decimal TotalClientPrice { get; set; }
        public decimal TotalProjectCost { get; set; }
        public int CorporateTarget { get; set; }
    }
}
