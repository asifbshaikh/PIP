using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ResourcePlanningMainDTO
    {
        public IList<ResourcePlanningDTO> Resources { get; set; }
        public IList<ProjectPeriodDTO> ProjectPeriods { get; set; }
        public IList<ResourcePlanningPipSheetDTO> ResourcePlanMasterData { get; set; }
}
}
