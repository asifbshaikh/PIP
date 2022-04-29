using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class DirectExpensePeriodDTO
    {
        public int UId { get; set; }
        public int DirectExpensePeriodDetailId { get; set; }
        public int DirectExpenseId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? Expense { get; set; }
    }
}
