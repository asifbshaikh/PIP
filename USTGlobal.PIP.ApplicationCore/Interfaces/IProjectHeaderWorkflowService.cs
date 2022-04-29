using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IProjectHeaderWorkflowService
    {
        Task<RouteParamDTO> ProcessProjectHeaderSave(string userName, ProjectHeaderDTO projectHeader);
        Task<ProjectHeaderCurrencyDTO> GetProjectHeaderData(int projectId, int pipsheetId);

    }
}
