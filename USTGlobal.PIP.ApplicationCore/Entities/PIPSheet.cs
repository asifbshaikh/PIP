namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class PipSheet : BaseEntity
    {
        public int PipSheetId { get; set; }
        public int ProjectId { get; set; }
        public int VersionNumber { get; set; }
        public int? CurrencyId { get; set; }
        public int? PipSheetStatusId { get; set; }
        public string Comments { get; set; }
        public string ApproverComments { get; set; }
        public int? SubmittedBy { get; set; }
        public ProjectHeader ProjectHeader { get; set; }
        public bool? IsCheckedOut { get; set; }
        public int? CheckedInOutBy { get; set; }
        public int? ApproverStatusId { get; set; }
        public int? ApprovedBy { get; set; }
        public bool isDummy { get; set; }
        public bool IsActive { get; set; }
    }
}
