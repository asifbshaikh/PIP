using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class ProjectControlWorkflowService : IProjectControlWorkflowService
    {
        private readonly IPipSheetService pipSheetService;
        private readonly IEbitdaService ebitdaService;
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
        private readonly IPartnerCostAndRevenueService partnerCostAndRevenueService;
        private readonly IReimbursementAndSalesService reimbursementAndSalesService;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;

        public ProjectControlWorkflowService(IPipSheetService pipSheetService, IResourcePlanningService resourcePlanningService,
            IEbitdaService ebitdaService, ILaborPricingService laborPricingService, IVacationAbsencesService vacationAbsencesService,
            IRiskManagementService riskManagementService, IPriceAdjustmentYoyService priceAdjustmentYoyService, ISharedRepository sharedRepository,
            IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService, ICapitalChargeService capitalChargeService,
            IExpenseAndAssetService expenseAndAssetService, IPartnerCostAndRevenueService partnerCostAndRevenueService, IReimbursementAndSalesService reimbursementAndSalesService,
            IOtherPriceAdjustmentService otherPriceAdjustmentService)
        {
            this.pipSheetService = pipSheetService;
            this.ebitdaService = ebitdaService;
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
            this.partnerCostAndRevenueService = partnerCostAndRevenueService;
            this.reimbursementAndSalesService = reimbursementAndSalesService;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
        }

        public async Task ProcessProjectControlSave(string userName, ProjectControlDTO projectControlDTO)
        {
            int pipsheetId = projectControlDTO.PIPSheetListDTO[0].PIPSheetId;
            bool isDependentSaveNeeded = false;

            ProjectControlDTO projectControlData = await this.pipSheetService.GetProjectControlData(pipsheetId);
            bool IsAnyLocationDeleted = await pipSheetService.SaveProjectControlData(userName, projectControlDTO);

            if (projectControlData != null && projectControlData.PIPSheetListDTO.Count > 0)
            {
                if (projectControlData.PIPSheetListDTO[0].StartDate != projectControlDTO.PIPSheetListDTO[0].StartDate ||
                projectControlData.PIPSheetListDTO[0].EndDate != projectControlDTO.PIPSheetListDTO[0].EndDate ||
                projectControlData.PIPSheetListDTO[0].HolidayOption != projectControlDTO.PIPSheetListDTO[0].HolidayOption)
                {
                    isDependentSaveNeeded = true;
                }
                else
                {
                    if (IsAnyLocationDeleted)
                    {
                        isDependentSaveNeeded = true;
                    }
                    else
                    {
                        if (projectControlDTO.ProjectLocationListDTO.Count == projectControlData.ProjectLocationListDTO.Count)
                        {
                            foreach (ProjectLocationDTO projectLocationDTO in projectControlDTO.ProjectLocationListDTO)
                            {
                                ProjectLocationDTO locationDTO = projectControlData.ProjectLocationListDTO.FirstOrDefault(
                                    location => location.LocationId == projectLocationDTO.LocationId);
                                if (locationDTO.LocationId > 0)
                                {
                                    if (projectLocationDTO.HoursPerDay != locationDTO.HoursPerDay || projectLocationDTO.HoursPerMonth != locationDTO.HoursPerMonth)
                                    {
                                        isDependentSaveNeeded = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (isDependentSaveNeeded == true)
                {
                    await this.ProcessSaveDependency(userName, pipsheetId);
                }
            }
        }

        private async Task ProcessSaveDependency(string userName, int pipSheetId)
        {
            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);

            // Resource Planning
            if (pipSheetSaveStatus.ResourcePlanning == true)
            {
                ResourcePlanningSaveDependencyDTO resourcePlanningSaveDependencyDTO = await this.resourcePlanningService.GetResourcePlanningDataForSaveDependency(pipSheetId);
                IList<ResourcePlanningDTO> resourcePlanningDTO = this.resourcePlanningService.CreateResourcePlanningObject(resourcePlanningSaveDependencyDTO);
                if (resourcePlanningDTO.Count > 0)
                {
                    await this.resourcePlanningService.SaveResourcePlanningData(userName, resourcePlanningDTO);
                }
                else if (resourcePlanningDTO.Count == 0)
                {
                    await this.resourcePlanningService.SaveLocationDependentCalculations(userName, pipSheetId);
                }
            }

            // Ebita and Standard Overhead
            if (pipSheetSaveStatus.EbitdaAndOverhead)
            {
                List<EbitdaDTO> ebitdaDTO = await ebitdaService.GetEbitdaAndStandardOverhead(pipSheetId);
                List<EbitdaDTO> ebitdaDTOObj = ebitdaService.CreateEbitdaObject(pipSheetId, ebitdaDTO);
                await ebitdaService.UpdateEbitda(userName, ebitdaDTOObj);
            }

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

            // Expenses and Assets Save
            if (pipSheetSaveStatus.ExpensesAndAssets == true)
            {
                ExpenseAndAssetSaveDependencyDTO expenseAndAssetSaveDependencyDTO = await expenseAndAssetService.GetExpenseAndAssetForSaveDependency(pipSheetId);
                if (expenseAndAssetSaveDependencyDTO.DirectExpenseDTO.Count > 0)
                {
                    ExpenseAndAssetDTO expenseAndAssetDTOObj = await expenseAndAssetService.CreateExpenseAndAssetObject(expenseAndAssetSaveDependencyDTO);
                    await expenseAndAssetService.SaveExpenseAndAssetData(userName, expenseAndAssetDTOObj);
                }
            }

            // Partner Cost and Revenue
            if (pipSheetSaveStatus.PartnerCostAndRevenue)
            {
                PartnerCostAndRevenueDTO partnerCostAndRevenue = await this.partnerCostAndRevenueService.GetPartnerCostAndRevenue(pipSheetId);
                partnerCostAndRevenue = this.partnerCostAndRevenueService.ReAssignUIds(partnerCostAndRevenue);
                await this.partnerCostAndRevenueService.SavePartnerCostAndRevenueData(userName, partnerCostAndRevenue);
            }

            // Reimbursement and Sales Discount
            if (pipSheetSaveStatus.ReimbursementAndSales)
            {
                ReimbursementAndSalesDTO reimbursementAndSales = await this.reimbursementAndSalesService.GetReimbursementAndSalesDetails(pipSheetId);
                reimbursementAndSales = this.reimbursementAndSalesService.ReAssignUIds(reimbursementAndSales);
                await this.reimbursementAndSalesService.SaveReimbursementAndSalesDetails(userName, reimbursementAndSales);
            }

            // Other Price Adjustments
            if (pipSheetSaveStatus.OtherPriceAdjustment)
            {
                OtherPriceAdjustmentMainDTO otherPriceAdjustment = await this.otherPriceAdjustmentService.GetOtherPriceAdjustment(pipSheetId);
                otherPriceAdjustment = this.otherPriceAdjustmentService.ReAssignUIds(otherPriceAdjustment);
                await this.otherPriceAdjustmentService.SaveOtherPriceAdjustmentData(userName, otherPriceAdjustment);
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

                // Expenses and Assets Save
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
