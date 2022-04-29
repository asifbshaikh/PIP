using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerRevenuePeriodDetailDTO
    {
        public int UId { get; set; }
        public int PartnerRevenueId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? Revenue { get; set; }
    }
}
