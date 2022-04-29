namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class Resource : BaseEntity
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public int ResourceGroupId { get; set; }
        public string Grade { get; set; }
        public int MasterVersionId { get; set; }
    }
}
