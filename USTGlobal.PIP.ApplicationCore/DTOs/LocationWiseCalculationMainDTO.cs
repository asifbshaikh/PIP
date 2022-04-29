
using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LocationWiseCalculationMainDTO
    {
        public List<LocationWiseDetailDTO> LocationWiseDetailDTO { get; set; }
        public List<ProjectSummaryLocationDTO> projectSummaryLocationDTO { get; set; }
    }
}
