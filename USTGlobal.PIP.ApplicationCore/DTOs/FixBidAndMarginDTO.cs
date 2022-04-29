using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class FixBidAndMarginDTO
    {
        public MarginDTO MarginDTO { get; set; }
        public IList<ProjectPeriodDTO> ProjectPeriodDTO { get; set; }
        public IList<FixBidCalcParentDTO> FixBidMarginDTO { get; set; }
        public decimal? MarginBeforeAdjustment { get; set; }
    }
}
