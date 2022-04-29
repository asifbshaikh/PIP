using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class OtherPriceAdjustmentSubDTO
    {
        public List<ProjectMilestoneDTO> ProjectMilestone { get; set; }
        public List<OtherPriceAdjustmentDTO> OtherPriceAdjustment { get; set; }
        public List<OtherPriceAdjustmentPeriodDetailDTO> OtherPriceAdjustmentPeriodDetail { get; set; }
        public IList<ProjectPeriodDTO> ProjectPeriod { get; set; }
        public FixBidCalcDTO FeeBeforeAdjustment { get; set; }
        public IList<FixBidCalcPeriodDTO> FeeBeforeAdjustmentPeriod { get; set; }
        public bool IsMonthlyFeeAdjustment { get; set; }
    }
}
