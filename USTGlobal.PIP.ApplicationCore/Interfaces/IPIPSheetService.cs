using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPipSheetService
    {
        Task<PipSheetDTO> GetByPIPSheetId(int pipSheetId);
        Task<ProjectControlDTO> GetProjectControlData(int pipSheetId);
        Task<bool> SaveProjectControlData(string userName, ProjectControlDTO projectControlDTO);
        Task<HeaderInfoDTO> GetHeader1Data(int projectId, int pipSheetId);
        Task UpdatePIPSheetCurrency(int pipSheetId, int currencyId);
        Task<PipSheetVersionMainDTO> GetPIPSheetVersionData(int projectId, string userName);
        Task<bool> SubmitPIPSheet(PipSheetMainDTO pipSheetMain, string userName);
        Task<CurrencyDTO> GetCurrencyConversionData(int pipSheetId);
        Task<ProjectControlDTO> ReAssignHoursPerDayHoursPerMonth(int projectId, ProjectControlDTO projectControl);
        Task UpdatePIPSheetCheckIn(PipCheckInDTO pipCheckIn, string userName);
        Task<PipCheckInDTO> GetPipCheckInCheckOut(int pipSheetId);
        Task<SubmitPipSheetDTO> GetPIPSheetStatus(PipSheetMainDTO pipSheetMain);
        Task<List<PipOverrideDTO>> GetPipOverrides(int pipSheetId);
    }
}
