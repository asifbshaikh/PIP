namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ProjectBillingType : BaseEntity
    {
        public int ProjectBillingTypeId { get; set; }
        public string BillingTypeName { get; set; }
        public int MasterVersionId { get; set; }
    }
}
