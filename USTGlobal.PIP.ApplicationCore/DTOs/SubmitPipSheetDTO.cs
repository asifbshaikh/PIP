namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SubmitPipSheetDTO
    {
        public bool IsSuccess { get; set; }
        public string ApproverName { get; set; }
        public string SFProjectId { get; set; }
        public bool IsAlreadyResend { get; set; }
        public bool IsAlreadyApproved { get; set; }
        public bool IsAlreadyRevised { get; set; }
    }
}
