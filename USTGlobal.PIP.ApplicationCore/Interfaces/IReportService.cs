using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IReportService
    {
        Task<List<AccountBasedProjectDTO>> GetAccountBasedProjects(IList<AccountDTO> accountsList);
        Task<bool> GenerateProjectReport(ReportRequestDTO report, string userName, string reportsFolderPath);
        Task<List<ReportKPIDTO>> GetCustomReportKPIList();
        Task<List<AccountId>> GetAuthorizedAccounts(string userName);
    }
}
