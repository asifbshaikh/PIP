using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CheckOutPipVersionDTO
    {
        public int PipSheetId { get; set; }
        public int VersionNumber { get; set; }
        public string CheckedOutByName { get; set; }
        public string CheckedOutByUID { get; set; }
    }
}
