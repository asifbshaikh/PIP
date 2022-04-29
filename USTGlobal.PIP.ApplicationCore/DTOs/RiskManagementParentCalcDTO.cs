using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class RiskManagementParentCalcDTO
    {
        public CalculatedValueDTO CalculatedValue { get; set; }
        public RiskManagementDTO RiskManagement { get; set; }
        public List<RiskManagementPeriodDetailDTO> RiskManagementPeriodDetail { get; set; }
        public IList<ProjectPeriodDTO> projectPeriod { get; set; }
        public int? ProjectDeliveryTypeId { get; set; }
    }
}
