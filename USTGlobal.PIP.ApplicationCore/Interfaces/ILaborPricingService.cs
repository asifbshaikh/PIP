using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ILaborPricingService
    {
        Task<LaborPricingDTO> GetLaborPricingData(int pipSheetId);
        Task SaveLaborPricingData(string userName, LaborPricingDTO laborPricingDTO);
        Task<LaborPricingBackgroundCalculationDTO> CalculateBackgroundFields(int pipSheetId, bool isMarginSet,
            int which, decimal marginPercent, bool isInitLoad, decimal inflatedCappedCost, decimal totalInflation);

        //Task ProcessSaveDependency(int pipSheetId, string userName);

        Task<LaborPricingDTO> CalculateLaborPricing(string emailId, int pipSheetId);

        //Task ProcessLaborPricingSaving(string userName, LaborPricingDTO laborPricingDTO);


    }
}
