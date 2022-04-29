using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ISharedService
    {
        Task<OverrideNotificationDTO> GetOverrideNotificationStatus(int pipSheetId);
    }
}
