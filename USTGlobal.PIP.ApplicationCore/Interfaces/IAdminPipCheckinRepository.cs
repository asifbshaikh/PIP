using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IAdminPipCheckinRepository
    {
        Task<IList<AccountBasedProjectDTO>> GetAccountBasedProjects(int accountId);
        Task<IList<CheckOutPipVersionDTO>> GetCheckedOutVersions(int projectId);
        Task SaveCheckedInVersions(IList<CheckOutPipVersionDTO> checkInPipVersions, string userName);
    }
}
