using System;

namespace USTGlobal.PIP.EmailScheduler.DTOs
{
    public class PipSheetCommentDTO
    {
        public int PIPSheetCommentId { get; set; }
        public int PIPSheetId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int AccountId { get; set; }
        public int EmailStatusId { get; set; }
        public int VersionNumber { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; }
        public string SFProjectId { get; set; }
        public string PipSheetStatus { get; set; }
        public string TemplateName { get; set; }
        public string OperationName { get; set; }
        public DateTime? CommentTimeStamp { get; set; }

    }
}
