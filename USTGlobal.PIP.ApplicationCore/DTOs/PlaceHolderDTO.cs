using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PlaceHolderDTO
    {
        public int? ProjectId { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public int? PIPSheetId { get; set; }
        public int? VersionNumber { get; set; }
        public int? RoleId { get; set; }
        public DateTime? CommentDateTime { get; set; }
        public string SFProjectId { get; set; }
        public string AccessName { get; set; }
        public string TemplateName { get; set; }
        public string Comment { get; set; }
        public string PIPSheetLink { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderEmailId { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string ReceiverEmailId { get; set; }
        public string OperationName { get; set; }
        public string OldAccessName { get; set; }
        public string NewAccessName { get; set; }
        public string ReceiverUId { get; set; }
        public string PipSheetStatus { get; set; }
        public int? ReceiverUserId { get; set; }
        public bool? WasAdmin { get; set; }
        public bool? WasEditor { get; set; }
        public bool? WasReviewer { get; set; }
        public bool? WasFinanceApprover { get; set; }
        public bool? WasReadOnly { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsEditor { get; set; }
        public bool? IsReviewer { get; set; }
        public bool? IsFinanceApprover { get; set; }
        public bool? IsReadOnly { get; set; }
    }
}
