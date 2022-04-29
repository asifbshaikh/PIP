using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class ProjectHeaderRepository : IProjectHeaderRepository
    {
        private readonly PipContext pipContext;

        public ProjectHeaderRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<ProjectHeaderCurrencyDTO> GetProjectHeaderData(int projectId, int pipSheetId)
        {
            ProjectHeaderCurrencyDTO projectHeaderUIDTO = new ProjectHeaderCurrencyDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetProjectHeaderAndCurrency")
                           .WithSqlParam("@PIPSheetId", pipSheetId)
                           .ExecuteStoredProcAsync((projectHeaderAndCurrencyResultSet) =>
                           {
                               projectHeaderUIDTO.ProjectHeader = projectHeaderAndCurrencyResultSet.ReadToList<ProjectHeaderDTO>().FirstOrDefault();
                               projectHeaderAndCurrencyResultSet.NextResult();

                               projectHeaderUIDTO.Currency = projectHeaderAndCurrencyResultSet.ReadToList<CurrencyDTO>().FirstOrDefault();
                               projectHeaderAndCurrencyResultSet.NextResult();

                               projectHeaderUIDTO.TotalVersionsPresent = projectHeaderAndCurrencyResultSet.ReadToValue<int>();
                           });

            return projectHeaderUIDTO;
        }

        public async Task<RouteParamDTO> SaveProjectHeaderData(string userName, ProjectHeaderDTO projectHeader)
        {
            RouteParamDTO routeParams = new RouteParamDTO();
            try
            {
                SqlParameter[] inputParams = new SqlParameter[18];
                inputParams[0] = new SqlParameter("@ProjectId", projectHeader.ProjectId);
                inputParams[1] = new SqlParameter("@PIPSheetId", projectHeader.PIPSheetId);
                inputParams[2] = new SqlParameter("@SFProjectId", projectHeader.SfProjectId);
                inputParams[3] = new SqlParameter("@ProjectName", projectHeader.ProjectName);
                inputParams[4] = new SqlParameter("@AccountId", projectHeader.AccountId);
                inputParams[5] = new SqlParameter("@ContractingEntityId", projectHeader.ContractingEntityId.HasValue ?
                    (object)projectHeader.ContractingEntityId : DBNull.Value);
                inputParams[6] = new SqlParameter("@DeliveryOwner", System.Data.SqlDbType.NVarChar);
                inputParams[6].Value = (object)projectHeader.DeliveryOwner ?? DBNull.Value;
                inputParams[7] = new SqlParameter("@ServicePortfolioId", projectHeader.ServicePortfolioId.HasValue ?
                   (object)projectHeader.ServicePortfolioId : DBNull.Value);
                inputParams[8] = new SqlParameter("@ServiceLineId", projectHeader.ServiceLineId.HasValue ?
                       (object)projectHeader.ServiceLineId : DBNull.Value);
                inputParams[9] = new SqlParameter("@ProjectDeliveryTypeId", projectHeader.ProjectDeliveryTypeId);
                inputParams[10] = new SqlParameter("@ProjectBillingTypeId", projectHeader.ProjectBillingTypeId);
                inputParams[11] = new SqlParameter("@SubmittedDate", DateTime.UtcNow);
                inputParams[12] = new SqlParameter("@RequestingUserId", userName);
                inputParams[13] = new SqlParameter("@CurrencyId", projectHeader.CurrencyId);
                inputParams[14] = new SqlParameter("@OutputProjectId", System.Data.SqlDbType.Int);
                inputParams[14].Direction = System.Data.ParameterDirection.Output;

                inputParams[15] = new SqlParameter("@OutputPIPSheetId", System.Data.SqlDbType.Int);
                inputParams[15].Direction = System.Data.ParameterDirection.Output;

                inputParams[16] = new SqlParameter("@IsDummy", projectHeader.IsDummy);
                inputParams[17] = new SqlParameter("@BeatTax", projectHeader.BeatTax.HasValue ?
                    (object)projectHeader.BeatTax : DBNull.Value);


                await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveProjectHeaderData @ProjectId, @PIPSheetId," +
                    " @SFProjectId, @ProjectName, @AccountId, @ContractingEntityId, @DeliveryOwner," +
                    " @ServicePortfolioId, @ServiceLineId, @ProjectDeliveryTypeId, @ProjectBillingTypeId," +
                    " @SubmittedDate, @RequestingUserId, @CurrencyId, @OutputProjectId OUTPUT,@OutputPIPSheetId OUTPUT, @IsDummy, @BeatTax", inputParams);

                await pipContext.SaveChangesAsync();

                routeParams.ProjectId = Convert.ToInt32(inputParams[14].Value);
                routeParams.PipSheetId = Convert.ToInt32(inputParams[15].Value);
            }
            catch (System.Exception e)
             when (e.Message.Contains("UNIQUE") && e.Message.Contains(projectHeader.SfProjectId))
            {
                routeParams.ErrorCode = -1;
                return routeParams;
            }
            return routeParams;
        }
        public async Task<CurrencyDTO> GetCurrencyConversionData(int countryId)
        {
            return await this.pipContext.Currency
                .Where(country => country.CountryId == countryId)
                .ProjectToType<CurrencyDTO>()
                .SingleOrDefaultAsync();
        }
        public async Task<WorkflowStatusAndAccountSpecificRoleDTO> GetWorkflowStatusAccountRole(string userName, int pipSheetId, int accountId, int projectId = 0)
        {
            WorkflowStatusAndAccountSpecificRoleDTO workflowStatusAndAccountSpecificRoleDTO = new WorkflowStatusAndAccountSpecificRoleDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetWorkflowStatusAndAccountSpecificRole")
                           .WithSqlParam("@ProjectId", projectId)
                           .WithSqlParam("@UserName", userName)
                           .WithSqlParam("@PIPSheetId", pipSheetId)
                           .WithSqlParam("@AccountId", accountId)
                           .ExecuteStoredProcAsync((WorkflowStatusAndAccountSpecificRoleResultSet) =>
                           {
                               workflowStatusAndAccountSpecificRoleDTO.PipSheetWorkflowStatus = WorkflowStatusAndAccountSpecificRoleResultSet.ReadToList<PipSheetWorkflowStatus>().FirstOrDefault();
                               WorkflowStatusAndAccountSpecificRoleResultSet.NextResult();

                               workflowStatusAndAccountSpecificRoleDTO.RoleAndAccountDTO = WorkflowStatusAndAccountSpecificRoleResultSet.ReadToList<RoleAndAccountDTO>().ToList();
                               WorkflowStatusAndAccountSpecificRoleResultSet.NextResult();

                               workflowStatusAndAccountSpecificRoleDTO.HasAccountLevelAccess = Convert.ToBoolean(WorkflowStatusAndAccountSpecificRoleResultSet.ReadToValue<bool>());
                               WorkflowStatusAndAccountSpecificRoleResultSet.NextResult();

                               workflowStatusAndAccountSpecificRoleDTO.CanNavigate = Convert.ToBoolean(WorkflowStatusAndAccountSpecificRoleResultSet.ReadToValue<bool>());
                               WorkflowStatusAndAccountSpecificRoleResultSet.NextResult();

                               workflowStatusAndAccountSpecificRoleDTO.isDummy = Convert.ToBoolean(WorkflowStatusAndAccountSpecificRoleResultSet.ReadToValue<bool>());


                           });

            return workflowStatusAndAccountSpecificRoleDTO;
        }

        public async Task<RoleAndAccountMainDTO> GetUserRoleForAllAccounts(string userName)
        {
            RoleAndAccountMainDTO roleAndAccountMainDTO = new RoleAndAccountMainDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetLoggedInUserRolesForAllAccounts")
                          .WithSqlParam("@UserName", userName)
                          .ExecuteStoredProcAsync((userSpecificRoleResultSet) =>
                          {
                              roleAndAccountMainDTO.AccountLevelAccessIds = userSpecificRoleResultSet.ReadToList<AccountId>().ToList();
                              userSpecificRoleResultSet.NextResult();

                              roleAndAccountMainDTO.RoleAndAccountDTO = userSpecificRoleResultSet.ReadToList<RoleAndAccountDTO>().ToList();
                              userSpecificRoleResultSet.NextResult();

                              roleAndAccountMainDTO.SharedAccountRoles = userSpecificRoleResultSet.ReadToList<RoleAndAccountDTO>().ToList();
                          });
            return roleAndAccountMainDTO;
        }

        public async Task<string> GetAutoGeneratedProjectId(int accountId, string accountCode)
        {
            var pips = await (from proj in pipContext.Project
                              join pip in pipContext.PipSheet on proj.ProjectId equals pip.ProjectId
                              where (proj.AccountId == accountId) && (pip.isDummy == true)
                              select pip.ProjectId
                                ).Distinct().ToListAsync();

            return string.Concat("TEST-", accountCode, "-", pips.Count() + 1);
        }

        public async Task<List<ProjectDTO>> GetProjectsByAccountId(int accountId)
        {
            var projects = await (
                    from proj in pipContext.Project
                    join pip in pipContext.PipSheet on proj.ProjectId equals pip.ProjectId into ps
                    from pip in ps.DefaultIfEmpty()
                    where (proj.AccountId == accountId && proj.IsActive == true && proj.Status != 3 && proj.Status != 2 && pip.PipSheetStatusId == 1 && pip.isDummy == false)
                    select new ProjectDTO
                    {
                        ProjectId = proj.ProjectId,
                        SFProjectId = proj.SFProjectId
                    }).Distinct().ToListAsync();

            return projects;

        }
    }
}
