using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Entities;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class ClientPriceRepository : IClientPriceRepository
    {
        private readonly PipContext pipContext;

        public ClientPriceRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<ClientPriceDBResultDTO> GetClientPriceData(int pipSheetId)
        {
            ClientPriceDBResultDTO clientPriceDBResultDTO = new ClientPriceDBResultDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetClientPriceData")
                  .WithSqlParam("@PIPSheetId", pipSheetId)
                  .ExecuteStoredProcAsync((clientPriceResultSet) =>
                  {
                      clientPriceDBResultDTO.FixBidCalcDTO = clientPriceResultSet.ReadToList<FixBidCalcDTO>().FirstOrDefault();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.FixBidCalcPeriodDTO = clientPriceResultSet.ReadToList<FixBidCalcPeriodDTO>().ToList();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.CalculatedValueDTO = clientPriceResultSet.ReadToList<CalculatedValueDTO>().FirstOrDefault();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.ProjectPeriodTotalDTO = clientPriceResultSet.ReadToList<ProjectPeriodTotalDTO>().ToList();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.ClientPriceDTO = clientPriceResultSet.ReadToList<ClientPriceDTO>().ToList();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.ClientPricePeriodDTO = clientPriceResultSet.ReadToList<ClientPricePeriodDTO>().ToList();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.ProjectPeriodDTO = clientPriceResultSet.ReadToList<ProjectPeriodDTO>().ToList();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.IsFixedBid = clientPriceResultSet.ReadToValue<bool>(); 
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.FeesAtRisk = clientPriceResultSet.ReadToValue<decimal>();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.RiskManagementDTO = clientPriceResultSet.ReadToList<RiskManagementDTO>().FirstOrDefault();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.RiskManagementPeriodDTO = clientPriceResultSet.ReadToList<RiskManagementPeriodDetailDTO>().ToList();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.CapitalCharge = clientPriceResultSet.ReadToValue<decimal>();
                      clientPriceResultSet.NextResult();

                      bool IsOverrideUpdated = clientPriceResultSet.ReadToValue<bool>() ?? false;
                      clientPriceResultSet.NextResult();


                      clientPriceDBResultDTO.CurrencyId = clientPriceResultSet.ReadToValue<int>();
                      clientPriceResultSet.NextResult();

                      if (clientPriceDBResultDTO.ClientPriceDTO.Count > 1)
                      {
                          clientPriceDBResultDTO.ClientPriceDTO[0].IsOverrideUpdated = IsOverrideUpdated;
                      }
                  });
            return clientPriceDBResultDTO;
        }

        public async Task SaveClientPriceData(IList<ClientPricePeriodDTO> clientPricePeriodDTOs, IList<ClientPriceDTO> clientPriceDTOs, IList<ProjectPeriodTotalDTO> projectPeriodTotalDTOs, string userName)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveClientPriceData {0}, {1}, {2}, {3} ",
               userName,
               new SqlParameter("@InputClientPrice", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(clientPriceDTOs),
                   TypeName = "dbo.ClientPrice"
               },
               new SqlParameter("@InputClientPricePeriod", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(clientPricePeriodDTOs),
                   TypeName = "dbo.ClientPricePeriod"
               },
               new SqlParameter("@PeriodTotals", SqlDbType.Structured)
               {
                   Value = IListToDataTableHelper.ToDataTables(projectPeriodTotalDTOs),
                   TypeName = "dbo.ProjectPeriodTotal"
               });
        }

        public async Task<ClientPriceDBResultDTO> GetClientPriceForCalculation(int pipSheetId)
        {
            ClientPriceDBResultDTO clientPriceDBResultDTO = new ClientPriceDBResultDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetClientPriceForCalculation")
                  .WithSqlParam("@PipSheetId", pipSheetId)
                  .ExecuteStoredProcAsync((clientPriceResultSet) =>
                  {
                      clientPriceDBResultDTO.FixBidCalcDTO = clientPriceResultSet.ReadToList<FixBidCalcDTO>().FirstOrDefault();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.FixBidCalcPeriodDTO = clientPriceResultSet.ReadToList<FixBidCalcPeriodDTO>().ToList();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.CalculatedValueDTO = clientPriceResultSet.ReadToList<CalculatedValueDTO>().FirstOrDefault();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.ProjectPeriodTotalDTO = clientPriceResultSet.ReadToList<ProjectPeriodTotalDTO>().ToList();
                      clientPriceResultSet.NextResult();

                      clientPriceDBResultDTO.FeesAtRisk = clientPriceResultSet.ReadToValue<decimal>();
                      clientPriceResultSet.NextResult();
                  });
            return clientPriceDBResultDTO;
        }
    }
}
