namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public int CountryId { get; set; }
        public string Symbol { get; set; }
        public decimal Factors { get; set; }
        public decimal USDToLocal { get; set; }
        public decimal LocalToUSD { get; set; }
        public int MasterVersionId { get; set; }
    }
}
