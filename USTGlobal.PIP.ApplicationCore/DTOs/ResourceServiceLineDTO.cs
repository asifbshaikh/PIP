using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ResourceServiceLineDTO
    {
        public int ResourceServiceLineId { get; set; }
        public string ResourceServiceLineName { get; set; }
        public decimal TargetMargin { get; set; }
        public int MasterVersionId { get; set; }
    }
}
