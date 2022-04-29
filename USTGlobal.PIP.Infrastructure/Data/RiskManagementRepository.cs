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
    public class RiskManagementRepository : IRiskManagementRepository
    {
        private readonly PipContext pipContext;
        public RiskManagementRepository(PipContext context)
        {
            this.pipContext = context;
        }
        public async Task<RiskManagementParentCalcDTO> GetRiskManagement(int pipSheetId)
        {
            RiskManagementParentCalcDTO riskManagementParentCalcDTO = new RiskManagementParentCalcDTO();
            bool isOverrideUpdated = false;
            await pipContext.LoadStoredProc("dbo.sp_GetRiskManagement")
          .WithSqlParam("@PIPSheetId", pipSheetId)
          .ExecuteStoredProcAsync((riskManagementResultSet) =>
          {
              riskManagementParentCalcDTO.CalculatedValue = riskManagementResultSet.ReadToList<CalculatedValueDTO>().FirstOrDefault();
              riskManagementResultSet.NextResult();

              riskManagementParentCalcDTO.RiskManagement = riskManagementResultSet.ReadToList<RiskManagementDTO>().FirstOrDefault();
              riskManagementResultSet.NextResult();

              riskManagementParentCalcDTO.RiskManagementPeriodDetail = riskManagementResultSet.ReadToList<RiskManagementPeriodDetailDTO>().ToList();
              riskManagementResultSet.NextResult();

              riskManagementParentCalcDTO.projectPeriod = riskManagementResultSet.ReadToList<ProjectPeriodDTO>().ToList();
              riskManagementResultSet.NextResult();

              riskManagementParentCalcDTO.ProjectDeliveryTypeId = riskManagementResultSet.ReadToValue<int>();
              riskManagementResultSet.NextResult();

              isOverrideUpdated = riskManagementResultSet.ReadToValue<bool>() ?? false;
          });

            if (riskManagementParentCalcDTO.RiskManagement != null)
            {
                riskManagementParentCalcDTO.RiskManagement.IsOverrideUpdated = isOverrideUpdated;
            }
            return riskManagementParentCalcDTO;
        }

        public async Task SaveRiskManagement(string userName, CalculatedValueDTO calculatedValueDTO, RiskManagementDTO riskManagementDTO, IList<RiskManagementPeriodDetailDTO> riskManagementPeriodDetailDTO)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveRiskManagement {0}, {1}, {2}, {3}, {4}, {5}",
                userName,
                calculatedValueDTO.PipSheetId,
                calculatedValueDTO.StdOverheadAmount,
                calculatedValueDTO.TotalDirectExpense,

               new SqlParameter("@InputRiskManagement", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTable(riskManagementDTO),
                   TypeName = "dbo.RiskManagement"
               },
               new SqlParameter("@InputRiskManagementPeriod", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(riskManagementPeriodDetailDTO),
                   TypeName = "dbo.RiskManagementPeriod"
               });
            await pipContext.SaveChangesAsync();
        }
    }
}
