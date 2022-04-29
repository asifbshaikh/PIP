using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SummaryYoyDbResultDTO
    {
        public CalculatedValueDTO CalculatedValueDTO { get; set; }
        public IList<ProjectYearTotalDTO> ProjectYearTotalDTO { get; set; }
        public RiskManagementDTO RiskManagementDTO { get; set; }
        public decimal? CapitalCharge { get; set; }
    }
}
