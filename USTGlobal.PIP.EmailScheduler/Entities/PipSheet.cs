namespace USTGlobal.PIP.EmailScheduler.Entities
{
    public class PipSheet
    {
        public int PipSheetId { get; set; }
        public int ProjectId { get; set; }
        public int? PipSheetStatusId { get; set; }
        public string ApproverComments { get; set; }
        public int? SubmittedBy { get; set; }
        public bool? IsCheckedOut { get; set; }
        public int? CheckedInOutBy { get; set; }
        public int? ApproverStatusId { get; set; }
        public bool IsActive { get; set; }
    }
}
