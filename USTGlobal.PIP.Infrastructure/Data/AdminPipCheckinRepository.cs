using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class AdminPipCheckinRepository : IAdminPipCheckinRepository
    {
        private readonly PipContext pipContext;

        public AdminPipCheckinRepository(PipContext pipContext)
        {
            this.pipContext = pipContext;
        }

        public async Task<IList<AccountBasedProjectDTO>> GetAccountBasedProjects(int accountId)
        {
            return await this.pipContext.Project
                .Where(p => p.AccountId == accountId
                && p.Status == Constants.StatusNotSubmitted)
                .Select(p => new AccountBasedProjectDTO
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    SFProjectId = p.SFProjectId
                }).ToListAsync();
        }

        public async Task<IList<CheckOutPipVersionDTO>> GetCheckedOutVersions(int projectId)
        {
            return await (from ps in this.pipContext.PipSheet
                          join u in this.pipContext.User on ps.CheckedInOutBy equals u.UserId
                          where ps.ProjectId == projectId
                          && ps.IsCheckedOut == false
                          && ps.IsActive == true
                          select new CheckOutPipVersionDTO()
                          {
                              PipSheetId = ps.PipSheetId,
                              VersionNumber = ps.VersionNumber,
                              CheckedOutByName = u.FirstName + " " + u.LastName,
                              CheckedOutByUID = u.UID
                          }).ToListAsync();
        }

        public async Task SaveCheckedInVersions(IList<CheckOutPipVersionDTO> checkInPipVersions, string userName)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveAdminPipCheckin {0}, {1}",
                                userName,
                                new SqlParameter("@InputAdminPipCheckinVersions", SqlDbType.Structured)
                                {
                                    Value = IListToDataTableHelper.ToDataTables(checkInPipVersions),
                                    TypeName = "dbo.AdminPipCheckinVersion"
                                }
                );
            await pipContext.SaveChangesAsync();
        }
    }
}
