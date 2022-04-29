using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Entities;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class ApproverRepository : IApproverRepository
    {
        private readonly PipContext pipContext;
        public ApproverRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<ApproverDTO>> GetApproversData(string userName)
        {
            List<ApproverDTO> approverDTOList = new List<ApproverDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetApproversData")
               .WithSqlParam("@UserName", userName)
               .ExecuteStoredProcAsync((result) =>
               {
                   approverDTOList = result.ReadToList<ApproverDTO>().ToList();
               });

            for (int i = 0; i < approverDTOList.Count; i++) 
            {
                if (approverDTOList[i].PipSheetStatus == Constants.strStatusApprovalPending)
                {
                    approverDTOList[i].PipSheetStatus = Constants.StatusWaitingforApproval;
                }                
            }

            return approverDTOList;
        }
    }
}
