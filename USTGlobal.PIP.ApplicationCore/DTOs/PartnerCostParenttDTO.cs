using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerCostParentDTO : PartnerCostDTO
    {
        public List<PartnerCostPeriodDetailDTO> PartnerCostPeriodDetail { get; set; }
    }
}
