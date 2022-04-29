using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IOtherPriceAdjustmentService
    {
        Task<OtherPriceAdjustmentMainDTO> GetOtherPriceAdjustment(int pipSheetId);
        Task SaveOtherPriceAdjustmentData(string userName, OtherPriceAdjustmentMainDTO otherPriceAdjustmentMainDTO);
        OtherPriceAdjustmentMainDTO ReAssignUIds(OtherPriceAdjustmentMainDTO otherPriceAdjustment);
    }
}
