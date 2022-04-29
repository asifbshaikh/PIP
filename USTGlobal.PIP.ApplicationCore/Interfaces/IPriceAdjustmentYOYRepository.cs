using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPriceAdjustmentYoyRepository
    {
        Task<IList<PriceAdjustmentResourceDTO>> GetLaborPricingDataForPriceAdjustment(int pipSheetId);
        Task<PriceAdjustmentDTO> GetPriceAdjustmentYoy(int pipSheetId);
        Task SavePriceAdjustmentYoy(string userName, PriceAdjustmentDTO priceAdjustmentDTO, IList<ProjectResourcePeriodDTO> projectResourcePeriodDTO,
            IList<ResourceLaborPricingSubDTO> resourceLaborPricingDTO, IList<ProjectPeriodTotalDTO> projectPeriodTotals);
    }
}
