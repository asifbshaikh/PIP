namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ProjectDeliveryType : BaseEntity
    {
        public int ProjectDeliveryTypeId { get; set; }
        public string DeliveryType { get; set; }
        public int MasterVersionId { get; set; }
    }
}
