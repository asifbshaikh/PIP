namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class Milestone : BaseEntity
    {
        public int MilestoneId { get; set; }
        public string Name { get; set; }
        public int MilestoneGroupId { get; set; }
        public int MasterVersionId { get; set; }
    }
}
