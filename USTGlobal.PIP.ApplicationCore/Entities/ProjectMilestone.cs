namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ProjectMilestone : BaseEntity
    {
        public int ProjectMilestoneId { get; set; }
        public int? MilestoneId { get; set; }
        public string MilestoneName { get; set; }
        public int PipSheetId { get; set; }
        public int MilestoneGroupId { get; set; }
        public bool IsChecked { get; set; }

    }
}
