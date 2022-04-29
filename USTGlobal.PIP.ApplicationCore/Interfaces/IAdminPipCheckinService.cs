using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IAdminPipCheckinService
    {
        Task<IList<AccountBasedProjectDTO>> GetAccountBasedProjects(int accountId);
        Task<IList<CheckOutPipVersionDTO>> GetCheckedOutVersions(int projectId);
        Task SaveCheckedInVersions(IList<CheckOutPipVersionDTO> checkInPipVersions, string userName);
    }
}
