using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class AccountAuthService : IAccountAuthService
    {
        private readonly IAccountAuthRepository accountAuthRepository;
        public AccountAuthService(IAccountAuthRepository accountAuthRepository)
        {
            this.accountAuthRepository = accountAuthRepository;
        }

        public async Task<List<RoleAndAccountDTO>> GetUserAccountLevelRoles(string userName)
        {
            return await this.accountAuthRepository.GetUserAccountLevelRoles(userName);
        }

        public async Task<List<RoleAccountProjectDTO>> GetUserAccountProjectRoles(string userName)
        {
            return await this.accountAuthRepository.GetUserAccountProjectRoles(userName);
        }

        public async Task<bool> GetUserAuthStatusForSubmit(string userName, PipSheetMainDTO pipSheetMain)
        {
            return await this.accountAuthRepository.GetUserAuthStatusForSubmit(userName, pipSheetMain);
        }
        public async Task<bool> GetProjectListAuthCheck(string userName)
        {
            return await this.accountAuthRepository.GetProjectListAuthCheck(userName);
        }

        public async Task<bool> GetPipSheetLevelAuthorization(string userName, int pipsheetId, bool isGetOrPostRequest)
        {
            return await this.accountAuthRepository.GetPipSheetLevelAuthorization(userName, pipsheetId, isGetOrPostRequest);
        }

        public async Task<bool> GetPipSheetLevelAuth(string userName, int pipSheetId)
        {
            return await this.accountAuthRepository.GetPipSheetLevelAuth(userName, pipSheetId);
        }

        public async Task<bool> GetEditorLevelCheck(string userName, int pipSheetId)
        {
            return await this.accountAuthRepository.GetEditorLevelCheck(userName, pipSheetId);
        }

        public async Task<bool> GetAccountLevelEditorLevelCheck(string userName, int pipSheetId)
        {
            return await this.accountAuthRepository.GetAccountLevelEditorLevelCheck(userName, pipSheetId);
        }

        public async Task<bool> GetCheckIfAuth(string userName, int pipSheetId)
        {
            return await this.accountAuthRepository.GetCheckIfAuth(userName, pipSheetId);
        }

        public async Task<bool> GetAccountLevelEditorCheck(string userName, int accountId)
        {
            return await this.accountAuthRepository.GetAccountLevelEditorCheck(userName, accountId);
        }

        public async Task<bool> GetPipVersionAuth(string userName, int projectId)
        {
            return await this.accountAuthRepository.GetPipVersionAuth(userName, projectId);
        }

        public async Task<bool> GetHeader1DataAuth(string userName, int projectId, int pipSheetId)
        {
            return await this.accountAuthRepository.GetHeader1DataAuth(userName, projectId, pipSheetId);
        }

        public async Task<bool> GetPipSheetCommentAuth(string userName, int pipSheetId)
        {
            return await this.accountAuthRepository.GetPipSheetCommentAuth(userName, pipSheetId);
        }

        public async Task<bool> GetDeletePipCommentAuth(string userName, int pipSheetCommentId)
        {
            return await this.accountAuthRepository.GetDeletePipCommentAuth(userName, pipSheetCommentId);
        }

        public async Task<bool> GetAccountEditorCheckForDeletePip(string userName, int pipSheetId)
        {
            return await this.accountAuthRepository.GetAccountEditorCheckForDeletePip(userName, pipSheetId);
        }
    }
}
