using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PIPSheetCommentDTO
    {
        public int PIPSheetCommentId { get; set; }

        public int ProjectId { get; set; }

        public int PIPSheetId { get; set; }

        public string comment { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime? CommentTimeStamp { get; set; }

        public bool IsDeleted { get; set; }
    }
}
