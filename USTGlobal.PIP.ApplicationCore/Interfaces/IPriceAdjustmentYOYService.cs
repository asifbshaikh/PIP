using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPriceAdjustmentYoyService
    {
        Task<PriceAdjustmentDTO> GetPriceAdjustmentYoy(int pipSheetId);
        Task SavePriceAdjustmentYoy(string userName, PriceAdjustmentDTO priceAdjustmentDTO);
    }
}
