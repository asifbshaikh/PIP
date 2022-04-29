namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LocationWiseDetailDTO
    {
        public int LocationId { get; set; }
        public decimal ResourceRevenue { get; set; }
        public decimal ResourceCost { get; set; }
        public decimal TotalCostHrs { get; set; }
        public decimal TotalBilledHrs { get; set; }
        public decimal TotalFTE { get; set; }
        public decimal NonBillableFTE { get; set; }
        public decimal NonBillableCost { get; set; }
    }
}
