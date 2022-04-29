using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class ReplicatePIPSheetRepository : IReplicateRepository
    {
        private readonly PipContext pipContext;
        public ReplicatePIPSheetRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<RouteParamDTO> ReplicatePIPSheet(string userName, ReplicatePIPSheetDTO replicate)
        {
            RouteParamDTO routeParams = new RouteParamDTO();

            try
            {

                // check if project exists
                if (replicate.ReplicateType == 1)
                {
                    var projectId = pipContext.Project.FirstOrDefault(val => val.SFProjectId == replicate.SFProjectId);
                    if (projectId != null)
                    {
                        routeParams.ErrorCode = -1;
                        routeParams.PipSheetId = -1;
                        routeParams.ProjectId = -1;
                        return routeParams;
                    }
                }

                SqlParameter[] inputParams = new SqlParameter[12];

                inputParams[0] = new SqlParameter("@SourceProjectId", replicate.SourceProjectId);
                inputParams[1] = new SqlParameter("@SourcePIPSheetId", replicate.SourcePIPSheetId);
                inputParams[2] = new SqlParameter("@AccountId", replicate.AccountId);
                inputParams[3] = new SqlParameter("@PaymentLag", replicate.paymentLag);
                inputParams[4] = new SqlParameter("@SFProjectId", replicate.SFProjectId);
                inputParams[5] = new SqlParameter("@ProjectNamePerSF", replicate.ProjectNamePerSF);
                inputParams[6] = new SqlParameter("@IsDummy", replicate.IsDummy);
                inputParams[7] = new SqlParameter("@UserName", userName);
                inputParams[8] = new SqlParameter("@ReplicateType", replicate.ReplicateType);
                inputParams[9] = new SqlParameter("@VersionNumber", replicate.VersionNumber);

                inputParams[10] = new SqlParameter("@NewProjectId", System.Data.SqlDbType.Int);
                inputParams[10].Direction = System.Data.ParameterDirection.Output;

                inputParams[11] = new SqlParameter("@NewPIPSheetId", System.Data.SqlDbType.Int);
                inputParams[11].Direction = System.Data.ParameterDirection.Output;



                await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_ReplicatePIPSheet @SourceProjectId, @SourcePIPSheetId," +
                   " @AccountId, @PaymentLag, @SFProjectId, @ProjectNamePerSF, @IsDummy, @UserName, @ReplicateType, @VersionNumber," +
                   " @NewProjectId OUTPUT,@NewPIPSheetId OUTPUT", inputParams);

                await pipContext.SaveChangesAsync();

                routeParams.ProjectId = Convert.ToInt32(inputParams[10].Value);
                routeParams.PipSheetId = Convert.ToInt32(inputParams[11].Value);
            }
            catch (Exception)
            {
                throw;
            }

            return routeParams;
        }
    }
}
