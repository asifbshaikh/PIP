using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IProjectRepository
    {
        Task<ProjectListMainDTO> GetProjectsList(string userName);
    }
}
