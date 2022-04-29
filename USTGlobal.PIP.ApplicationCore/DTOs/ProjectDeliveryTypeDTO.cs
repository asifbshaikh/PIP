namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectDeliveryTypeDTO : BaseDTO
    {
        public int ProjectDeliveryTypeId { get; set; }
        public string DeliveryType { get; set; }
        public int MasterVersionId { get; set; }
    }
}
