namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CapitalChargeDTO : BaseDTO
    {
        public int PipSheetId { get; set; }
        public decimal? PaymentLag { get; set; }
        public decimal? CapitalCharge { get; set; }
        public string CapitalChargeDailyRate { get; set; }
        public decimal? TotalCostBeforeCap { get; set; }
        public bool IsTargetMarginPrice { get; set; }
    }
}
