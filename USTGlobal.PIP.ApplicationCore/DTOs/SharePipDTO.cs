using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SharePipDTO
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public int PipSheetId { get; set; }
        public int ProjectId { get; set; }
        public int VersionNumber { get; set; }
        public int SharedWithUserId { get; set; }
        public string SharedWithUserName { get; set; }
        public string SharedWithUserEmail { get; set; }
        public int SharedByUserId { get; set; }
        public string ShareComments { get; set; }
        public string SharedWithUId { get; set; }
        public string VersionName { get; set; }
    }
}
