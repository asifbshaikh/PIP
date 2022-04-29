using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LocationDependentCalculationDTO
    {
        public IList<CalculatedValueDTO> CalculatedValue { get; set; }
        public IList<ProjectPeriodTotalDTO> ProjectPeriodList { get; set; }
        public IList<ClientPricePeriodDTO> ClientPricePeriodList { get; set; }
        public IList<ClientPriceDTO> ClientPriceDTO { get; set; }
        public IList<RiskManagementDTO> RiskManagementDTO { get; set; }
    }
}
