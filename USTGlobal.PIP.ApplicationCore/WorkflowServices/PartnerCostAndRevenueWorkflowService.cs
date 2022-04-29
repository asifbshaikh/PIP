using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class PartnerCostAndRevenueWorkflowService : IPartnerCostAndRevenueWorkflowService
    {
        private readonly IPartnerCostAndRevenueService partnerCostAndRevenueService;
        private readonly IRiskManagementService riskManagementService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ISharedRepository sharedRepository;
        private readonly ILaborPricingService laborPricingService;
        private readonly IVacationAbsencesService vacationAbsencesService;
        private readonly IPriceAdjustmentYoyService priceAdjustmentYoyService;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;

        public PartnerCostAndRevenueWorkflowService(IPartnerCostAndRevenueService partnerCostAndRevenueService, IRiskManagementService riskManagementService,
                        IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService, ICapitalChargeService capitalChargeService,
                        ISharedRepository sharedRepository, ILaborPricingService laborPricingService, IVacationAbsencesService vacationAbsencesService,
                        IPriceAdjustmentYoyService priceAdjustmentYoyService, IOtherPriceAdjustmentService otherPriceAdjustmentService)
        {
            this.partnerCostAndRevenueService = partnerCostAndRevenueService;
            this.riskManagementService = riskManagementService;
            this.sharedRepository = sharedRepository;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.laborPricingService = laborPricingService;
            this.vacationAbsencesService = vacationAbsencesService;
            this.priceAdjustmentYoyService = priceAdjustmentYoyService;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
        }

        public async Task ProcessPartnerCostAndRevenueSaving(string userName, PartnerCostAndRevenueDTO partnerCostAndRevenueDTO)
        {
            int pipSheetId = partnerCostAndRevenueDTO.PartnerCost[0].PipSheetId;
            await this.partnerCostAndRevenueService.SavePartnerCostAndRevenueData(userName, partnerCostAndRevenueDTO);

            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);

            // Risk Management
            if (pipSheetSaveStatus.RiskManagement == true)
            {
                RiskManagementCalcDTO riskManagementCalcDTO = await riskManagementService.CalculateRiskManagementData(pipSheetId, userName);
                await this.riskManagementService.SaveRiskManagement(userName, riskManagementCalcDTO);

                if (pipSheetSaveStatus.LaborPricing == true)
                {
                    LaborPricingDTO laborPricingDTO = await this.laborPricingService.CalculateLaborPricing(userName, pipSheetId);
                    await this.laborPricingService.SaveLaborPricingData(userName, laborPricingDTO);

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
                            LaborPricingDTO laborPricing = await this.laborPricingService.CalculateLaborPricing(userName, pipSheetId);
                            await this.laborPricingService.SaveLaborPricingData(userName, laborPricing);
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

                    // Other Price Adjustment
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
    }
}
