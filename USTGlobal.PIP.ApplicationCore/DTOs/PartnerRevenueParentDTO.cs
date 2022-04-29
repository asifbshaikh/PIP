using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerRevenueParentDTO : PartnerRevenueDTO
    {
        public IList<PartnerRevenuePeriodDetailDTO> PartnerRevenuePeriodDetail { get; set; }
    }
}
