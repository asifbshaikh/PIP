namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectMilestoneDTO
    {
        public int ProjectMilestoneId { get; set; }
        public int PipSheetId { get; set; }
        public int? MilestoneId { get; set; }
        public string MilestoneName { get; set; }
        public bool IsChecked { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public string MilestoneMonth { get; set; }
        public int MilestoneGroupId { get; set; }
    }
}
