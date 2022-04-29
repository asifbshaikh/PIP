namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CurrencyDTO
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
