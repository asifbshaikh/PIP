using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class CorpBillingRate
    {
        public int CorpBillingRateId { get; set; }
        public int ResourceId { get; set; }
        public int LocationId { get; set; }
        public decimal? Rate { get; set; }
        public int MasterVersionId { get; set; }
    }
}
