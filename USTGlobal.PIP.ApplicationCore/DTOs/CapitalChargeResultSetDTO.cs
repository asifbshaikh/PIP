using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CapitalChargeResultSetDTO
    {
        public CapitalChargeDTO capitalChargeDTO { get; set; }
        public List<ProjectPeriodTotalDTO> projectPeriodTotalDTO { get; set; }
    }
}
