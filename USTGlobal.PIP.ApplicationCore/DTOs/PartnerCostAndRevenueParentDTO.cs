using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerCostAndRevenueParentDTO
    {
        public List<ProjectMilestoneDTO> ProjectMilestoneDTO { get; set; }
        public List<PartnerCostDTO> PartnerCostDTO { get; set; }
        public List<PartnerCostPeriodDetailDTO> PartnerCostPeriodDetailDTO { get; set; }
        public List<PartnerRevenueParentDTO> PartnerRevenueDTO { get; set; }
        public List<PartnerRevenuePeriodDetailDTO> PartnerRevenuePeriodDetailDTO { get; set; }
        public IList<ProjectPeriodDTO> projectPeriodDTO { get; set; }
    }
}
