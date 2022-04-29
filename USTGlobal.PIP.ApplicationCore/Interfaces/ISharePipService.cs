using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ISharePipService
    {
        Task<List<SharePipDTO>> GetSharedPipData(int projectId);
        Task<SharePipDTO> GetSharedPipData(SharePipDTO sharedPip);
        Task DeleteSharedPipData(int pipSheetId, int roleId, int accountId, int sharedWithUserId, string userName);
        Task UpdateSharedPipData(SharePipDTO sharedPip, string userName);
        Task<bool> SaveSharedPipData(List<SharePipDTO> sharedPip, string userName);
        Task<SharePipVersionDTO> GetSharePipVersionData(int projectId);
    }
}
