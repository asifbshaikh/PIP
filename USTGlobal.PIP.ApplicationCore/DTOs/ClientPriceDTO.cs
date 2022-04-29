namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ClientPriceDTO : BaseDTO
    {
        public int UId { get; set; }
        public int ClientPriceId { get; set; }
        public int PipSheetId { get; set; }
        public int? DescriptionId { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool IsOverrideUpdated { get; set; }
    }
}
