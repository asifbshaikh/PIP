namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class ServicePortfolio : BaseEntity
    {
        public int ServicePortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public int MasterVersionId { get; set; }
    }
}
