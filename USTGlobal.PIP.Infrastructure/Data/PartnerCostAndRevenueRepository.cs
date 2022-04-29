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
    public class PartnerCostAndRevenueRepository : IPartnerCostAndRevenueRepository
    {
        private readonly PipContext pipContext;
        public PartnerCostAndRevenueRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<PartnerCostAndRevenueParentDTO> GetPartnerCostAndRevenue(int pipSheetId)
        {
            PartnerCostAndRevenueParentDTO partnerCostAndRevenueParentDTO = new PartnerCostAndRevenueParentDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetPartnerCostAndRevenue")
             .WithSqlParam("@PIPSheetId", pipSheetId)
             .ExecuteStoredProcAsync((partnerCostAndRevenueResultSet) =>
             {
                 partnerCostAndRevenueParentDTO.ProjectMilestoneDTO = partnerCostAndRevenueResultSet.ReadToList<ProjectMilestoneDTO>().ToList();
                 partnerCostAndRevenueResultSet.NextResult();

                 partnerCostAndRevenueParentDTO.PartnerCostDTO = partnerCostAndRevenueResultSet.ReadToList<PartnerCostDTO>().ToList();
                 partnerCostAndRevenueResultSet.NextResult();

                 partnerCostAndRevenueParentDTO.PartnerCostPeriodDetailDTO = partnerCostAndRevenueResultSet.ReadToList<PartnerCostPeriodDetailDTO>().ToList();
                 partnerCostAndRevenueResultSet.NextResult();

                 partnerCostAndRevenueParentDTO.PartnerRevenueDTO = partnerCostAndRevenueResultSet.ReadToList<PartnerRevenueParentDTO>().ToList();
                 partnerCostAndRevenueResultSet.NextResult();

                 partnerCostAndRevenueParentDTO.PartnerRevenuePeriodDetailDTO = partnerCostAndRevenueResultSet.ReadToList<PartnerRevenuePeriodDetailDTO>().ToList();
                 partnerCostAndRevenueResultSet.NextResult();

                 partnerCostAndRevenueParentDTO.projectPeriodDTO = partnerCostAndRevenueResultSet.ReadToList<ProjectPeriodDTO>().ToList();
             });
            return partnerCostAndRevenueParentDTO;
        }

        public async Task SavePartnerCostAndRevenueData(string userName, IList<PartnerCostDTO> partnerCostDTO, IList<PartnerCostPeriodDetailDTO> partnerCostPeriodDetailDTO, IList<PartnerRevenueDTO> partnerRevenueDTO, IList<PartnerRevenuePeriodDetailDTO> partnerRevenuePeriodDetailDTO, IList<PartnerCostPeriodTotalDTO> partnerCostPeriodTotalDTO, IList<PartnerRevenuePeriodTotalDTO> partnerRevenuePeriodTotalDTO)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SavePartnerCostAndRevenue {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                userName,
                new SqlParameter("@InputPartnerCost", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(partnerCostDTO),
                    TypeName = "dbo.PartnerCost"
                },
               new SqlParameter("@InputPartnerCostPeriod", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(partnerCostPeriodDetailDTO),
                   TypeName = "dbo.PartnerCostPeriod"
               },
               new SqlParameter("@InputPartnerRevenue", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(partnerRevenueDTO),
                   TypeName = "dbo.PartnerRevenue"
               },
               new SqlParameter("@InputPartnerRevenuePeriod", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(partnerRevenuePeriodDetailDTO),
                   TypeName = "dbo.PartnerRevenuePeriod"
               },
               new SqlParameter("@InputPartnerCostPeriodTotal", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(partnerCostPeriodTotalDTO),
                   TypeName = "dbo.PartnerCostPeriodTotal"
               },
               new SqlParameter("@InputPartnerRevenuePeriodTotal", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(partnerRevenuePeriodTotalDTO),
                   TypeName = "dbo.PartnerRevenuePeriodTotal"
               });
            await pipContext.SaveChangesAsync();
        }
    }
}
