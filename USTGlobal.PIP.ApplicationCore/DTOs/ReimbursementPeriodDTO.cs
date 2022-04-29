using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReimbursementPeriodDTO
    {
        public int UId { get; set; }
        public int ReimbursementId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? Expense { get; set; }

    }
}
