using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ApproverDTO
    {
        public int ProjectId { get; set; }
        public string SFProjectId { get; set; }
        public string ProjectName { get; set; }
        public int PipSheetId { get; set; }
        public int VersionNumber { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string SubmittedBy { get; set; }
        public string SubmittedOn { get; set; }
        public string Comments { get; set; }
        public string ApproverComments { get; set; }
        public string ResendComments { get; set; }
        public string PipSheetStatus { get; set; }
        public DateTime ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ResendOn { get; set; }
        public string ResendBy { get; set; }
        public string ResendOnString { get; set; }
        public string ApprovedOnString { get; set; }
    }
}
