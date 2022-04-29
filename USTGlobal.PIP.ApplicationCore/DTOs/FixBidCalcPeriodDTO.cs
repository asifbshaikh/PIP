using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class FixBidCalcPeriodDTO
    {
        public int CostMarginId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal Cost { get; set; }

    }
}
