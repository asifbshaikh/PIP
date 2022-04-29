using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class ResourcePlanningWorkflowService : IResourcePlanningWorkflowService
    {
        private readonly IResourcePlanningService resourcePlanningService;
        private readonly ILaborPricingService laborPricingService;
        private readonly IRiskManagementService riskManagementService;
        private readonly IVacationAbsencesService vacationAbsencesService;
        private readonly IPriceAdjustmentYoyService priceAdjustmentYoyService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ISharedRepository sharedRepository;
        private readonly IExpenseAndAssetService expenseAndAssetService;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;

        public ResourcePlanningWorkflowService(IResourcePlanningService resourcePlanningService, ILaborPricingService laborPricingService,
            IRiskManagementService riskManagementService, IPriceAdjustmentYoyService priceAdjustmentYoyService, ISharedRepository sharedRepository,
            IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService, ICapitalChargeService capitalChargeService,
            IVacationAbsencesService vacationAbsencesService, IExpenseAndAssetService expenseAndAssetService, IOtherPriceAdjustmentService otherPriceAdjustmentService)
        {
            this.resourcePlanningService = resourcePlanningService;
            this.laborPricingService = laborPricingService;
            this.vacationAbsencesService = vacationAbsencesService;
            this.riskManagementService = riskManagementService;
            this.priceAdjustmentYoyService = priceAdjustmentYoyService;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.sharedRepository = sharedRepository;
            this.expenseAndAssetService = expenseAndAssetService;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
        }

        public async Task ProcessResourcePlanningSave(string userName, IList<ResourcePlanningDTO> resourcePlanningDTO)
        {
            decimal clientTotalCostHours = 0;
            decimal clientTotalStaffHours = 0;
            decimal dbTotalCostHours = 0;
            decimal dbTotalStaffHours = 0;
            bool isUSTRoleIdChanged = false;
            int pipSheetId = resourcePlanningDTO[0].PipSheetId;
            int resourceCount = 0;

            ResourcePlanningMainDTO resourcePlanningGETDTO = await this.resourcePlanningService.GetResourcePlanningData(pipSheetId);
            foreach (var data in resourcePlanningDTO)
            {
                if (!data.IsDeleted)
                    resourceCount++;
            }
            // Save Resource Planning
            await this.resourcePlanningService.SaveResourcePlanningData(userName, resourcePlanningDTO);
            if (resourcePlanningGETDTO.Resources != null && resourcePlanningGETDTO.Resources.Count > 0)
            {
                // Calculating Totals from Client Object
                for (int i = 0; i < resourcePlanningDTO.Count; i++)
                {
                    clientTotalCostHours += (resourcePlanningDTO[i].CostHrsPerResource ?? 0);
                    clientTotalStaffHours += (resourcePlanningDTO[i].TotalhoursPerResource ?? 0);
                }

                // Calculating Totals from DB Object
                for (int i = 0; i < resourcePlanningGETDTO.Resources.Count; i++)
                {
                    dbTotalCostHours += (resourcePlanningGETDTO.Resources[i].CostHrsPerResource ?? 0);
                    dbTotalStaffHours += (resourcePlanningGETDTO.Resources[i].TotalhoursPerResource ?? 0);
                }

                // Check if any USTResourceId or Markup Changed was changed
                if (resourcePlanningGETDTO.Resources.Count == resourcePlanningDTO.Count)
                {
                    for (int i = 0; i < resourcePlanningDTO.Count; i++)
                    {
                        if ((resourcePlanningGETDTO.Resources[i].ResourceId != resourcePlanningDTO[i].ResourceId) ||
                            (resourcePlanningGETDTO.Resources[i].MarkupId != resourcePlanningDTO[i].MarkupId))
                        {
                            isUSTRoleIdChanged = true;
                            break;
                        }
                    }
                }

                // Condition to check if something is changed
                if (resourceCount != resourcePlanningGETDTO.Resources.Count
                    || clientTotalCostHours != dbTotalCostHours
                    || clientTotalStaffHours != dbTotalStaffHours
                    || isUSTRoleIdChanged == true)
                {
                    // Bring PipSheetSaveStatus DTO here
                    PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);

                    // Labor Pricing Dependent Save
                    if (pipSheetSaveStatus.LaborPricing)
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
            // Else Block for First Time Save
            else
            {
                // Labor Pricing Dependent Save
                LaborPricingDTO laborPricingDTO = await this.laborPricingService.CalculateLaborPricing(userName, pipSheetId);
                await this.laborPricingService.SaveLaborPricingData(userName, laborPricingDTO);

                // Expenses and Assets
                ExpenseAndAssetSaveDependencyDTO expenseAndAssetSaveDependencyDTO = await expenseAndAssetService.GetExpenseAndAssetForSaveDependency(pipSheetId);
                ExpenseAndAssetDTO expenseAndAssetDTOObj = await expenseAndAssetService.CreateExpenseAndAssetObject(expenseAndAssetSaveDependencyDTO);
                await expenseAndAssetService.SaveExpenseAndAssetData(userName, expenseAndAssetDTOObj);

                // Risk Management
                RiskManagementCalcDTO RiskManagementCalcDTO = await this.riskManagementService.CalculateRiskManagementData(pipSheetId, userName);
                await this.riskManagementService.SaveRiskManagement(userName, RiskManagementCalcDTO);

                // Fixed Bid and Margin
                await this.fixbidAndMarginService.CalculateAndSaveFixBidData(pipSheetId, userName);

                // Client Price
                ClientPriceMainDTO clientPriceMainDTO = await this.clientPriceService.CalculateTotalClientPrice(pipSheetId, userName);
                await this.clientPriceService.SaveClientPriceData(clientPriceMainDTO, userName);

                // Capital Charge
                CapitalChargeResultSetDTO capitalChargeResultSetDTO = await this.capitalChargeService.CalculateCapitalCharges(pipSheetId);
                await this.capitalChargeService.SaveCapitalCharge(userName, capitalChargeResultSetDTO);
            }
        }
    }
}
