using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LaborPricingDTO
    {
        public MarginDTO marginDTO { get; set; }
        public IList<ResourceLaborPricingDTO> resourceLaborPricingDTOs { get; set; }
        public IList<ProjectPeriodDTO> projectPeriodDTO { get; set; }
        public bool isDeliveryTypeRestricted { get; set; }
    }
}
