using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ResourcePlanningSaveDependencyDTO
    {
        public IList<ResourcePlanningDTO> Resources { get; set; }
        public IList<ProjectPeriodDTO> ProjectPeriods { get; set; }
        public IList<ResourcePlanningPipSheetDTO> ResourcePlanMasterData { get; set; }
        public IList<LocationDTO> ResourceLocationDTO { get; set; }
        public IList<HolidayDTO> ResourceHolidayDTO { get; set; }
        public IList<ProjectBillingTypeDTO> ProjectBillingTypeDTO { get; set; }
    }
}
