namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PipSheetWorkflowStatus
    {
        public int PipSheetStatusId { get; set; }
        public string PipSheetStatusName { get; set; }
        public int? ApproverStatusId { get; set; }
        public string ApproverStatusName { get; set; }
        public bool? IsCheckedOut { get; set; }
        public int? CheckedInOutBy { get; set; }
    }
}
