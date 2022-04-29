using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IResourcePlanningService
    {
        Task<List<LocationDTO>> GetProjectLocations(int pipSheetId);
        Task<List<ProjectMilestoneDTO>> GetProjectMilestones(int pipSheetId);
        Task SaveResourcePlanningData(string userName, IList<ResourcePlanningDTO> resourcePlanningDTO);
        Task<ResourcePlanningMainDTO> GetResourcePlanningData(int pipSheetId);
        Task<ResourcePlanningSaveDependencyDTO> GetResourcePlanningDataForSaveDependency(int pipSheetId);
        IList<ResourcePlanningDTO> CreateResourcePlanningObject(ResourcePlanningSaveDependencyDTO resourcePlanningSaveDependencyDTO);
        Task<LocationDependentCalculationDTO> GetLocationDependentCalculationData(int pipSheetId);
        Task SaveLocationDependentCalculations(string userName, int pipSheetId);
    }
}
