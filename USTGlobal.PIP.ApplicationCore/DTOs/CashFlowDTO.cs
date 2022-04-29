using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CashFlowDTO
    {
        public int ClientPriceId { get; set; }
        public decimal? SumOfYearPrice { get; set; }
        public int Year { get; set; }
        public List<ClientPricePeriodDTO> ClientPricePeriodDTO { get; set; }
    }
}
