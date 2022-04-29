namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ServicePortfolioDTO : BaseDTO
    {
        public int ServicePortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public int MasterVersionId { get; set; }
    }
}
