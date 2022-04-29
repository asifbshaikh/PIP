using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ClientPriceDBResultDTO
    {
        public FixBidCalcDTO FixBidCalcDTO { get; set; }
        public List<FixBidCalcPeriodDTO> FixBidCalcPeriodDTO { get; set; }
        public CalculatedValueDTO CalculatedValueDTO { get; set; }
        public List<ProjectPeriodTotalDTO> ProjectPeriodTotalDTO { get; set; }
        public List<ClientPriceDTO> ClientPriceDTO { get; set; }
        public List<ClientPricePeriodDTO> ClientPricePeriodDTO { get; set; }
        public List<ProjectPeriodDTO> ProjectPeriodDTO { get; set; }
        public bool? IsFixedBid { get; set; }
        public decimal? FeesAtRisk { get; set; }
        public RiskManagementDTO RiskManagementDTO { get; set; }
        public List<RiskManagementPeriodDetailDTO> RiskManagementPeriodDTO { get; set; }
        public decimal? CapitalCharge { get; set; }
        public int?  CurrencyId { get; set; }
    }
}
