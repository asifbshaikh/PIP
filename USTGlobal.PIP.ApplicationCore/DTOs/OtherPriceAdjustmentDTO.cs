namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class OtherPriceAdjustmentDTO : BaseDTO
    {
        public int UId { get; set; }
        public int OtherPriceAdjustmentId { get; set; }
        public int PipSheetId { get; set; }
        public int? MilestoneId { get; set; }
        public string Description { get; set; }
        public decimal? TotalRevenue { get; set; }
        public bool IsDeleted { get; set; }
        public int RowType { get; set; }
    }
}
