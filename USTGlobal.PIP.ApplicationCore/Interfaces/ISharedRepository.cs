using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ISharedRepository
    {
        Task<PipSheetSaveStatusDTO> GetPipSheetSaveStatus(int pipSheetId);
        Task<OverrideNotificationDTO> GetOverrideNotificationStatus(int pipSheetId);
    }
}
