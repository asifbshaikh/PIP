namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class EbitdaDTO
    {
        public int PipSheetId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public decimal RefUSD { get; set; }
        public decimal EbitdaSeatCost { get; set; }
        public decimal? OverheadAmount { get; set; }
        public decimal? SharedSeatsUsePercent { get; set; }
        public bool IsStdOverheadOverriden { get; set; }
    }
}
