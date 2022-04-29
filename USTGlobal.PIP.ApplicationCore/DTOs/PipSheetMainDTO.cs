namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PipSheetMainDTO
    {
        public int PIPSheetId { get; set; }
        public int ProjectId { get; set; }
        public int VersionNumber { get; set; }
        public int? CurrencyId { get; set; }
        public int PipSheetStatusId { get; set; }
        public string Comments { get; set; }
        public bool IsCheckedOut { get; set; }
        public bool IsSubmit { get; set; }
        public bool IsApprove { get; set; }
        public bool IsResend { get; set; }
        public bool IsRevise { get; set; }
        public bool IsEdit { get; set; }
        public string ApproverComments { get; set; }
        public string ResendComments { get; set; }
    }
}
