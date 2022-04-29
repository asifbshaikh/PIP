using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class ColaWorkflowService : IColaWorkflowService
    {
        private readonly IPriceAdjustmentYoyService priceAdjustmentYoyService;
        private readonly IVacationAbsencesService vacationAbsencesService;
        private readonly IRiskManagementService riskManagementService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ISharedRepository sharedRepository;
        private readonly ILaborPricingService laborPricingService;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;
        private readonly IExpenseAndAssetService expenseAndAssetService;

        public ColaWorkflowService(IPriceAdjustmentYoyService priceAdjustmentYoyService, IVacationAbsencesService vacationAbsencesService,
           IRiskManagementService riskManagementService, IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService,
           ICapitalChargeService capitalChargeService, ISharedRepository sharedRepository, ILaborPricingService laborPricingService,
           IOtherPriceAdjustmentService otherPriceAdjustmentService, IExpenseAndAssetService expenseAndAssetService
           )
        {
            this.priceAdjustmentYoyService = priceAdjustmentYoyService;
            this.vacationAbsencesService = vacationAbsencesService;
            this.riskManagementService = riskManagementService;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.sharedRepository = sharedRepository;
            this.laborPricingService = laborPricingService;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
            this.expenseAndAssetService = expenseAndAssetService;
        }

        public async Task ProcessPriceAdjustmentYoySaving(string userName, PriceAdjustmentDTO priceAdjustmentDTO)
        {
            int pipSheetId = priceAdjustmentDTO.PriceAdjustmentYoyDTO.PipSheetId;
            await this.priceAdjustmentYoyService.SavePriceAdjustmentYoy(userName, priceAdjustmentDTO);

            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);

            // Vacation Absences
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
                    PriceAdjustmentDTO priceAdjustment = await this.priceAdjustmentYoyService.GetPriceAdjustmentYoy(pipSheetId);
                    await this.priceAdjustmentYoyService.SavePriceAdjustmentYoy(userName, priceAdjustment);
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
