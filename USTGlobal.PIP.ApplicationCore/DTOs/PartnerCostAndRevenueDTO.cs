using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerCostAndRevenueDTO
    {
        public List<ProjectMilestoneDTO> ProjectMilestone { get; set; }
        public List<PartnerCostParentDTO> PartnerCost { get; set; }
        public List<PartnerRevenueParentDTO> PartnerRevenue { get; set; }
        public IList<ProjectPeriodDTO> projectPeriod { get; set; }
    }
}
