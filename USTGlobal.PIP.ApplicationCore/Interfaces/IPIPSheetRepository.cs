using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPipSheetRepository
    {
        Task<PipSheetDTO> GetByPIPSheetId(int pipSheetId);
        Task<ProjectControlDTO> GetProjectControlData(int pipSheetId);
        Task<bool> SaveProjectControlData(string userName, ProjectControlDTO projectControlDTO);
        Task<HeaderInfoDTO> GetHeader1Data(int projectId, int pipSheetId);
        Task UpdatePIPSheetCurrency(int pipSheetId, int currencyId);
        Task<PipSheetVersionListAndRoleDTO> GetPIPSheetVersionData(int projectId, string userName);
        Task SubmitPIPSheet(PipSheetMainDTO pipSheetMain, string userName);
        Task<CurrencyDTO> GetCurrencyConversionData(int pipSheetId);
        Task<GrossProfitDTO> GetGrossProfit(int pipSheetId);
        Task UpdatePIPSheetCheckIn(PipCheckInDTO pipCheckIn, string userName);
        Task<PipCheckInDTO> GetPipCheckInCheckOut(int pipSheetId);
        Task<PipSheetStatusDTO> GetPipsheetStatus(int pipSheetId);
        Task<List<PipOverrideDTO>> GetPipOverrides(int pipSheetId);
    }
}
