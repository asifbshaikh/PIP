using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class RiskManagementPeriodDetailDTO : BaseDTO
    {
        public int RiskManagementId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? RiskAmount { get; set; }
    }
}
