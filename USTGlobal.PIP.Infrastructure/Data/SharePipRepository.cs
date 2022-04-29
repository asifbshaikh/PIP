using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class SharePipRepository : ISharePipRepository
    {
        private readonly PipContext pipContext;

        public SharePipRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<SharePipDTO>> GetSharedPipData(int projectId)
        {
            return await (from sharePip in this.pipContext.SharePipSheet
                          join pipsheet in this.pipContext.PipSheet on sharePip.PipSheetId equals pipsheet.PipSheetId
                          join user in this.pipContext.User on sharePip.SharedWithUserId equals user.UserId
                          where (pipsheet.ProjectId == projectId && pipsheet.IsActive)
                          orderby user.FirstName + " " + user.LastName, pipsheet.VersionNumber
                          select new SharePipDTO
                          {
                              SharedWithUserName = user.FirstName + " " + user.LastName,
                              SharedWithUserId = sharePip.SharedWithUserId,
                              SharedWithUId = user.UID,
                              SharedWithUserEmail = user.Email,
                              AccountId = sharePip.AccountId,
                              PipSheetId = sharePip.PipSheetId,
                              VersionNumber = pipsheet.VersionNumber,
                              VersionName = Constants.VersionString + pipsheet.VersionNumber,
                              ProjectId = pipsheet.ProjectId,
                              RoleId = sharePip.RoleId,
                              ShareComments = sharePip.ShareComments
                          }).ToListAsync();
        }


        public async Task<SharePipDTO> GetSharedPipData(SharePipDTO sharedPIP)
        {

            return await this.pipContext.SharePipSheet
              .Where(shareP => shareP.PipSheetId == sharedPIP.PipSheetId
                      && shareP.ProjectId == sharedPIP.ProjectId
                      && shareP.SharedWithUserId == sharedPIP.SharedWithUserId)
              .Select(sharedPip => new SharePipDTO
              {

                  SharedWithUserId = sharedPip.SharedWithUserId,
                  AccountId = sharedPip.AccountId,
                  PipSheetId = sharedPip.PipSheetId,
                  ProjectId = sharedPip.ProjectId,
                  RoleId = sharedPip.RoleId,
                  ShareComments = sharedPip.ShareComments
              }).FirstAsync();

        }

        public async Task DeleteSharedPipData(int pipSheetId, int roleId, int accountId, int sharedWithUserId)
        {
            SqlParameter[] inputParams = new SqlParameter[4];
            inputParams[0] = new SqlParameter("@PipSheetId", pipSheetId);
            inputParams[1] = new SqlParameter("@RoleId", roleId);
            inputParams[2] = new SqlParameter("@AccountId", accountId);
            inputParams[3] = new SqlParameter("@SharedWithUserId", sharedWithUserId);

            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_DeleteSharedPipSheet @PipSheetId, @RoleId, @AccountId, @SharedWithUserId", inputParams);
            await pipContext.SaveChangesAsync();
        }

        public async Task UpdateSharedPipData(SharePipDTO sharedPip)
        {
            List<SharePipDTO> listSharedPip = new List<SharePipDTO>();
            listSharedPip.Add(sharedPip);
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_UpdateSharePipSheet {0}",
                                new SqlParameter("@InputSharedPipSheet", SqlDbType.Structured)
                                {
                                    Value = IListToDataTableHelper.ToDataTables(listSharedPip),
                                    TypeName = "dbo.SharePipSheet"
                                }
                );
            await pipContext.SaveChangesAsync();
        }

        public async Task<bool> SaveSharedPipData(List<SharePipDTO> sharedPip, string userName)
        {
            bool IsRecordAlreadyExist = false;
            try
            {
                await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveSharePipSheet {0}, {1}",
                 userName,
                 new SqlParameter("@InputSharedPipSheets", SqlDbType.Structured)
                 {
                     Value = IListToDataTableHelper.ToDataTables(sharedPip),
                     TypeName = "dbo.SharePipSheet"
                 });
                await pipContext.SaveChangesAsync();
            }
            catch (Exception exception)
             when (exception.Message.Contains("Violation of PRIMARY KEY constraint 'PK_SharePipSheet'"))
            {
                IsRecordAlreadyExist = true;
                return IsRecordAlreadyExist;
            }
            return IsRecordAlreadyExist;
        }

        public async Task<SharePipVersionDTO> GetSharePipVersionData(int projectId)
        {
            SharePipVersionDTO sharePipVersionDTO = new SharePipVersionDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetSharePipData")
         .WithSqlParam("@ProjectId", projectId)
         .ExecuteStoredProcAsync((sharePipResultSet) =>
         {
             sharePipVersionDTO.projectDTO = sharePipResultSet.ReadToList<ProjectDTO>().FirstOrDefault();
             sharePipResultSet.NextResult();

             sharePipVersionDTO.PipSheetDTO = sharePipResultSet.ReadToList<PipSheetDTO>().ToList();
             sharePipResultSet.NextResult();

             sharePipVersionDTO.UserDTO = sharePipResultSet.ReadToList<UserDTO>().ToList();
         });
            return sharePipVersionDTO;
        }


    }
}
