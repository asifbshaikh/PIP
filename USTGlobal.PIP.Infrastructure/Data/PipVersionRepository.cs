using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class PipVersionRepository : IPipVersionRepository
    {
        private readonly PipContext pipContext;

        public PipVersionRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<int> CreateNewPipSheetVersion(string userName, int projectId, int pipsheetId)
        {
            int? outputVersionNumber = null;
            SqlParameter[] inputParams = new SqlParameter[4];
            inputParams[0] = new SqlParameter("@ProjectId", projectId);
            inputParams[1] = new SqlParameter("@PipsheetId", pipsheetId);
            inputParams[2] = new SqlParameter("@RequestingUser", userName);
            inputParams[3] = new SqlParameter("@OutputVersionNumber", outputVersionNumber)
            {
                Direction = ParameterDirection.Output,
                Size = 10
            };
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_CreateNewPipVersion @ProjectId, @PipsheetId, @RequestingUser, @OutputVersionNumber output ", inputParams);
            return Int32.Parse(inputParams[3].Value.ToString());
        }

        public async Task DeletePipSheet(int pipSheetId, int projectId, string userName)
        {
            SqlParameter[] inputParams = new SqlParameter[3];
            inputParams[0] = new SqlParameter("@ProjectId", projectId);
            inputParams[1] = new SqlParameter("@PipSheetId", pipSheetId);
            inputParams[2] = new SqlParameter("@UserName", userName);

            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_DeletePipSheet @ProjectId, @PipSheetId, @UserName", inputParams);
            await pipContext.SaveChangesAsync();
        }

        public async Task<SummaryPipVersionDTO> GetVersionDetailsOnSummary(int pipSheetId)
        {
            return await (from proj in this.pipContext.Project
                          join ps in this.pipContext.PipSheet on proj.ProjectId equals ps.ProjectId
                          where ps.PipSheetId == pipSheetId
                          select new SummaryPipVersionDTO()
                          {
                              SFProjectId = proj.SFProjectId,
                              TotalVersionsPresent = (from v in this.pipContext.PipSheet where v.ProjectId == proj.ProjectId select v.VersionNumber).Max(),
                              VersionNumber = ps.VersionNumber
                          }
            ).SingleOrDefaultAsync();
        }       
    }
}
