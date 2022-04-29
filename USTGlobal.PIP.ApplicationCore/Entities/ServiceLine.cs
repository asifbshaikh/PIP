namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class ServiceLine : BaseEntity
    {
        public int ServiceLineId { get; set; }
        public string ServiceLineName { get; set; }
        public int ServicePortfolioId { get; set; }
        public int MasterVersionId { get; set; }
    }
}
