using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IReportRepository
    {
        Task<List<AccountBasedProjectDTO>> GetAccountBasedProjects(IList<AccountDTO> accountsList);
        Task<ReportDBResultDTO> GetSummaryReportViewReport(DateTime startDate, DateTime endDate, IList<AccountBasedProjectDTO> projectIds, bool isUSDCurrency, string userName);
        Task<ReportDBResultDTO> GetProjectDetailedLevelReport(DateTime startDate, DateTime endDate, IList<AccountBasedProjectDTO> projectIds, bool isUSDCurrency, string userName);
        Task<List<ReportKPIDTO>> GetCustomReportKPIList();
        Task<ReportDBResultDTO> GetCustomReportData(DateTime startDate, DateTime endDate, IList<AccountBasedProjectDTO> projectIds, bool isUSDCurrency, string userName, IList<ReportKPIDTO> reportKPIs);
        Task<ReportDBResultDTO> GetProjectResourceLevelReport(int projectId, bool isUSDCurrency, string userName);
        Task<List<AccountId>> GetAuthorizedAccounts(string userName);
    }
}
