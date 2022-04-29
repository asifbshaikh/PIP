using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IOtherPriceAdjustmentRepository
    {
        Task<OtherPriceAdjustmentSubDTO> GetOtherPriceAdjustment(int pipSheetId);
        Task SaveOtherPriceAdjustmentData(string userName, IList<OtherPriceAdjustmentDTO> otherPriceAdjustmentDTO,  IList<OtherPriceAdjustmentPeriodDetailDTO> otherPriceAdjustmentPeriodDetailDTO, IList<OtherPriceAdjustmentPeriodTotalDTO> otherPriceAdjustmentPeriodTotalDTO, bool isMonthlyFeeAdjustment);
    }
}
