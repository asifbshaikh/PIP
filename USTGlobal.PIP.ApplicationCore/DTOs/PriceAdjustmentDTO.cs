using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PriceAdjustmentDTO
    {
        public ProjectDurationDTO ProjectDurationDTO { get; set; }
        public PriceAdjustmentYoyDTO PriceAdjustmentYoyDTO { get; set; }
        public List<ColaDTO> ColaDTO { get; set; }
    }
}
