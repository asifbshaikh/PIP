using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class FixBidAndMarginRepository : IFixBidAndMarginRepository
    {
        private readonly PipContext pipContext;
        public FixBidAndMarginRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<FixBidAndMarginDTO> CalculateAndSaveFixBidData(int pipSheetId, string userName)
        {
            FixBidAndMarginDTO fixBidAndMarginDTO = new FixBidAndMarginDTO();
            List<FixBidCalcParentDTO> fixBidCalcParentDTO = new List<FixBidCalcParentDTO>();
            await pipContext.LoadStoredProc("dbo.sp_CalculateFixedBidMarginData")
                .WithSqlParam("@PipSheetId", pipSheetId)
                .WithSqlParam("@UserName", userName)
                .ExecuteStoredProcAsync((resultSet) =>
                {
                    fixBidCalcParentDTO = resultSet.ReadToList<FixBidCalcParentDTO>().ToList();
                    resultSet.NextResult();

                    IList<FixBidCalcPeriodDTO> fixBidPeriodDTO = resultSet.ReadToList<FixBidCalcPeriodDTO>();
                    resultSet.NextResult();

                    fixBidAndMarginDTO.ProjectPeriodDTO = resultSet.ReadToList<ProjectPeriodDTO>();
                    resultSet.NextResult();

                    fixBidAndMarginDTO.MarginDTO = resultSet.ReadToList<MarginDTO>().FirstOrDefault();
                    resultSet.NextResult();

                    fixBidAndMarginDTO.MarginBeforeAdjustment = resultSet.ReadToValue<decimal>();

                    foreach (FixBidCalcParentDTO fixBid in fixBidCalcParentDTO)
                    {
                        fixBid.PeriodDetails = new List<FixBidCalcPeriodDTO>();
                        fixBid.PeriodDetails.AddRange(fixBidPeriodDTO.Where(fixBidPeriod => fixBidPeriod.CostMarginId == fixBid.CostMarginId).ToList());
                    }
                    fixBidAndMarginDTO.FixBidMarginDTO = fixBidCalcParentDTO;
                });
            return fixBidAndMarginDTO;
        }

    }
}
