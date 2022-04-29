using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IResourcePlanningRepository
    {
        Task<List<LocationDTO>> GetProjectLocations(int pipSheetId);
        Task<List<ProjectMilestoneDTO>> GetProjectMilestones(int pipSheetId);
        Task SaveResourcePlanningData(string userName, IList<ProjectResourceDTO> projectResource, IList<ProjectResourcePeriodDTO> projectPeriods,
            IList<ProjectPeriodTotalDTO> projectPeriodTotals, decimal inflatedCostHours);
        Task<ResourcePlanningMainDTO> GetResourcePlanningData(int pipSheetId);
        Task<ResourcePlanningSaveDependencyDTO> GetResourcePlanningDataForSaveDependency(int pipSheetId);
        Task<LocationDependentCalculationDTO> GetLocationDependentCalculationData(int pipSheetId);
        Task SaveLocationDependentCalculations(string userName, int pipSheetId);
    }
}
