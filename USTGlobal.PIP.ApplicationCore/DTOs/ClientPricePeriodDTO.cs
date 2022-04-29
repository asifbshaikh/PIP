using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ClientPricePeriodDTO
    {
        public int UId { get; set; }
        public int ClientPriceId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal? Price { get; set; }
        public int Year { get; set; }
        
    }

}
