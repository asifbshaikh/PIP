namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectBillingTypeDTO : BaseDTO
    {
        public int ProjectBillingTypeId { get; set; }
        public string BillingTypeName { get; set; }
        public int MasterVersionId { get; set; }
    }
}
