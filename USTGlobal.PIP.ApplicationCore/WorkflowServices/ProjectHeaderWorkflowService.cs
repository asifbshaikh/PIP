using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class ProjectHeaderWorkflowService : IProjectHeaderWorkflowService
    {
        private readonly IProjectHeaderService projectHeaderService;
        private readonly IPipSheetService projectControlService;
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
        private readonly IEbitdaService ebitdaService;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;

        public ProjectHeaderWorkflowService(IProjectHeaderService projectHeaderService, IPipSheetService projectControlService, ISharedRepository sharedRepository,
            IVacationAbsencesService vacationAbsencesService, IResourcePlanningService resourcePlanningService, ICapitalChargeService capitalChargeService,
            IRiskManagementService riskManagementService, IPriceAdjustmentYoyService priceAdjustmentYoyService, IClientPriceService clientPriceService,
            IFixBidAndMarginService fixbidAndMarginService, ILaborPricingService laborPricingService, IExpenseAndAssetService expenseAndAssetService, IEbitdaService ebitdaService,
            IOtherPriceAdjustmentService otherPriceAdjustmentService)
        {
            this.projectHeaderService = projectHeaderService;
            this.projectControlService = projectControlService;
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
            this.ebitdaService = ebitdaService;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
        }

        public async Task<RouteParamDTO> ProcessProjectHeaderSave(string userName, ProjectHeaderDTO projectHeader)
        {
            ProjectHeaderCurrencyDTO projectHeaderCurrencyDTO = await this.projectHeaderService.GetProjectHeaderData(projectHeader.ProjectId, projectHeader.PIPSheetId);
            RouteParamDTO routeParamDTO = await this.projectHeaderService.SaveProjectHeaderData(userName, projectHeader);
            bool isDependencySaveNeeded = false;

            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(projectHeader.PIPSheetId);

            if (projectHeaderCurrencyDTO.ProjectHeader != null && routeParamDTO.ErrorCode != -1)
            {
                if (projectHeader.CurrencyId != projectHeaderCurrencyDTO.ProjectHeader.CurrencyId)
                {
                    isDependencySaveNeeded = true;
                }

                if (projectHeader.AccountId != projectHeaderCurrencyDTO.ProjectHeader.AccountId)
                {
                    isDependencySaveNeeded = true;
                }

                if (projectHeaderCurrencyDTO.ProjectHeader.ProjectDeliveryTypeId != projectHeader.ProjectDeliveryTypeId
                        || projectHeaderCurrencyDTO.ProjectHeader.ProjectBillingTypeId != projectHeader.ProjectBillingTypeId)
                {
                    // Project Control Save
                    ProjectControlDTO projectControl = await this.projectControlService.GetProjectControlData(projectHeader.PIPSheetId);
                    if (projectControl != null && projectControl.PIPSheetListDTO.Count > 0)
                    {
                        projectControl.PIPSheetListDTO[0].MilestoneGroupId = projectControl.PIPSheetListDTO[0].MilestoneGroupId == -1 ? null : projectControl.PIPSheetListDTO[0].MilestoneGroupId;
                        projectControl = await this.projectControlService.ReAssignHoursPerDayHoursPerMonth(projectHeader.ProjectId, projectControl);
                        await this.projectControlService.SaveProjectControlData(userName, projectControl);
                    }

                    // Resource Planning
                    if (pipSheetSaveStatus.ResourcePlanning == true)
                    {
                        ResourcePlanningSaveDependencyDTO resourcePlanningSaveDependencyDTO = await this.resourcePlanningService.GetResourcePlanningDataForSaveDependency(projectHeader.PIPSheetId);
                        IList<ResourcePlanningDTO> resourcePlanningDTO = this.resourcePlanningService.CreateResourcePlanningObject(resourcePlanningSaveDependencyDTO);
                        await this.resourcePlanningService.SaveResourcePlanningData(userName, resourcePlanningDTO);
                    }
                    isDependencySaveNeeded = true;
                }

                if (isDependencySaveNeeded == true || projectHeader.IsFromReplicate == true)
                {
                    await ProcessSaveDependency(projectHeader.PIPSheetId, userName, pipSheetSaveStatus);
                }
            }
            return routeParamDTO;
        }

        public async Task<ProjectHeaderCurrencyDTO> GetProjectHeaderData(int projectId, int pipsheetId)
        {
            return await this.projectHeaderService.GetProjectHeaderData(projectId, pipsheetId);
        }

        private async Task ProcessSaveDependency(int pipSheetId, string userName, PipSheetSaveStatusDTO pipSheetSaveStatus)
        {
            // Ebitda Save
            List<EbitdaDTO> ebitdaDTO = await ebitdaService.GetEbitdaAndStandardOverhead(pipSheetId);
            List<EbitdaDTO> ebitdaDTOObj = ebitdaService.CreateEbitdaObject(pipSheetId, ebitdaDTO);
            await ebitdaService.UpdateEbitda(userName, ebitdaDTOObj);

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

            // Other Price Adjustment
            if (pipSheetSaveStatus.OtherPriceAdjustment == true)
            {
                OtherPriceAdjustmentMainDTO priceAdjustment = await this.otherPriceAdjustmentService.GetOtherPriceAdjustment(pipSheetId);
                await this.otherPriceAdjustmentService.SaveOtherPriceAdjustmentData(userName, priceAdjustment);
            }

            // Capital Charge 
            if (pipSheetSaveStatus.CapitalCharge == true)
            {
                CapitalChargeResultSetDTO capitalChargeResultSetDTO = await this.capitalChargeService.CalculateCapitalCharges(pipSheetId);
                await this.capitalChargeService.SaveCapitalCharge(userName, capitalChargeResultSetDTO);

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
