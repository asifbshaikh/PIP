using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerRevenuePeriodTotalDTO
    {
        public int ProjectPeriodId { get; set; }
        public int PipSheetId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? PartnerRevenue { get; set; }
    }
}
