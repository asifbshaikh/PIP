using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IResourcePlanningWorkflowService
    {
        Task ProcessResourcePlanningSave(string userName, IList<ResourcePlanningDTO> resourcePlanningDTO);
    }
}
