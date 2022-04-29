namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class BillingScheduleProjectResourceDTO
    {
        public int ProjectResourceId { get; set; }
        public int PipSheetId { get; set; }
        public decimal TotalHoursPerResource { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
