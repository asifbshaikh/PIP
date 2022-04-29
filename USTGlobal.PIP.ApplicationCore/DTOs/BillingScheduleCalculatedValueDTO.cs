namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class BillingScheduleCalculatedValueDTO
    {
        public int PipSheetId { get; set; }
        public decimal TotalCappedCost { get; set; }
        public decimal LaborRevenue { get; set; }
    }
}
