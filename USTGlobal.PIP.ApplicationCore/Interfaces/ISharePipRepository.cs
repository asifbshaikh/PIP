using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ISharePipRepository
    {
        Task<List<SharePipDTO>> GetSharedPipData(int projectId);

        Task<SharePipDTO> GetSharedPipData(SharePipDTO sharedPIP);

        Task DeleteSharedPipData(int pipSheetId, int roleId, int accountId, int sharedWithUserId);
        Task UpdateSharedPipData(SharePipDTO sharedPip);
        Task<bool> SaveSharedPipData(List<SharePipDTO> sharedPip, string userName);
        Task<SharePipVersionDTO> GetSharePipVersionData(int projectId);
    }
}
