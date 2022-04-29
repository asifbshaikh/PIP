using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ClientPriceMainDTO
    {
        public List<ClientPriceParentDTO> ClientPriceDTO { get; set; }
        public List<ProjectPeriodDTO> ProjectPeriodDTO { get; set; }
        public bool IsFixedBid { get; set; }
        public decimal? NetEstimatedRevenue { get; set; }
        public decimal? FeesAtRisk { get; set; }
        public IList<PLForecastDTO> PLForecastDTO { get; set; }
        public int? CurrencyId { get; set; }
    }
}
