using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class OtherPriceAdjustmentMainDTO
    {
        public List<ProjectMilestoneDTO> ProjectMilestone { get; set; }
        public List<OtherPriceAdjustmentParentDTO> OtherPriceAdjustmentParent { get; set; }
        public IList<ProjectPeriodDTO> ProjectPeriod { get; set; }
        public bool IsMonthlyFeeAdjustment { get; set; }
    }
}
