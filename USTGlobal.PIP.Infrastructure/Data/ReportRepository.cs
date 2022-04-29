using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class ReportRepository : IReportRepository
    {
        private readonly PipContext pipContext;

        public ReportRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<AccountBasedProjectDTO>> GetAccountBasedProjects(IList<AccountDTO> accountsList)
        {
            List<AccountBasedProjectDTO> accountBasedProjects = new List<AccountBasedProjectDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetAccountsForReports")
                  .WithSqlParam("@SelectedAccounts", new SqlParameter("@SelectedAccounts", SqlDbType.Structured)
                  {
                      Value = IListToDataTableHelper.ToDataTables(accountsList),
                      TypeName = "dbo.Account"
                  })
                  .ExecuteStoredProcAsync((result) =>
                  {
                      accountBasedProjects = result.ReadToList<AccountBasedProjectDTO>().ToList();
                  });
            return accountBasedProjects;
        }

        public async Task<List<ReportKPIDTO>> GetCustomReportKPIList()
        {
            return await (from r in this.pipContext.ReportKPI
                          where r.ReportKPIId == 1 || r.ReportKPIId == 2 || r.ReportKPIId == 3 || r.ReportKPIId == 4 || r.ReportKPIId == 5
                          select new ReportKPIDTO
                          {
                              KPIId = r.ReportKPIId,
                              KPIName = r.KPIName,
                              PLForecastLabelId = r.PLForecastLabelId
                          }).ToListAsync();
        }

        public async Task<ReportDBResultDTO> GetSummaryReportViewReport(DateTime startDate, DateTime endDate, IList<AccountBasedProjectDTO> projectIds, bool isUSDCurrency, string userName)
        {
            ReportDBResultDTO summaryViewReportDBResultDTO = new ReportDBResultDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetProjectSummaryViewReport")
                  .WithSqlParam("@StartDate", startDate)
                  .WithSqlParam("@EndDate", endDate)
                  .WithSqlParam("@SelectedProjects", new SqlParameter("@SelectedProjects", SqlDbType.Structured)
                  {
                      Value = IListToDataTableHelper.ToDataTables(projectIds),
                      TypeName = "dbo.AccountBasedProject"
                  })
                  .WithSqlParam("@IsUSDCurrency", isUSDCurrency)
                  .WithSqlParam("@UserName", userName)
                  .ExecuteStoredProcAsync((result) =>
                  {
                      summaryViewReportDBResultDTO.ReportFirstHalf = result.ReadToList<ReportFirstHalfDTO>().ToList();
                      result.NextResult();
                      summaryViewReportDBResultDTO.PLForecast = result.ReadToList<ReportPLForecastDTO>().ToList();
                      result.NextResult();
                      summaryViewReportDBResultDTO.PLForecastPeriods = result.ReadToList<ReportPLForecastPeriodDTO>().ToList();
                      result.NextResult();
                      summaryViewReportDBResultDTO.ReportPeriods = result.ReadToList<ReportPeriodDTO>().ToList();
                      result.NextResult();
                      summaryViewReportDBResultDTO.ReportKPIs = result.ReadToList<ReportKPIDTO>().ToList();
                      result.NextResult();
                      summaryViewReportDBResultDTO.ReportEmail = result.ReadToList<ReportEmail>().SingleOrDefault();
                  });
            return summaryViewReportDBResultDTO;
        }

        public async Task<ReportDBResultDTO> GetProjectDetailedLevelReport(DateTime startDate, DateTime endDate, IList<AccountBasedProjectDTO> projectIds, bool isUSDCurrency, string userName)
        {
            ReportDBResultDTO reportDBResultDTO = new ReportDBResultDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetProjectDetailedLevelReport")
                  .WithSqlParam("@StartDate", startDate)
                  .WithSqlParam("@EndDate", endDate)
                  .WithSqlParam("@SelectedProjects", new SqlParameter("@SelectedProjects", SqlDbType.Structured)
                  {
                      Value = IListToDataTableHelper.ToDataTables(projectIds),
                      TypeName = "dbo.AccountBasedProject"
                  })
                  .WithSqlParam("@IsUSDCurrency", isUSDCurrency)
                  .WithSqlParam("@UserName", userName)
                  .ExecuteStoredProcAsync((result) =>
                  {
                      reportDBResultDTO.ReportFirstHalf = result.ReadToList<ReportFirstHalfDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.PLForecast = result.ReadToList<ReportPLForecastDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.PLForecastPeriods = result.ReadToList<ReportPLForecastPeriodDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportInvoicingSchedule = result.ReadToList<ClientPriceDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportInvoicingSchedulePeriodDetails = result.ReadToList<ClientPricePeriodDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportPeriods = result.ReadToList<ReportPeriodDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportKPIs = result.ReadToList<ReportKPIDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportEmail = result.ReadToList<ReportEmail>().SingleOrDefault();
                  });
            return reportDBResultDTO;
        }

        public async Task<ReportDBResultDTO> GetCustomReportData(DateTime startDate, DateTime endDate, IList<AccountBasedProjectDTO> projectIds, bool isUSDCurrency, string userName, IList<ReportKPIDTO> reportKPIs)
        {
            ReportDBResultDTO reportDBResultDTO = new ReportDBResultDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetCustomReport")
                  .WithSqlParam("@StartDate", startDate)
                  .WithSqlParam("@EndDate", endDate)
                  .WithSqlParam("@SelectedProjects", new SqlParameter("@SelectedProjects", SqlDbType.Structured)
                  {
                      Value = IListToDataTableHelper.ToDataTables(projectIds),
                      TypeName = "dbo.AccountBasedProject"
                  })
                  .WithSqlParam("@IsUSDCurrency", isUSDCurrency)
                  .WithSqlParam("@CustomReportKPIList", new SqlParameter("@CustomReportKPIList", SqlDbType.Structured)
                  {
                      Value = IListToDataTableHelper.ToDataTables(reportKPIs),
                      TypeName = "dbo.ReportKPI"
                  })
                  .WithSqlParam("@UserName", userName)
                  .ExecuteStoredProcAsync((result) =>
                  {
                      reportDBResultDTO.ReportFirstHalf = result.ReadToList<ReportFirstHalfDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportResources = result.ReadToList<ReportResourceDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportResourcePeriodDetails = result.ReadToList<ReportResourcePeriodDetailDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportPeriods = result.ReadToList<ReportPeriodDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportKPIs = result.ReadToList<ReportKPIDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportEmail = result.ReadToList<ReportEmail>().SingleOrDefault();
                  });
            return reportDBResultDTO;
        }

        public async Task<ReportDBResultDTO> GetProjectResourceLevelReport(int projectId, bool isUSDCurrency, string userName)
        {
            ReportDBResultDTO reportDBResultDTO = new ReportDBResultDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetProjectResourceViewReport")
                  .WithSqlParam("@ProjectId", projectId)
                  .WithSqlParam("@IsUSDCurrency", isUSDCurrency)
                  .WithSqlParam("@UserName", userName)

                  .ExecuteStoredProcAsync((result) =>
                  {
                      reportDBResultDTO.ReportFirstHalf = result.ReadToList<ReportFirstHalfDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.PLForecast = result.ReadToList<ReportPLForecastDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.PLForecastPeriods = result.ReadToList<ReportPLForecastPeriodDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportInvoicingSchedule = result.ReadToList<ClientPriceDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportInvoicingSchedulePeriodDetails = result.ReadToList<ClientPricePeriodDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportResources = result.ReadToList<ReportResourceDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportResourcePeriodDetails = result.ReadToList<ReportResourcePeriodDetailDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportPeriods = result.ReadToList<ReportPeriodDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportKPIs = result.ReadToList<ReportKPIDTO>().ToList();
                      result.NextResult();
                      reportDBResultDTO.ReportEmail = result.ReadToList<ReportEmail>().SingleOrDefault();
                  });
            return reportDBResultDTO;


        }

        public async Task<List<AccountId>> GetAuthorizedAccounts(string userName)
        {
            return await (from ur in this.pipContext.UserRole
                          join u in this.pipContext.User on ur.UserId equals u.UserId
                          where u.Email == userName && ur.RoleId == 3
                          select new AccountId
                          {
                              AccountLevelAccessIds = Convert.ToInt32(ur.AccountId)
                          }).ToListAsync();
        }
    }
}
