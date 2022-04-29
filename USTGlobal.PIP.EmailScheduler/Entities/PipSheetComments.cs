using System;

namespace USTGlobal.PIP.EmailScheduler.Entities
{
    public class PipSheetComments
    {
        public int PIPSheetCommentId { get; set; }

        public int PIPSheetId { get; set; }

        public string comment { get; set; }

        public int UserId { get; set; }

        public DateTime CommentTimeStamp { get; set; }

        public bool IsDeleted { get; set; }
    }
}
