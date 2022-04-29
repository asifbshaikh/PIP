using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class CapitalChargeWorkflowService : ICapitalChargeWorkflowService
    {
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ILaborPricingService laborPricingService;
        private readonly ISharedRepository sharedRepository;
        private readonly IVacationAbsencesService vacationAbsencesService;
        private readonly IPriceAdjustmentYoyService priceAdjustmentYoyService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly IRiskManagementService riskManagementService;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;

        public CapitalChargeWorkflowService(ICapitalChargeService capitalChargeService, ILaborPricingService laborPricingService, ISharedRepository sharedRepository,
            IRiskManagementService riskManagementService, IVacationAbsencesService vacationAbsencesService, IPriceAdjustmentYoyService priceAdjustmentYoyService,
            IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService, IOtherPriceAdjustmentService otherPriceAdjustmentService)
        {
            this.capitalChargeService = capitalChargeService;
            this.laborPricingService = laborPricingService;
            this.sharedRepository = sharedRepository;
            this.sharedRepository = sharedRepository;
            this.vacationAbsencesService = vacationAbsencesService;
            this.riskManagementService = riskManagementService;
            this.priceAdjustmentYoyService = priceAdjustmentYoyService;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
        }

        public async Task ProcessCapitalChargeSaving(string userName, CapitalChargeResultSetDTO capitalCharge)
        {
            int pipSheetId = capitalCharge.capitalChargeDTO.PipSheetId;
            await this.capitalChargeService.SaveCapitalCharge(userName, capitalCharge);

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

                // Labor Pricing
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
            }

            // Fixed Bid and Margin 
            if (pipSheetSaveStatus.FixBidAndMargin == true)
            {
                await this.fixbidAndMarginService.CalculateAndSaveFixBidData(pipSheetId, userName);
            }

            if (pipSheetSaveStatus.OtherPriceAdjustment == true)
            {
                OtherPriceAdjustmentMainDTO priceAdjustment = await this.otherPriceAdjustmentService.GetOtherPriceAdjustment(pipSheetId);
                await this.otherPriceAdjustmentService.SaveOtherPriceAdjustmentData(userName, priceAdjustment);
            }

            // Client Price 
            if (pipSheetSaveStatus.ClientPrice == true)
            {
                ClientPriceMainDTO clientPriceMainDTO = await this.clientPriceService.CalculateTotalClientPrice(pipSheetId, userName);
                await this.clientPriceService.SaveClientPriceData(clientPriceMainDTO, userName);
            }

            // Capital Charge 
            if (pipSheetSaveStatus.CapitalCharge == true)
            {
                CapitalChargeResultSetDTO capitalChargeResultSetDTO = await this.capitalChargeService.CalculateCapitalCharges(pipSheetId);
                await this.capitalChargeService.SaveCapitalCharge(userName, capitalChargeResultSetDTO);
            }

            // Client Price 
            if (pipSheetSaveStatus.ClientPrice == true)
            {
                ClientPriceMainDTO clientPriceMainDTO = await this.clientPriceService.CalculateTotalClientPrice(pipSheetId, userName);
                await this.clientPriceService.SaveClientPriceData(clientPriceMainDTO, userName);
            }
        }
    }
}
