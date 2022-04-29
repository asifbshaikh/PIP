using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class PriceAdjustmentYoyRepository : IPriceAdjustmentYoyRepository
    {
        private readonly PipContext pipContext;
        public PriceAdjustmentYoyRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<PriceAdjustmentDTO> GetPriceAdjustmentYoy(int pipSheetId)
        {
            PriceAdjustmentDTO priceAdjustmentDTO = new PriceAdjustmentDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetPriceAdjustmentYOY")
                            .WithSqlParam("@PIPSheetId", pipSheetId)
                            .ExecuteStoredProcAsync((priceAdjustmentResultSet) =>
                            {
                                ProjectDurationDTO projectDurationDTO = priceAdjustmentResultSet.ReadToList<ProjectDurationDTO>().FirstOrDefault();
                                priceAdjustmentDTO.ProjectDurationDTO = projectDurationDTO;
                                priceAdjustmentResultSet.NextResult();

                                PriceAdjustmentYoyDTO priceAdjustmentYoyDTO = priceAdjustmentResultSet.ReadToList<PriceAdjustmentYoyDTO>().FirstOrDefault();
                                priceAdjustmentDTO.PriceAdjustmentYoyDTO = priceAdjustmentYoyDTO;
                                priceAdjustmentResultSet.NextResult();

                                List<ColaDTO> colaDTO = priceAdjustmentResultSet.ReadToList<ColaDTO>().ToList();
                                priceAdjustmentDTO.ColaDTO = colaDTO;
                            });
            return priceAdjustmentDTO;
        }

        public async Task<IList<PriceAdjustmentResourceDTO>> GetLaborPricingDataForPriceAdjustment(int pipSheetId)
        {
            IList<PriceAdjustmentResourceDTO> laborPricingData = new List<PriceAdjustmentResourceDTO>();

            await pipContext.LoadStoredProc("dbo.sp_GetLaborPricingDataForPriceAdjustment")
             .WithSqlParam("@PIPSheetId", pipSheetId)
             .ExecuteStoredProcAsync((laborPricingResultSet) =>
             {
                 List<PriceAdjustmentResourceDTO> resourceLaborPricingResultSet = laborPricingResultSet
                 .ReadToList<PriceAdjustmentResourceDTO>().ToList();
                 laborPricingResultSet.NextResult();

                 List<ProjectResourcePeriodDTO> projectResourcePeriods = laborPricingResultSet.ReadToList<ProjectResourcePeriodDTO>().ToList();

                 foreach (PriceAdjustmentResourceDTO resource in resourceLaborPricingResultSet)
                 {
                     laborPricingData.Add(new PriceAdjustmentResourceDTO
                     {
                         ProjectResourceId = resource.ProjectResourceId,
                         PipSheetId = resource.PipSheetId,
                         LocationId = resource.LocationId,
                         UtilizationType = resource.UtilizationType,
                         Rate = resource.Rate,
                         Cost = resource.Cost,
                         RatePerHour = resource.RatePerHour,
                         Yr1PerHour = resource.Yr1PerHour,
                         Margin = resource.Margin,
                         CappedCost = resource.CappedCost,
                         TotalRevenue = resource.TotalRevenue,
                         projectResourcePeriodDTO = projectResourcePeriods
                                                     .Where(resourcePeriods => resourcePeriods.ProjectResourceId == resource.ProjectResourceId)
                                                     .Select(resourcePeriods => resourcePeriods).ToList(),
                     });
                 }
             });
            return laborPricingData;
        }

        public async Task SavePriceAdjustmentYoy(string userName, PriceAdjustmentDTO priceAdjustmentDTO, IList<ProjectResourcePeriodDTO> projectResourcePeriodDTO,
            IList<ResourceLaborPricingSubDTO> resourceLaborPricingDTO, IList<ProjectPeriodTotalDTO> projectPeriodTotals)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SavePriceAdjustmentYOY {0}, {1}, {2}, " +
                " {3}, {4}, {5}, {6}, {7}",
                userName,
                priceAdjustmentDTO.PriceAdjustmentYoyDTO.PipSheetId,
                priceAdjustmentDTO.PriceAdjustmentYoyDTO.TriggerDate,
                priceAdjustmentDTO.PriceAdjustmentYoyDTO.EffectiveDate,     
                new SqlParameter("@InputProjectLocationColaPercentData", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(priceAdjustmentDTO.ColaDTO),
                    TypeName = "dbo.ProjectLocationColaPercent"
                }
                , new SqlParameter("@InputResourceLaborPricingData", SqlDbType.Structured)
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
    }
}
