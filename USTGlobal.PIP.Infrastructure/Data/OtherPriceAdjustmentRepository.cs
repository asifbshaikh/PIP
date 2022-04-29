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
    public class OtherPriceAdjustmentRepository : IOtherPriceAdjustmentRepository
    {
        private readonly PipContext pipContext;

        public OtherPriceAdjustmentRepository(PipContext pipContext)
        {
            this.pipContext = pipContext;
        }

        public async Task<OtherPriceAdjustmentSubDTO> GetOtherPriceAdjustment(int pipSheetId)
        {
            OtherPriceAdjustmentSubDTO otherPriceAdjustmentSubDTO = new OtherPriceAdjustmentSubDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetOtherPriceAdjustment")
                .WithSqlParam("@PIPSheetId", pipSheetId)
             .ExecuteStoredProcAsync((otherPriceAdjustmentResultSet) =>
             {
                 otherPriceAdjustmentSubDTO.ProjectMilestone = otherPriceAdjustmentResultSet.ReadToList<ProjectMilestoneDTO>().ToList();
                 otherPriceAdjustmentResultSet.NextResult();

                 otherPriceAdjustmentSubDTO.OtherPriceAdjustment = otherPriceAdjustmentResultSet.ReadToList<OtherPriceAdjustmentDTO>().ToList();
                 otherPriceAdjustmentResultSet.NextResult();

                 otherPriceAdjustmentSubDTO.OtherPriceAdjustmentPeriodDetail = otherPriceAdjustmentResultSet.ReadToList<OtherPriceAdjustmentPeriodDetailDTO>().ToList();
                 otherPriceAdjustmentResultSet.NextResult();

                 otherPriceAdjustmentSubDTO.ProjectPeriod = otherPriceAdjustmentResultSet.ReadToList<ProjectPeriodDTO>().ToList();
                 otherPriceAdjustmentResultSet.NextResult();

                 otherPriceAdjustmentSubDTO.FeeBeforeAdjustment = otherPriceAdjustmentResultSet.ReadToList<FixBidCalcDTO>().SingleOrDefault();
                 otherPriceAdjustmentResultSet.NextResult();

                 otherPriceAdjustmentSubDTO.FeeBeforeAdjustmentPeriod = otherPriceAdjustmentResultSet.ReadToList<FixBidCalcPeriodDTO>().ToList();
                 otherPriceAdjustmentResultSet.NextResult();

                 otherPriceAdjustmentSubDTO.IsMonthlyFeeAdjustment = (otherPriceAdjustmentResultSet.ReadToValue<bool>() ?? false);
             });

            return otherPriceAdjustmentSubDTO;
        }

        public async Task SaveOtherPriceAdjustmentData(string userName, IList<OtherPriceAdjustmentDTO> otherPriceAdjustmentDTO, IList<OtherPriceAdjustmentPeriodDetailDTO> otherPriceAdjustmentPeriodDetailDTO,
            IList<OtherPriceAdjustmentPeriodTotalDTO> otherPriceAdjustmentPeriodTotalDTO, bool isMonthlyFeeAdjustment)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveOtherPriceAdjustmentData {0}, {1}, {2}, {3}, {4}",
                userName,
                new SqlParameter("@InputOtherPriceAdjustment", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(otherPriceAdjustmentDTO),
                    TypeName = "dbo.OtherPriceAdjustment"
                },
               new SqlParameter("@InputOtherPriceAdjustmentPeriod", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(otherPriceAdjustmentPeriodDetailDTO),
                   TypeName = "dbo.OtherPriceAdjustmentPeriod"
               },
               new SqlParameter("@InputOtherPriceAdjustmentPeriodTotal", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(otherPriceAdjustmentPeriodTotalDTO),
                   TypeName = "dbo.OtherPriceAdjustmentPeriodTotal"
               }
               , isMonthlyFeeAdjustment);

            await pipContext.SaveChangesAsync();
        }
    }
}
