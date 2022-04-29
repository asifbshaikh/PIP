using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Entities;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class PipSheetRepository : IPipSheetRepository
    {
        private readonly PipContext pipContext;

        public PipSheetRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<PipSheetDTO> GetByPIPSheetId(int pipSheetId)
        {
            return await this.pipContext.PipSheet
                .Where(pip => pip.PipSheetId == pipSheetId)
                .ProjectToType<PipSheetDTO>().SingleOrDefaultAsync();
        }

        public async Task<ProjectControlDTO> GetProjectControlData(int pipSheetId)
        {
            ProjectControlDTO projectControlDTO = new ProjectControlDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetProjectControlData")
               .WithSqlParam("@PIPSheetId", pipSheetId)
               .ExecuteStoredProcAsync((projectControlResultSet) =>
               {
                   var pipSheets = projectControlResultSet.ReadToList<PipSheetDTO>();
                   projectControlDTO.PIPSheetListDTO = pipSheets;
                   projectControlResultSet.NextResult();

                   var projectLocations = projectControlResultSet.ReadToList<ProjectLocationDTO>();
                   projectControlDTO.ProjectLocationListDTO = projectLocations;
                   projectControlResultSet.NextResult();

                   var projectMilestones = projectControlResultSet.ReadToList<ProjectMilestoneDTO>();
                   projectControlDTO.ProjectMilestoneListDTO = projectMilestones;
               });

            return projectControlDTO;
        }

        public async Task<bool> SaveProjectControlData(string userName, ProjectControlDTO projectControlDTO)
        {
            SqlParameter[] inputParams = new SqlParameter[9];
            inputParams[0] = new SqlParameter("@StartDate", projectControlDTO.PIPSheetListDTO[0].StartDate);
            inputParams[1] = new SqlParameter("@EndDate", projectControlDTO.PIPSheetListDTO[0].EndDate);
            inputParams[2] = new SqlParameter("@HolidayOption", projectControlDTO.PIPSheetListDTO[0].HolidayOption);
            inputParams[3] = new SqlParameter("@PipSheetId", projectControlDTO.PIPSheetListDTO[0].PIPSheetId);
            inputParams[4] = new SqlParameter("@MilestoneGroupId", projectControlDTO.PIPSheetListDTO[0].MilestoneGroupId.HasValue ? (object)projectControlDTO.PIPSheetListDTO[0].MilestoneGroupId : DBNull.Value);
            inputParams[5] = new SqlParameter("@RequestingUserId", userName);
            inputParams[6] = new SqlParameter("@InputProjectLocations", SqlDbType.Structured);
            inputParams[6].Value = IListToDataTableHelper.ToDataTables(projectControlDTO.ProjectLocationListDTO);
            inputParams[6].TypeName = "dbo.ProjectLocation";
            inputParams[7] = new SqlParameter("@InputProjectMilestones", SqlDbType.Structured);
            inputParams[7].Value = IListToDataTableHelper.ToDataTables(projectControlDTO.ProjectMilestoneListDTO);
            inputParams[7].TypeName = "dbo.ProjectMilestone";
            inputParams[8] = new SqlParameter
            {
                ParameterName = "@IsAnyLocationDeleted",
                DbType = System.Data.DbType.Boolean,
                Direction = System.Data.ParameterDirection.Output
            };

            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveProjectControlData @StartDate, @EndDate, @HolidayOption, " +
                " @PipSheetId, @MilestoneGroupId, @RequestingUserId, @InputProjectLocations, @InputProjectMilestones, @IsAnyLocationDeleted output "
                , inputParams);
            object IsAnyLocationDeleted = Convert.ToBoolean(inputParams[8].Value);
            await pipContext.SaveChangesAsync();

            return (bool)IsAnyLocationDeleted;
        }

        /// <summary>
        /// Get PIPSheet duration
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<HeaderInfoDTO> GetHeader1Data(int projectId, int pipSheetId)
        {
            HeaderInfoDTO headerInfoDTO = new HeaderInfoDTO();
            headerInfoDTO.HeaderEbitda = new HeaderEbitdaDTO();
            headerInfoDTO.Header1 = new Header1DTO();
            // Send only project related data when pipsheet id not present
            if (pipSheetId == 0)
            {
                headerInfoDTO.Header1 = await (
                  from project in pipContext.Project
                  join account in pipContext.Account on project.AccountId equals account.AccountId
                  where project.ProjectId == projectId
                  select new Header1DTO
                  {
                      SFAccountName = account.AccountName,
                      ProjectName = project.ProjectName,
                  }).SingleOrDefaultAsync();

                return headerInfoDTO;
            }

            await pipContext.LoadStoredProc("dbo.sp_GetHeader1Data")
            .WithSqlParam("@ProjectId", projectId)
              .WithSqlParam("@PIPSheetId", pipSheetId)
              .ExecuteStoredProcAsync((header1ResultSet) =>
              {
                  headerInfoDTO.Header1 = header1ResultSet.ReadToList<Header1DTO>().FirstOrDefault();
                  header1ResultSet.NextResult();

                  headerInfoDTO.HeaderEbitda = header1ResultSet.ReadToList<HeaderEbitdaDTO>().FirstOrDefault();
              });
            return headerInfoDTO;
        }

        public async Task UpdatePIPSheetCurrency(int pipSheetId, int currencyId)
        {
            var pipSheetRow = await (from pipSheet in this.pipContext.PipSheet
                                     where pipSheet.PipSheetId == pipSheetId
                                     select new PipSheet
                                     {
                                         ProjectId = pipSheet.ProjectId,
                                         PipSheetId = pipSheet.PipSheetId,
                                         VersionNumber = pipSheet.VersionNumber,
                                         CreatedBy = pipSheet.CreatedBy,
                                         UpdatedBy = pipSheet.UpdatedBy,
                                         CreatedOn = pipSheet.CreatedOn,
                                         UpdatedOn = DateTime.UtcNow,
                                         CurrencyId = currencyId,
                                         IsCheckedOut = pipSheet.IsCheckedOut
                                     }).FirstOrDefaultAsync();



            pipContext.Update(pipSheetRow);
            await pipContext.SaveChangesAsync();
        }

        public async Task<PipSheetVersionListAndRoleDTO> GetPIPSheetVersionData(int projectId, string userName)
        {
            PipSheetVersionListAndRoleDTO pipSheetVersionListAndRoleDTO = new PipSheetVersionListAndRoleDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetPIPSheetVersionData")
           .WithSqlParam("@ProjectId", projectId)
            .WithSqlParam("@UserName", userName)
             .ExecuteStoredProcAsync((versionDataResultSet) =>
             {
                 pipSheetVersionListAndRoleDTO.PipSheetVersionListDTO = versionDataResultSet.ReadToList<PipSheetVersionListDTO>().ToList();
                 versionDataResultSet.NextResult();

                 pipSheetVersionListAndRoleDTO.RoleNameDTO = versionDataResultSet.ReadToList<RoleNameDTO>().ToList();
                 versionDataResultSet.NextResult();

                 pipSheetVersionListAndRoleDTO.ProjectWorkflowStatus = Convert.ToInt32(versionDataResultSet.ReadToValue<int>());
             });
            return pipSheetVersionListAndRoleDTO;
        }

        public async Task SubmitPIPSheet(PipSheetMainDTO pipSheetMain, string userName)
        {
            SubmitPipSheetDTO submitPipSheetDTO = new SubmitPipSheetDTO();
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SubmitPipSheet {0}, {1}",
              userName,
              new SqlParameter("@InputProjectPeriod", SqlDbType.Structured)
              {
                  Value = IListToDataTableHelper.ToDataTable(pipSheetMain),
                  TypeName = "dbo.PipSheet"
              });
            await pipContext.SaveChangesAsync();
        }

        public async Task<CurrencyDTO> GetCurrencyConversionData(int pipSheetId)
        {
            return
                await (
                from pipSheet in this.pipContext.PipSheet
                join currency in this.pipContext.Currency on pipSheet.CurrencyId equals currency.CurrencyId
                where pipSheet.PipSheetId == pipSheetId
                select new CurrencyDTO()
                {
                    CurrencyId = currency.CurrencyId,
                    CountryId = currency.CountryId,
                    Symbol = currency.Symbol,
                    Factors = currency.Factors,
                    USDToLocal = currency.USDToLocal,
                    LocalToUSD = currency.LocalToUSD,
                    MasterVersionId = currency.MasterVersionId
                }).FirstOrDefaultAsync();
        }

        public async Task<GrossProfitDTO> GetGrossProfit(int pipSheetId)
        {
            return await (
                 from calculatedValue in pipContext.CalculatedValue
                 where calculatedValue.PipSheetId == pipSheetId
                 select new GrossProfitDTO
                 {
                     TotalClientPrice = calculatedValue.TotalClientPrice ?? 0,
                     FeesAtRisk = calculatedValue.TotalFeesAtRisk * (-1) ?? 0, //Fees At Risk is always Negative. Hence, multiplied by -1
                     NetEstimatedRevenue = calculatedValue.TotalNetEstimatedRevenue ?? 0,
                     TotalProjectCost = calculatedValue.TotalProjectCost ?? 0,
                     TotalSeatCost = calculatedValue.SeatCostEbitda ?? 0
                 }).SingleOrDefaultAsync();
        }

        public async Task UpdatePIPSheetCheckIn(PipCheckInDTO pipCheckIn, string userName)
        {
            await pipContext.Database.ExecuteSqlCommandAsync("exec dbo.sp_UpdatePipCheckInOut {0}, {1}, {2}",
              userName,
              pipCheckIn.PIPSheetId,
              pipCheckIn.IsCheckedOut);

            await pipContext.SaveChangesAsync();
        }

        public async Task<PipCheckInDTO> GetPipCheckInCheckOut(int pipSheetId)
        {
            PipCheckInDTO pipCheckInDTO = new PipCheckInDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetPipCheckInCheckOut")
               .WithSqlParam("@PipSheetId", pipSheetId)
               .ExecuteStoredProcAsync((result) =>
               {
                   pipCheckInDTO = result.ReadToList<PipCheckInDTO>().FirstOrDefault();
               });
            return pipCheckInDTO;
        }

        public async Task<PipSheetStatusDTO> GetPipsheetStatus(int pipSheetId)
        {
            return await (
                from pipSheet in pipContext.PipSheet
                join user in this.pipContext.User on pipSheet.ApprovedBy equals user.UserId into ur
                from usr in ur.DefaultIfEmpty()
                join project in this.pipContext.Project on pipSheet.ProjectId equals project.ProjectId
                where pipSheet.PipSheetId == pipSheetId
                select new PipSheetStatusDTO
                {
                    PipSheetStatusId = pipSheet.PipSheetStatusId,
                    ApproverStatusId = pipSheet.ApproverStatusId,
                    ApproversName = usr.FirstName + usr.LastName,
                    SFProjectId = project.SFProjectId
                }).SingleOrDefaultAsync();
        }

        public async Task<List<PipOverrideDTO>> GetPipOverrides(int pipSheetId)
        {
            List<PipOverrideDTO> listPipOverrides = new List<PipOverrideDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetPipOverrides")
               .WithSqlParam("@PipSheetId", pipSheetId)
               .ExecuteStoredProcAsync((resultSet) =>
               {
                   listPipOverrides = resultSet.ReadToList<PipOverrideDTO>().ToList();
               });
            return listPipOverrides;
        }
    }
}
