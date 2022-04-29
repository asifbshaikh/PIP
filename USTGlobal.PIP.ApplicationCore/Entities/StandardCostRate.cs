using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class StandardCostRate
    {
        public int StandardCostRateId { get; set; }
        public int ResourceId { get; set; }
        public int LocationId { get; set; }
        public decimal? Rate { get; set; }
        public int MasterVersionId { get; set; }
    }
}
