using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class EbitdaWorkflowService : IEbitdaWorkflowService
    {
        private readonly IEbitdaService ebitdaService;
        private readonly ILaborPricingService laborPricingService;
        private readonly IExpenseAndAssetService expenseAndAssetService;
        private readonly ISharedRepository sharedRepository;
        private readonly IRiskManagementService riskManagementService;
        private readonly IVacationAbsencesService vacationAbsencesService;
        private readonly IPriceAdjustmentYoyService priceAdjustmentYoyService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly ICapitalChargeService capitalChargeService;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;

        public EbitdaWorkflowService(ILaborPricingService laborPricingService, IExpenseAndAssetService expenseAndAssetService, IVacationAbsencesService vacationAbsencesService,
            IRiskManagementService riskManagementService, IPriceAdjustmentYoyService priceAdjustmentYoyService, ISharedRepository sharedRepository,
            IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService, ICapitalChargeService capitalChargeService,
            IEbitdaService ebitdaService, IOtherPriceAdjustmentService otherPriceAdjustmentService
            )
        {
            this.expenseAndAssetService = expenseAndAssetService;
            this.laborPricingService = laborPricingService;
            this.sharedRepository = sharedRepository;
            this.ebitdaService = ebitdaService;
            this.vacationAbsencesService = vacationAbsencesService;
            this.riskManagementService = riskManagementService;
            this.priceAdjustmentYoyService = priceAdjustmentYoyService;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
        }

        public async Task ProcessEbitdaSaving(string userName, List<EbitdaDTO> ebitdadata)
        {
            int pipSheetId = ebitdadata[0].PipSheetId;
            await this.ebitdaService.UpdateEbitda(userName, ebitdadata);

            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);

            // Labor Pricing Dependent Save
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

            // Expenses and Assets
            if (pipSheetSaveStatus.ExpensesAndAssets == true)
            {
                ExpenseAndAssetSaveDependencyDTO expenseAndAssetSaveDependencyDTO = await expenseAndAssetService.GetExpenseAndAssetForSaveDependency(pipSheetId);
                if (expenseAndAssetSaveDependencyDTO.DirectExpenseDTO.Count > 0)
                {
                    ExpenseAndAssetDTO expenseAndAssetDTOObj = await expenseAndAssetService.CreateExpenseAndAssetObject(expenseAndAssetSaveDependencyDTO);
                    await expenseAndAssetService.SaveExpenseAndAssetData(userName, expenseAndAssetDTOObj);
                }
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
