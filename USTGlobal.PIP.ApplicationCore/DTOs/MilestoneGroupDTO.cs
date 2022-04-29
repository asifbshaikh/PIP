namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class MilestoneGroupDTO : BaseDTO
    {
        public int MilestoneGroupId { get; set; }
        public string GroupName { get; set; }
        public int MasterVersionId { get; set; }
    }
}
