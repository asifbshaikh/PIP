using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectMainDTO> GetProjectsList(string userName);
    }
}
