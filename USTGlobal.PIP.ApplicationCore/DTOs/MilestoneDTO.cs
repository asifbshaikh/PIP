namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class MilestoneDTO : BaseDTO
    {
        public int MilestoneId { get; set; }
        public string MilestoneName { get; set; }
        public int MilestoneGroupId { get; set; }
        public string MilestoneGroupName { get; set; }
        public int MasterVersionId { get; set; }
    }
}
