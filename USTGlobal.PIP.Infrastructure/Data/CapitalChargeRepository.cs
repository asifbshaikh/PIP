using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class CapitalChargeRepository : ICapitalChargeRepository
    {
        private readonly PipContext pipContext;
        public CapitalChargeRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<CapitalChargeResultSetDTO> GetCapitalCharge(int pipSheetId)
        {
            CapitalChargeResultSetDTO capitalChargeResultSetDTO = new CapitalChargeResultSetDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetCapitalCharge")
            .WithSqlParam("@PIPSheetId", pipSheetId)
            .ExecuteStoredProcAsync((capitalChargeResultSet) =>
            {
                capitalChargeResultSetDTO.capitalChargeDTO = capitalChargeResultSet.ReadToList<CapitalChargeDTO>().FirstOrDefault();
                capitalChargeResultSet.NextResult();

                capitalChargeResultSetDTO.projectPeriodTotalDTO = capitalChargeResultSet.ReadToList<ProjectPeriodTotalDTO>().ToList();
            });

            if (capitalChargeResultSetDTO.capitalChargeDTO == null)
            {
                capitalChargeResultSetDTO.capitalChargeDTO = new CapitalChargeDTO();
            }
            return capitalChargeResultSetDTO;
        }

        public async Task SaveCapitalCharge(string userName, CapitalChargeResultSetDTO capitalCharge, decimal? totalProjectCost)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveCapitalCharge {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                userName,
                capitalCharge.capitalChargeDTO.PipSheetId,
                capitalCharge.capitalChargeDTO.PaymentLag,
                capitalCharge.capitalChargeDTO.CapitalCharge,
                capitalCharge.capitalChargeDTO.TotalCostBeforeCap,
                totalProjectCost,

                new SqlParameter("@InputProjectPeriod", SqlDbType.Structured)
                {
                        Value = IListToDataTableHelper.ToDataTables(capitalCharge.projectPeriodTotalDTO),
                  TypeName = "dbo.ProjectPeriodTotal"
                });
                await pipContext.SaveChangesAsync();
        }
    }
}
