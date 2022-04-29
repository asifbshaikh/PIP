namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ResourceGroupDTO : BaseDTO
    {
        public int ResourceGroupId { get; set; }
        public string GroupName { get; set; }
        public int MasterVersionId { get; set; }
        public int LocationId { get; set; }
    }
}
