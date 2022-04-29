using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class LaborPricingRepository : ILaborPricingRepository
    {
        private readonly PipContext pipContext;

        public LaborPricingRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<LaborPricingDTO> GetLaborPricingData(int pipSheetId)
        {
            DbParameter outputParam = null;
            LaborPricingDTO laborPricingData = new LaborPricingDTO();
            laborPricingData.resourceLaborPricingDTOs = new List<ResourceLaborPricingDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetLaborPricingData")
             .WithSqlParam("@PIPSheetId", pipSheetId)
             .WithSqlParam("@isDeliveryTypeRestricted", (dbParam) =>
             {
                 dbParam.Direction = ParameterDirection.Output;
                 dbParam.DbType = DbType.Byte;
                 outputParam = dbParam;
             })
             .ExecuteStoredProcAsync((laborPricingResultSet) =>
             {
                 MarginDTO marginResultSet = laborPricingResultSet.ReadToList<MarginDTO>().FirstOrDefault();
                 laborPricingData.marginDTO = marginResultSet;
                 laborPricingResultSet.NextResult();

                 List<ResourceLaborPricingDTO> resourceLaborPricingResultSet = laborPricingResultSet
                 .ReadToList<ResourceLaborPricingDTO>().ToList();
                 laborPricingResultSet.NextResult();

                 List<ProjectResourcePeriodDTO> projectResourcePeriods = laborPricingResultSet.ReadToList<ProjectResourcePeriodDTO>().ToList();
                 laborPricingResultSet.NextResult();

                 List<ProjectPeriodDTO> projectPeriodResultSet = laborPricingResultSet
                .ReadToList<ProjectPeriodDTO>().ToList();
                 laborPricingData.projectPeriodDTO = projectPeriodResultSet;

                 foreach (ResourceLaborPricingDTO resource in resourceLaborPricingResultSet)
                 {
                     laborPricingData.resourceLaborPricingDTOs.Add(new ResourceLaborPricingDTO
                     {
                         ProjectResourceId = resource.ProjectResourceId,
                         PipSheetId = resource.PipSheetId,
                         ResourceId = resource.ResourceId,
                         LocationId = resource.LocationId,
                         MilestoneName = resource.MilestoneName,
                         LocationName = resource.LocationName,
                         Name = resource.Name,
                         StandardCostRate = resource.StandardCostRate,
                         Percent = resource.Percent,
                         UtilizationType = resource.UtilizationType,
                         Rate = resource.Rate,
                         Cost = resource.Cost,
                         RatePerHour = resource.RatePerHour,
                         Yr1PerHour = resource.Yr1PerHour,
                         Margin = resource.Margin,
                         CappedCost = resource.CappedCost,
                         TotalRevenue = resource.TotalRevenue,
                         TotalHoursPerResource = resource.TotalHoursPerResource,
                         CostHrsPerResource = resource.CostHrsPerResource,
                         Alias = resource.Alias,
                         NonBillableCategoryId = resource.NonBillableCategoryId,
                         ResourceServiceLineId =  resource.ResourceServiceLineId,
                         projectResourcePeriodDTO = projectResourcePeriods
                                                     .Where(resourcePeriods => resourcePeriods.ProjectResourceId == resource.ProjectResourceId)
                                                     .Select(resourcePeriods => resourcePeriods).ToList(),
                         GradeClientRole = resource.GradeClientRole,
                         CreatedBy = resource.CreatedBy,
                         UpdatedBy = resource.UpdatedBy
                     });
                 }
             });

            bool opParamValue = Convert.ToBoolean(outputParam?.Value);
            laborPricingData.isDeliveryTypeRestricted = opParamValue;
            return laborPricingData;
        }

        public async Task SaveLaborPricingData(string userName, LaborPricingDTO laborPricingDTO, IList<ProjectResourcePeriodDTO> projectResourcePeriodDTO,
            IList<ResourceLaborPricingSubDTO> resourceLaborPricingDTO, IList<ProjectPeriodTotalDTO> projectPeriodTotals)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveLaborPricingData {0}, {1}, {2}, " +
                " {3}, {4}, {5}, {6}, {7}",
                userName,
                laborPricingDTO.marginDTO.PipSheetId,
                laborPricingDTO.marginDTO.IsMarginSet,
                laborPricingDTO.marginDTO.MarginPercent,
                laborPricingDTO.marginDTO.Which,
                new SqlParameter("@InputResourceLaborPricingData", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(resourceLaborPricingDTO),
                    TypeName = "dbo.ResourceLaborPricing"
                }
                , new SqlParameter("@InputResourceLaborPricingPeriodDetails", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(projectResourcePeriodDTO),
                    TypeName = "dbo.ProjectResourcePeriod"
                }
                , new SqlParameter("@InputPeriodTotals", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(projectPeriodTotals),
                    TypeName = "dbo.ProjectPeriodTotal"
                }
                );
            await pipContext.SaveChangesAsync();
        }

        public async Task<LaborPricingBackgroundCalcParentDTO> CalculateBackgroundFields(int pipSheetId)
        {
            LaborPricingBackgroundCalcParentDTO laborPricingBackgroundCalcParentDTO = new LaborPricingBackgroundCalcParentDTO();
            MarginDTO marginDTO = new MarginDTO();
            LaborPricingBackgroundCalculationDTO laborPricingBackgroundCalculationDTO = new LaborPricingBackgroundCalculationDTO();
            IList<LaborPricingEbidtaCalculationDTO> laborPricingEbidtaCalculationDTO = null;
            IList<ResourcePlanningPipSheetDTO> resourcePlanningPipSheetDTO = null;
            IList<HolidayDTO> holidayList = null;
            ProjectPeriodTotalDTO projectPeriodTotal = null;

            await pipContext.LoadStoredProc("dbo.sp_CalculateLaborPricingBackgroundFields")
                 .WithSqlParam("@PIPSheetId", pipSheetId)
                   .ExecuteStoredProcAsync((result) =>
                   {
                       resourcePlanningPipSheetDTO = result.ReadToList<ResourcePlanningPipSheetDTO>();
                       result.NextResult();
                       laborPricingEbidtaCalculationDTO = result.ReadToList<LaborPricingEbidtaCalculationDTO>();
                       result.NextResult();
                       holidayList = result.ReadToList<HolidayDTO>();
                       result.NextResult();
                       marginDTO = result.ReadToList<MarginDTO>().FirstOrDefault();
                       result.NextResult();
                       laborPricingBackgroundCalculationDTO = result.ReadToList<LaborPricingBackgroundCalculationDTO>().FirstOrDefault();
                       result.NextResult();
                       laborPricingBackgroundCalcParentDTO.TotalAssesedRiskOverrun = Convert.ToDecimal(result.ReadToValue<decimal>());
                       result.NextResult();
                       laborPricingBackgroundCalcParentDTO.PaymentLag = Convert.ToDecimal(result.ReadToValue<decimal>());
                       result.NextResult();
                       laborPricingBackgroundCalcParentDTO.FeesAtRisk = Convert.ToDecimal(result.ReadToValue<decimal>());
                       result.NextResult();
                       projectPeriodTotal = result.ReadToList<ProjectPeriodTotalDTO>().FirstOrDefault();
                       result.NextResult();
                       laborPricingBackgroundCalcParentDTO.RiskManagementDTO = result.ReadToList<RiskManagementDTO>().FirstOrDefault();
                       result.NextResult();
                       laborPricingBackgroundCalcParentDTO.TotalPartnerCost = Convert.ToDecimal(result.ReadToValue<decimal>());
                       result.NextResult();
                       laborPricingBackgroundCalcParentDTO.TotalDirectExpense = Convert.ToDecimal(result.ReadToValue<decimal>());
                   });

            laborPricingBackgroundCalcParentDTO.LaborPricingEbidtaCalculationDTO = laborPricingEbidtaCalculationDTO;
            laborPricingBackgroundCalcParentDTO.LaborPricingBackgroundCalculationDTO = laborPricingBackgroundCalculationDTO;
            laborPricingBackgroundCalcParentDTO.ResourcePlanningPipSheetDTO = resourcePlanningPipSheetDTO;
            laborPricingBackgroundCalcParentDTO.HolidayList = holidayList;
            laborPricingBackgroundCalcParentDTO.MarginDTO = marginDTO;
            laborPricingBackgroundCalcParentDTO.ProjectPeriodTotalDTO = projectPeriodTotal;

            return laborPricingBackgroundCalcParentDTO;

        }

        public async Task<LaborPricingBackgroundCalcParentDTO> GetResourceCostCalculationFields(int pipSheetId)
        {
            LaborPricingBackgroundCalcParentDTO laborPricing = new LaborPricingBackgroundCalcParentDTO();
            
            await pipContext.LoadStoredProc("dbo.sp_CalculateLaborPricingBackgroundFields")
                 .WithSqlParam("@PipSheetId", pipSheetId)
                   .ExecuteStoredProcAsync((result) =>
                   {
                       laborPricing.ResourcePlanningPipSheetDTO = result.ReadToList<ResourcePlanningPipSheetDTO>();
                       result.NextResult();
                       laborPricing.LocationEbitdaSeatCost = result.ReadToList<LocationEbitdaSeatCostDTO>();
                       result.NextResult();
                       laborPricing.HolidayList = result.ReadToList<HolidayDTO>();
                   });

            return laborPricing;
        }
    }
}
