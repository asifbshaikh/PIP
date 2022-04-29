using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IClientPriceWorkflowService
    {
        Task ProcessClientPriceSaving(ClientPriceMainDTO clientPriceMainDTO, string userName);
    }
}
