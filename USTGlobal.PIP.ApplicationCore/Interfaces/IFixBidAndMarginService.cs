using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IFixBidAndMarginService
    {
        Task<FixBidAndMarginDTO> CalculateAndSaveFixBidData(int pipSheetId, string userName);
    }
}
