using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{   
   public class RiskManagementCalcDTO
    {
        public CalculatedValueDTO CalculatedValue { get; set; }
        public RiskManagementParentDTO riskManagement { get; set; }    
        public IList<ProjectPeriodDTO> projectPeriod { get; set; }
        public int? ProjectDeliveryTypeId { get; set; }
    }
}
