using USTGlobal.PIP.ApplicationCore.DTOs;
using System.Threading.Tasks;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IOtherPriceAdjustmentWorkflowService
    {
        Task ProcessOtherPriceAdjustmentSaving(string userName, OtherPriceAdjustmentMainDTO otherPriceAdjustmentMainDTO);
        Task<OtherPriceAdjustmentMainDTO> GetOtherPriceAdjustment(int pipSheetId, string userName);
    }
}
