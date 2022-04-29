using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class RiskManagementWorkflowService : IRiskManagementWorkflowService
    {
        private readonly ILaborPricingService laborPricingService;
        private readonly IRiskManagementService riskManagementService;
        private readonly IVacationAbsencesService vacationAbsencesService;
        private readonly IPriceAdjustmentYoyService priceAdjustmentYoyService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ISharedRepository sharedRepository;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;

        public RiskManagementWorkflowService(ILaborPricingService laborPricingService, IRiskManagementService riskManagementService,
            IVacationAbsencesService vacationAbsencesService, IPriceAdjustmentYoyService priceAdjustmentYoyService, IFixBidAndMarginService fixbidAndMarginService,
            IClientPriceService clientPriceService, ICapitalChargeService capitalChargeService, ISharedRepository sharedRepository,
            IOtherPriceAdjustmentService otherPriceAdjustmentService)
        {
            this.laborPricingService = laborPricingService;
            this.riskManagementService = riskManagementService;
            this.vacationAbsencesService = vacationAbsencesService;
            this.priceAdjustmentYoyService = priceAdjustmentYoyService;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.sharedRepository = sharedRepository;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
        }

        public async Task ProcessRiskManagementSaving(string userName, RiskManagementCalcDTO riskManagementCalcDTO)
        {
            int pipSheetId = riskManagementCalcDTO.riskManagement.PipSheetId;
            await this.riskManagementService.SaveRiskManagement(userName, riskManagementCalcDTO);

            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);

            //Labor Pricing Dependent Save
            if (pipSheetSaveStatus.LaborPricing == true)
            {
                LaborPricingDTO laborPricingDTO = await this.laborPricingService.CalculateLaborPricing(userName, pipSheetId);
                await this.laborPricingService.SaveLaborPricingData(userName, laborPricingDTO);
            }

            // Price Adjustment YOY / COLA
            if (pipSheetSaveStatus.COLA == true)
            {
                PriceAdjustmentDTO priceAdjustmentDTO = await this.priceAdjustmentYoyService.GetPriceAdjustmentYoy(pipSheetId);
                await this.priceAdjustmentYoyService.SavePriceAdjustmentYoy(userName, priceAdjustmentDTO);
            }

            // Vacation absences
            if (pipSheetSaveStatus.VacationAbsences == true)
            {
                VacationAbsencesParentDTO vacationAbsencesParentDTO = await this.vacationAbsencesService.CalculateVacationAbsences(pipSheetId, userName);
                await this.vacationAbsencesService.SaveVacationAbsencesData(userName, vacationAbsencesParentDTO);
            }

            // Risk Management
            if (pipSheetSaveStatus.RiskManagement == true)
            {
                RiskManagementCalcDTO RiskManagementCalcDTO = await this.riskManagementService.CalculateRiskManagementData(pipSheetId, userName);
                await this.riskManagementService.SaveRiskManagement(userName, RiskManagementCalcDTO);
            }

            // Fixed Bid and Margin 
            await this.fixbidAndMarginService.CalculateAndSaveFixBidData(pipSheetId, userName);

            // Other Price Adjustment
            if (pipSheetSaveStatus.OtherPriceAdjustment == true)
            {
                OtherPriceAdjustmentMainDTO priceAdjustment = await this.otherPriceAdjustmentService.GetOtherPriceAdjustment(pipSheetId);
                await this.otherPriceAdjustmentService.SaveOtherPriceAdjustmentData(userName, priceAdjustment);
            }

            // Client Price
            ClientPriceMainDTO clientPriceMainDTO = await this.clientPriceService.CalculateTotalClientPrice(pipSheetId, userName);
            await this.clientPriceService.SaveClientPriceData(clientPriceMainDTO, userName);

            // Capital Charge
            CapitalChargeResultSetDTO capitalChargeResultSetDTO = await this.capitalChargeService.CalculateCapitalCharges(pipSheetId);
            await this.capitalChargeService.SaveCapitalCharge(userName, capitalChargeResultSetDTO);
            
            // Client Price 
            if (pipSheetSaveStatus.ClientPrice == true)
            {
                ClientPriceMainDTO clientPriceMain = await this.clientPriceService.CalculateTotalClientPrice(pipSheetId, userName);
                await this.clientPriceService.SaveClientPriceData(clientPriceMain, userName);
            }
        }
    }
}
