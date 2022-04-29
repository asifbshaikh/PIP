using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class OtherPriceAdjustmentParentDTO : OtherPriceAdjustmentDTO
    {
        public List<OtherPriceAdjustmentPeriodDetailDTO> OtherPriceAdjustmentPeriodDetail { get; set; }
    }
}
