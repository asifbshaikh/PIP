namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CorporateTargetDTO : BaseDTO
    {
        public int CorporateTargetId { get; set; }
        public decimal Percent { get; set; }
        public string Description { get; set; }
        public int MasterVersionId { get; set; }
    }
}
