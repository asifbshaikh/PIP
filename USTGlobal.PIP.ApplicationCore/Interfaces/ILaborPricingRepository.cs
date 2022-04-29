using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ILaborPricingRepository
    {
        Task<LaborPricingDTO> GetLaborPricingData(int pipSheetId);
        Task SaveLaborPricingData(string userName, LaborPricingDTO laborPricingDTO, IList<ProjectResourcePeriodDTO> projectResourcePeriodDTO,
            IList<ResourceLaborPricingSubDTO> resourceLaborPricingDTO, IList<ProjectPeriodTotalDTO> projectPeriodTotals);
        Task<LaborPricingBackgroundCalcParentDTO> CalculateBackgroundFields(int pipSheetId);
        Task<LaborPricingBackgroundCalcParentDTO> GetResourceCostCalculationFields(int pipSheetId);
    }
}
