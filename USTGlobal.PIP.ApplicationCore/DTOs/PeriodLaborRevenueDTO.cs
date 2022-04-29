using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PeriodLaborRevenueDTO
    {
        public int BillingPeriodId { get; set; }
        public int PipSheetId { get; set; }
        public decimal Revenue { get; set; }
    }
}
