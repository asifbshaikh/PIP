using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PipSheetVersionListDTO
    {
        public int ProjectId { get; set; }
        public int PipSheetId { get; set; }
        public int AccountId { get; set; }
        public int VersionNumber { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string UserComments { get; set; }
        public string ApproverComments { get; set; }
        public string ResendComments { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string ResendBy { get; set; }
        public DateTime? ResendOn { get; set; }
        public bool? IsCheckedOut { get; set; }
        public int? CheckedInOutBy { get; set; }
        public string CheckedInOutByName { get; set; }
        public int ApproverStatusId { get; set; }
        public bool HasAccountLevelAccess { get; set; }
        public string SFProjectId { get; set; }
    }
}
