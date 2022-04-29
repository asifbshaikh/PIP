namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class ResourceGroup : BaseEntity
    {
        public int ResourceGroupId { get; set; }
        public string GroupName { get; set; }
        public int MasterVersionId { get; set; }
        public int LocationId { get; set; }
    }
}
