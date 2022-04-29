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
    public class ResourcePlanningRepository : IResourcePlanningRepository
    {
        private readonly PipContext pipContext;

        public ResourcePlanningRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<LocationDTO>> GetProjectLocations(int pipSheetId)
        {
            return await (from pl in this.pipContext.ProjectLocation
                          join l in this.pipContext.Location on pl.LocationId equals l.LocationId
                          where pl.PIPSheetId == pipSheetId && l.IsActive == true && l.EndDate == null
                          select new LocationDTO
                          {
                              LocationId = pl.LocationId,
                              LocationName = l.LocationName,
                              HoursPerDay = pl.HoursPerDay,
                              HoursPerMonth = pl.HoursPerMonth,
                          }).ToListAsync();
        }

        public async Task<List<ProjectMilestoneDTO>> GetProjectMilestones(int pipSheetId)
        {
            return await (from pm in this.pipContext.ProjectMilestone
                          join pc in this.pipContext.ProjectControl on pm.MilestoneGroupId equals pc.MilestoneGroupId
                          where pm.PipSheetId == pipSheetId
                          && pc.PipSheetId == pipSheetId
                          && pm.IsChecked == true
                          select new ProjectMilestoneDTO
                          {
                              ProjectMilestoneId = pm.ProjectMilestoneId,
                              MilestoneId = pm.MilestoneId,
                              PipSheetId = pm.PipSheetId,
                              MilestoneName = pm.MilestoneName,
                              MilestoneGroupId = pm.MilestoneGroupId,
                              IsChecked = pm.IsChecked
                          }).ToListAsync();
        }

        public async Task SaveResourcePlanningData(string userName, IList<ProjectResourceDTO> projectResource, IList<ProjectResourcePeriodDTO> projectPeriods,
            IList<ProjectPeriodTotalDTO> projectPeriodTotals, decimal inflatedCostHours)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveResourcePlanningData {0}, {1}, {2}, {3}, {4} ",
                userName,
                new SqlParameter("@InputProjectResources", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(projectResource),
                    TypeName = "dbo.ProjectResource"
                },
                new SqlParameter("@InputProjectPeriods", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(projectPeriods),
                    TypeName = "dbo.ProjectResourcePeriod"
                },
                new SqlParameter("@InputPeriodTotals", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(projectPeriodTotals),
                    TypeName = "dbo.ProjectPeriodTotal"
                },
                inflatedCostHours);
            await pipContext.SaveChangesAsync();
        }

        public async Task<ResourcePlanningMainDTO> GetResourcePlanningData(int pipSheetId)
        {
            ResourcePlanningMainDTO resourcePlanningMainDTO = new ResourcePlanningMainDTO();
            resourcePlanningMainDTO.Resources = new List<ResourcePlanningDTO>();

            await pipContext.LoadStoredProc("dbo.sp_GetResourcePlanningData")
               .WithSqlParam("@PIPSheetId", pipSheetId)
               .ExecuteStoredProcAsync((resourcePlanningResultSet) =>
               {
                   var projectPeriods = resourcePlanningResultSet.ReadToList<ProjectPeriodDTO>();
                   resourcePlanningMainDTO.ProjectPeriods = projectPeriods;
                   resourcePlanningResultSet.NextResult();

                   var projectResources = resourcePlanningResultSet.ReadToList<ProjectResourceDTO>();
                   resourcePlanningResultSet.NextResult();

                   var projectResourcePeriods = resourcePlanningResultSet.ReadToList<ProjectResourcePeriodDTO>();
                   ResourcePlanningDTO resourcePlanningDTO = new ResourcePlanningDTO();
                   foreach (var resource in projectResources)
                   {
                       resourcePlanningDTO.ProjectResourceId = resource.ProjectResourceId;
                       resourcePlanningDTO.PipSheetId = resource.PipSheetId;
                       resourcePlanningDTO.LocationId = resource.LocationId;
                       resourcePlanningDTO.ResourceGroupId = resource.ResourceGroupId;
                       resourcePlanningDTO.ResourceId = resource.ResourceId;
                       resourcePlanningDTO.UtilizationType = resource.UtilizationType;
                       resourcePlanningDTO.MilestoneId = resource.MilestoneId;
                       resourcePlanningDTO.MarkupId = resource.MarkupId;
                       resourcePlanningDTO.Alias = resource.Alias;
                       resourcePlanningDTO.CreatedBy = resource.CreatedBy;
                       resourcePlanningDTO.UpdatedBy = resource.UpdatedBy;
                       resourcePlanningDTO.TotalhoursPerResource = resource.TotalHoursPerResource;
                       resourcePlanningDTO.CostHrsPerResource = resource.CostHrsPerResource;
                       resourcePlanningDTO.ResourceServiceLineId = resource.ResourceServiceLineId;
                       resourcePlanningDTO.ClientRole = resource.ClientRole;
                       resourcePlanningDTO.ProjectPeriod = GetProjectResourcePeriods(resource.ProjectResourceId, projectResourcePeriods);

                       resourcePlanningMainDTO.Resources.Add(resourcePlanningDTO);
                       resourcePlanningDTO = new ResourcePlanningDTO();
                   }
                   resourcePlanningResultSet.NextResult();

                   resourcePlanningMainDTO.ResourcePlanMasterData = resourcePlanningResultSet.ReadToList<ResourcePlanningPipSheetDTO>();
               });

            return resourcePlanningMainDTO;
        }

        public async Task<ResourcePlanningSaveDependencyDTO> GetResourcePlanningDataForSaveDependency(int pipSheetId)
        {
            ResourcePlanningSaveDependencyDTO resourcePlanningSaveDependencyDTO = new ResourcePlanningSaveDependencyDTO();
            resourcePlanningSaveDependencyDTO.Resources = new List<ResourcePlanningDTO>();

            await pipContext.LoadStoredProc("dbo.sp_GetResourcePlanningData")
               .WithSqlParam("@PIPSheetId", pipSheetId)
               .ExecuteStoredProcAsync((resourcePlanningResultSet) =>
               {
                   var projectPeriods = resourcePlanningResultSet.ReadToList<ProjectPeriodDTO>();
                   resourcePlanningSaveDependencyDTO.ProjectPeriods = projectPeriods;
                   resourcePlanningResultSet.NextResult();

                   var projectResources = resourcePlanningResultSet.ReadToList<ProjectResourceDTO>();
                   resourcePlanningResultSet.NextResult();

                   var projectResourcePeriods = resourcePlanningResultSet.ReadToList<ProjectResourcePeriodDTO>();
                   ResourcePlanningDTO resourcePlanningDTO = new ResourcePlanningDTO();
                   foreach (var resource in projectResources)
                   {
                       resourcePlanningDTO.ProjectResourceId = resource.ProjectResourceId;
                       resourcePlanningDTO.PipSheetId = resource.PipSheetId;
                       resourcePlanningDTO.LocationId = resource.LocationId;
                       resourcePlanningDTO.ResourceGroupId = resource.ResourceGroupId;
                       resourcePlanningDTO.ResourceId = resource.ResourceId;
                       resourcePlanningDTO.UtilizationType = resource.UtilizationType;
                       resourcePlanningDTO.MilestoneId = resource.MilestoneId;
                       resourcePlanningDTO.MarkupId = resource.MarkupId;
                       resourcePlanningDTO.Alias = resource.Alias;
                       resourcePlanningDTO.CreatedBy = resource.CreatedBy;
                       resourcePlanningDTO.UpdatedBy = resource.UpdatedBy;
                       resourcePlanningDTO.TotalhoursPerResource = resource.TotalHoursPerResource;
                       resourcePlanningDTO.CostHrsPerResource = resource.CostHrsPerResource;
                       resourcePlanningDTO.ResourceServiceLineId = resource.ResourceServiceLineId;

                       resourcePlanningDTO.ProjectPeriod = GetProjectResourcePeriods(resource.ProjectResourceId, projectResourcePeriods);

                       resourcePlanningSaveDependencyDTO.Resources.Add(resourcePlanningDTO);
                       resourcePlanningDTO = new ResourcePlanningDTO();
                   }
                   resourcePlanningResultSet.NextResult();

                   resourcePlanningSaveDependencyDTO.ResourcePlanMasterData = resourcePlanningResultSet.ReadToList<ResourcePlanningPipSheetDTO>();
                   resourcePlanningResultSet.NextResult();

                   resourcePlanningSaveDependencyDTO.ResourceLocationDTO = resourcePlanningResultSet.ReadToList<LocationDTO>();
                   resourcePlanningResultSet.NextResult();

                   resourcePlanningSaveDependencyDTO.ResourceHolidayDTO = resourcePlanningResultSet.ReadToList<HolidayDTO>();
                   resourcePlanningResultSet.NextResult();

                   resourcePlanningSaveDependencyDTO.ProjectBillingTypeDTO = resourcePlanningResultSet.ReadToList<ProjectBillingTypeDTO>();
               });
            return resourcePlanningSaveDependencyDTO;
        }

        private IList<ProjectResourcePeriodDTO> GetProjectResourcePeriods(int projectResourceId, IList<ProjectResourcePeriodDTO> projectResourcePeriods)
        {
            return projectResourcePeriods.Where(x => x.ProjectResourceId == projectResourceId)
                .Select(x => x)
                .ToList();
        }

        public async Task<LocationDependentCalculationDTO> GetLocationDependentCalculationData(int pipSheetId)
        {
            LocationDependentCalculationDTO affectedDTO = new LocationDependentCalculationDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetLocationDependentCalculationsData")
               .WithSqlParam("@PIPSheetId", pipSheetId)
               .ExecuteStoredProcAsync((affectedResultSet) =>
               {
                   affectedDTO.CalculatedValue = affectedResultSet.ReadToList<CalculatedValueDTO>();
                   affectedResultSet.NextResult();

                   affectedDTO.ProjectPeriodList = affectedResultSet.ReadToList<ProjectPeriodTotalDTO>();
                   affectedResultSet.NextResult();

                   affectedDTO.ClientPricePeriodList = affectedResultSet.ReadToList<ClientPricePeriodDTO>();
                   affectedResultSet.NextResult();

                   affectedDTO.ClientPriceDTO = affectedResultSet.ReadToList<ClientPriceDTO>();
                   affectedResultSet.NextResult();

                   affectedDTO.RiskManagementDTO = affectedResultSet.ReadToList<RiskManagementDTO>();
               });
            return affectedDTO;
        }

        public async Task SaveLocationDependentCalculations(string userName, int pipSheetId)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveLocationDependentCalculations {0}, {1} ",
              userName, pipSheetId);
            await pipContext.SaveChangesAsync();
        }
    }
}
