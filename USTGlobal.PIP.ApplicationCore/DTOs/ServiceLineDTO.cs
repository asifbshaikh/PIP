namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ServiceLineDTO : BaseDTO
    {
        public int ServiceLineId { get; set; }
        public string ServiceLineName { get; set; }
        public int ServicePortfolioId { get; set; }
        public int MasterVersionId { get; set; }
    }
}
