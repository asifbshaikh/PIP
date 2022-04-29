using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPipVersionRepository
    {
        Task<int> CreateNewPipSheetVersion(string userName, int projectId, int pipsheetId);
        Task DeletePipSheet(int pipSheetId, int projectId, string userName);
        Task<SummaryPipVersionDTO> GetVersionDetailsOnSummary(int pipSheetId);       
    }
}
