using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class StandardCostRateDTO
    {
        public int StandardCostRateId { get; set; }
        public int ResourceId { get; set; }
        public int LocationId { get; set; }
        public decimal? Rate { get; set; }
        public int MasterVersionId { get; set; }

    }
}
