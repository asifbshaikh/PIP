using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IClientPriceRepository
    {
        Task<ClientPriceDBResultDTO> GetClientPriceData(int pipSheetId);
        Task SaveClientPriceData(IList<ClientPricePeriodDTO> clientPricePeriodDTOs, IList<ClientPriceDTO> clientPriceDTOs, IList<ProjectPeriodTotalDTO> projectPeriodTotalDTOs, string userName);
        Task<ClientPriceDBResultDTO> GetClientPriceForCalculation(int pipSheetId);
    }
}
