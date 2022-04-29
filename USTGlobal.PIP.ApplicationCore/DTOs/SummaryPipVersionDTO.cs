using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SummaryPipVersionDTO
    {
        public string SFProjectId { get; set; }
        public int VersionNumber { get; set; }
        public int TotalVersionsPresent { get; set; }
    }
}
