namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class DeliveryBilling : BaseEntity
    {
        public int ProjectDeliveryTypeId { get; set; }
        public int ProjectBillingTypeId { get; set; }
        public int MasterVersionId { get; set; }
    }
}
