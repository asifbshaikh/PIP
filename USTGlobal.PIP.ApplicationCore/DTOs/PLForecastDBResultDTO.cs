using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PLForecastDBResultDTO
    {
        public CalculatedValueDTO CalculatedValueDTO { get; set; }
        public IList<ProjectPeriodTotalDTO> ProjectPeriodTotalDTO { get; set; }
        public FixBidCalcDTO FixBidCalcDTO { get; set; }
        public IList<FixBidCalcPeriodDTO> FixBidCalcPeriodDTO { get; set; }
        public RiskManagementDTO RiskManagementDTO { get; set; }
        public List<RiskManagementPeriodDetailDTO> RiskManagementPeriodDTO { get; set; }
        public IList<ProjectPeriodDTO> ProjectPeriodDTO { get; set; }
        public decimal? CapitalCharge { get; set; }
    }
}
