using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IClientPriceService
    {
        Task<ClientPriceMainDTO> GetClientPriceData(int pipSheetId);
        Task SaveClientPriceData(ClientPriceMainDTO clientPriceMainDTO, string userName);
        Task<ClientPriceMainDTO> CalculateTotalClientPrice(int pipSheetId, string userName);
    }
}
