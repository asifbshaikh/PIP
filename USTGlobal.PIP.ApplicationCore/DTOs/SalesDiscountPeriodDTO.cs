using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SalesDiscountPeriodDTO
    {
        public int UId { get; set; }
        public int SalesDiscountId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? Discount { get; set; }
    }
}
