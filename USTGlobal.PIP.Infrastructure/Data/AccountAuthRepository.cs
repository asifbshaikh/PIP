using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class AccountAuthRepository : IAccountAuthRepository
    {
        private readonly PipContext pipContext;

        public AccountAuthRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<RoleAndAccountDTO>> GetUserAccountLevelRoles(string userName)
        {
            List<RoleAndAccountDTO> roleAndAccountDTOs = new List<RoleAndAccountDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetUserAccountLevelRoles")
            .WithSqlParam("@UserName", userName)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                roleAndAccountDTOs = resultSet.ReadToList<RoleAndAccountDTO>().ToList();
            });

            return roleAndAccountDTOs;
        }

        public async Task<List<RoleAccountProjectDTO>> GetUserAccountProjectRoles(string userName)
        {
            return await (from u in this.pipContext.User
                          join ur in this.pipContext.UserRole on u.UserId equals ur.UserId
                          join r in this.pipContext.Role on ur.RoleId equals r.RoleId
                          join a in this.pipContext.Account on ur.AccountId equals a.AccountId
                          join p in this.pipContext.Project on a.AccountId equals p.AccountId
                          join ps in this.pipContext.PipSheet on p.ProjectId equals ps.ProjectId
                          where u.Email == userName && a.AccountId > 0
                          select new RoleAccountProjectDTO()
                          {
                              RoleId = r.RoleId,
                              RoleName = r.RoleName,
                              AccountId = ur.AccountId,
                              AccountName = a.AccountName,
                              ProjectId = p.ProjectId,
                              PipSheetId = ps.PipSheetId
                          }).Distinct().ToListAsync();
        }

        public async Task<bool> GetUserAuthStatusForSubmit(string userName, PipSheetMainDTO pipSheetMain)
        {
            bool status = false;
            if (pipSheetMain.IsSubmit)
            {
                status = await (from u in this.pipContext.User
                                join ur in this.pipContext.UserRole on u.UserId equals ur.UserId
                                join r in this.pipContext.Role on ur.RoleId equals r.RoleId
                                join a in this.pipContext.Account on ur.AccountId equals a.AccountId
                                join p in this.pipContext.Project on a.AccountId equals p.AccountId
                                join ps in this.pipContext.PipSheet on p.ProjectId equals ps.ProjectId
                                where u.Email == userName && ps.PipSheetId == pipSheetMain.PIPSheetId && r.RoleId == 3 && ps.isDummy == false
                                select u).AnyAsync();
            }
            else if (pipSheetMain.IsApprove || pipSheetMain.IsRevise)
            {
                status = await (from u in this.pipContext.User
                                join ur in this.pipContext.UserRole on u.UserId equals ur.UserId
                                join r in this.pipContext.Role on ur.RoleId equals r.RoleId
                                join a in this.pipContext.Account on ur.AccountId equals a.AccountId
                                join p in this.pipContext.Project on a.AccountId equals p.AccountId
                                join ps in this.pipContext.PipSheet on p.ProjectId equals ps.ProjectId
                                where u.Email == userName && ps.PipSheetId == pipSheetMain.PIPSheetId && r.RoleId == 2 && ps.isDummy == false
                                select u).AnyAsync();
            }
            else if (pipSheetMain.IsResend)
            {
                status = await (from u in this.pipContext.User
                                join ur in this.pipContext.UserRole on u.UserId equals ur.UserId
                                join r in this.pipContext.Role on ur.RoleId equals r.RoleId
                                join a in this.pipContext.Account on ur.AccountId equals a.AccountId
                                join p in this.pipContext.Project on a.AccountId equals p.AccountId
                                join ps in this.pipContext.PipSheet on p.ProjectId equals ps.ProjectId
                                where u.Email == userName && ps.PipSheetId == pipSheetMain.PIPSheetId && (r.RoleId == 2 || r.RoleId == 4) && ps.isDummy == false
                                select u).AnyAsync();
            }
            return status;
        }
        public async Task<bool> GetProjectListAuthCheck(string userName)
        {
            bool authStatus = false;
            await pipContext.LoadStoredProc("dbo.sp_GetProjectListAuthCheck")
            .WithSqlParam("@UserName", userName)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                authStatus = Convert.ToBoolean(resultSet.ReadToValue<bool>());
            });
            return authStatus;
        }

        public async Task<bool> GetPipSheetLevelAuthorization(string userName, int pipsheetId, bool isGetOrPostRequest)
        {
            bool? isPipSheetAccessAuthorized = false;
            List<RoleAndAccountDTO> roleAndAccountDTOs = new List<RoleAndAccountDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetPipSheetLevelAuthorization")
            .WithSqlParam("@UserName", userName)
            .WithSqlParam("@PipSheetId", pipsheetId)
            .WithSqlParam("@IsGetOrPostRequest", isGetOrPostRequest)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                isPipSheetAccessAuthorized = resultSet.ReadToValue<bool>();
            });

            return isPipSheetAccessAuthorized ?? false;
        }

        public async Task<bool> GetPipSheetLevelAuth(string userName, int pipSheetId)
        {
            bool authStatus = false;
            List<RoleAndAccountDTO> roleAndAccountDTOs = new List<RoleAndAccountDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetPipSheetLevelAuth")
            .WithSqlParam("@UserName", userName)
            .WithSqlParam("@PipSheetId", pipSheetId)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                authStatus = Convert.ToBoolean(resultSet.ReadToValue<bool>());
            });
            return authStatus;
        }

        public async Task<bool> GetEditorLevelCheck(string userName, int pipSheetId)
        {
            bool authStatus = false;
            List<RoleAndAccountDTO> roleAndAccountDTOs = new List<RoleAndAccountDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetEditorLevelCheck")
            .WithSqlParam("@UserName", userName)
            .WithSqlParam("@PipSheetId", pipSheetId)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                authStatus = Convert.ToBoolean(resultSet.ReadToValue<bool>());
            });
            return authStatus;
        }

        public async Task<bool> GetAccountLevelEditorLevelCheck(string userName, int pipSheetId)
        {
            return await (from u in this.pipContext.User
                          join ur in this.pipContext.UserRole on u.UserId equals ur.UserId
                          join r in this.pipContext.Role on ur.RoleId equals r.RoleId
                          join a in this.pipContext.Account on ur.AccountId equals a.AccountId
                          join p in this.pipContext.Project on a.AccountId equals p.AccountId
                          join ps in this.pipContext.PipSheet on p.ProjectId equals ps.ProjectId
                          where u.Email == userName && ps.PipSheetId == pipSheetId && r.RoleId == 3 && ps.isDummy == false
                          select u).AnyAsync();
        }

        public async Task<bool> GetAccountEditorCheckForDeletePip(string userName, int pipSheetId)
        {
            return await (from u in this.pipContext.User
                          join ur in this.pipContext.UserRole on u.UserId equals ur.UserId
                          join r in this.pipContext.Role on ur.RoleId equals r.RoleId
                          join a in this.pipContext.Account on ur.AccountId equals a.AccountId
                          join p in this.pipContext.Project on a.AccountId equals p.AccountId
                          join ps in this.pipContext.PipSheet on p.ProjectId equals ps.ProjectId
                          where u.Email == userName && ps.PipSheetId == pipSheetId && r.RoleId == 3
                          && (ps.IsCheckedOut == true || (ps.IsCheckedOut == false && ps.CheckedInOutBy == u.UserId))
                          select u).AnyAsync();
        }

        public async Task<bool> GetCheckIfAuth(string userName, int pipSheetId)
        {
            return await (from u in this.pipContext.User
                          join ur in this.pipContext.UserRole on u.UserId equals ur.UserId
                          join r in this.pipContext.Role on ur.RoleId equals r.RoleId
                          join a in this.pipContext.Account on ur.AccountId equals a.AccountId
                          join p in this.pipContext.Project on a.AccountId equals p.AccountId
                          join ps in this.pipContext.PipSheet on p.ProjectId equals ps.ProjectId
                          where u.Email == userName && ps.PipSheetId == pipSheetId && (r.RoleId == 2 || r.RoleId == 4) && ps.isDummy == false
                          select u).AnyAsync();
        }

        public async Task<bool> GetAccountLevelEditorCheck(string userName, int accountId)
        {
            return await (from u in this.pipContext.User
                          join ur in this.pipContext.UserRole on u.UserId equals ur.UserId
                          join r in this.pipContext.Role on ur.RoleId equals r.RoleId
                          join a in this.pipContext.Account on ur.AccountId equals a.AccountId
                          where u.Email == userName && a.AccountId == accountId && r.RoleId == 3
                          select u).AnyAsync();
        }

        public async Task<bool> GetPipVersionAuth(string userName, int projectId)
        {
            bool authStatus = false;
            await pipContext.LoadStoredProc("dbo.sp_GetPipVersionAuth")
            .WithSqlParam("@UserName", userName)
            .WithSqlParam("@ProjectId", projectId)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                authStatus = Convert.ToBoolean(resultSet.ReadToValue<bool>());
            });
            return authStatus;
        }

        public async Task<bool> GetHeader1DataAuth(string userName, int projectId, int pipSheetId)
        {
            bool authStatus = false;
            await pipContext.LoadStoredProc("dbo.sp_GetHeader1DataAuth")
            .WithSqlParam("@UserName", userName)
            .WithSqlParam("@ProjectId", projectId)
            .WithSqlParam("@PipSheetId", pipSheetId)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                authStatus = Convert.ToBoolean(resultSet.ReadToValue<bool>());
            });
            return authStatus;
        }

        public async Task<bool> GetPipSheetCommentAuth(string userName, int pipSheetId)
        {
            bool authStatus = false;
            await pipContext.LoadStoredProc("dbo.sp_GetPipSheetCommentAuth")
            .WithSqlParam("@UserName", userName)
            .WithSqlParam("@PipSheetId", pipSheetId)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                authStatus = Convert.ToBoolean(resultSet.ReadToValue<bool>());
            });
            return authStatus;
        }

        public async Task<bool> GetDeletePipCommentAuth(string userName, int pipSheetCommentId)
        {
            bool authStatus = false;
            await pipContext.LoadStoredProc("dbo.sp_GetDeletePipCommentAuth")
            .WithSqlParam("@UserName", userName)
            .WithSqlParam("@PipSheetCommentId", pipSheetCommentId)
            .ExecuteStoredProcAsync((resultSet) =>
            {
                authStatus = Convert.ToBoolean(resultSet.ReadToValue<bool>());
            });
            return authStatus;
        }
    }
}
