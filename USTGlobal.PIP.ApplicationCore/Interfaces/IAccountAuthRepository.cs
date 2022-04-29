using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IAccountAuthRepository
    {
        Task<List<RoleAndAccountDTO>> GetUserAccountLevelRoles(string userName);
        Task<List<RoleAccountProjectDTO>> GetUserAccountProjectRoles(string userName);
        Task<bool> GetUserAuthStatusForSubmit(string userName, PipSheetMainDTO pipSheetMain);
        Task<bool> GetProjectListAuthCheck(string userName);
        Task<bool> GetPipSheetLevelAuthorization(string userName, int pipsheetId, bool isGetOrPostRequest);
        Task<bool> GetPipSheetLevelAuth(string userName, int pipSheetId);
        Task<bool> GetEditorLevelCheck(string userName, int pipSheetId);
        Task<bool> GetAccountLevelEditorLevelCheck(string userName, int pipSheetId);
        Task<bool> GetCheckIfAuth(string userName, int pipSheetId);
        Task<bool> GetAccountLevelEditorCheck(string userName, int accountId);
        Task<bool> GetPipVersionAuth(string userName, int projectId);
        Task<bool> GetHeader1DataAuth(string userName, int projectId, int pipSheetId);
        Task<bool> GetPipSheetCommentAuth(string userName, int pipSheetId);
        Task<bool> GetDeletePipCommentAuth(string userName, int pipSheetCommentId);
        Task<bool> GetAccountEditorCheckForDeletePip(string userName, int pipSheetId);
    }
}
