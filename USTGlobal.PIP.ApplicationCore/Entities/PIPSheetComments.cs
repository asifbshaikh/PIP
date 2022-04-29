using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class PIPSheetComments
    {
        public int PIPSheetCommentId { get; set; }

        public int PIPSheetId { get; set; }

        public string comment { get; set; }

        public int UserId { get; set; }

        public DateTime CommentTimeStamp { get; set; }

        public bool IsDeleted { get; set; }
    }
}
