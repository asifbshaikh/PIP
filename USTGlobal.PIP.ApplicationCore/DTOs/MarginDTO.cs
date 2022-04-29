namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class MarginDTO
    {
        public int MarginId { get; set; }
        public int PipSheetId { get; set; }
        public bool IsMarginSet { get; set; }
        public decimal? MarginPercent { get; set; }
        public int Which { get; set; }
    }
}
