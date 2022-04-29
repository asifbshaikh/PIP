using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LocationWiseSummaryDTO
    {
        public List<LocationWiseDetailsDTO> LocationWiseDetails { get; set; }
        public List<ProjectSummaryLocationDTO> SummaryLocations { get; set; }
    }
}
