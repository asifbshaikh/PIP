using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IProjectControlWorkflowService
    {
        Task ProcessProjectControlSave(string userName, ProjectControlDTO projectControlDTO);
    }
}
