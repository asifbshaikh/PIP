using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class SharePipSheet
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public int PipSheetId { get; set; }
        public int ProjectId { get; set; }
        public int SharedWithUserId { get; set; }
        public int SharedByUserId { get; set; }
        public string ShareComments { get; set; }
    }
}
