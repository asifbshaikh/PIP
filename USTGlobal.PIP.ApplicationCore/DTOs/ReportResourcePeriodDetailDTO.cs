using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportResourcePeriodDetailDTO
    {
        public int ProjectResourceId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal CappedCost { get; set; }
        public decimal Revenue { get; set; }
        public decimal BillRate { get; set; }
        public decimal TotalHours { get; set; }
        public decimal CostHours { get; set; }
        public decimal CostRate { get; set; }
        public decimal FTE { get; set; }
    }
}
