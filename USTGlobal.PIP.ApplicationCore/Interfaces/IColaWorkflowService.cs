using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IColaWorkflowService
    {
        Task ProcessPriceAdjustmentYoySaving(string userName, PriceAdjustmentDTO priceAdjustmentDTO);
    }
}
