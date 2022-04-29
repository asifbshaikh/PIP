using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportPLForecastPeriodDTO
    {
        public int PLForecastId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal Amount { get; set; }
    }
}
