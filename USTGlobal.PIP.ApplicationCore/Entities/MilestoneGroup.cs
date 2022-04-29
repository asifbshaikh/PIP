namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class MilestoneGroup : BaseEntity
    {
        public int MilestoneGroupId { get; set; }
        public string GroupName { get; set; }
        public int MasterVersionId { get; set; }
    }
}
