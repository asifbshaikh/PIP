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
    public class VacationAbsencesRepository : IVacationAbsencesRepository
    {
        private readonly PipContext pipContext;

        public VacationAbsencesRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<VacationAbsencesParentDTO> GetVacationAbsences(int pipSheetId)
        {
            VacationAbsencesParentDTO vacationAbsencesParentDTO = new VacationAbsencesParentDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetVacationAbsencesData")
                  .WithSqlParam("@PIPSheetId", pipSheetId)
                  .ExecuteStoredProcAsync((vacationAbsencesResultSet) =>
                  {
                      var vacationAbsences = vacationAbsencesResultSet.ReadToList<VacationAbsencesDTO>().FirstOrDefault();
                      vacationAbsencesResultSet.NextResult();

                      var periodLostRevenue = vacationAbsencesResultSet.ReadToList<PeriodLostRevenueDTO>();
                      vacationAbsencesParentDTO.PeriodLostRevenue = periodLostRevenue;
                      vacationAbsencesResultSet.NextResult();

                      bool IsOverrideUpdated = vacationAbsencesResultSet.ReadToValue<bool>() ?? false;

                      if (vacationAbsences != null)
                      {
                          vacationAbsencesParentDTO.PIPSheetId = vacationAbsences.PIPSheetId;
                          vacationAbsencesParentDTO.TotalRevenue = vacationAbsences.TotalRevenue;
                          vacationAbsencesParentDTO.IsPercent = vacationAbsences.IsPercent;
                          vacationAbsencesParentDTO.IsMarginSet = vacationAbsences.IsMarginSet;
                          vacationAbsencesParentDTO.IsOverride = vacationAbsences.IsOverride;
                          vacationAbsencesParentDTO.Amount = vacationAbsences.Amount == -1 ? null : vacationAbsences.Amount;
                          vacationAbsencesParentDTO.LostRevenue = vacationAbsences.LostRevenue;
                          vacationAbsencesParentDTO.IsOverrideUpdated = IsOverrideUpdated;
                      }
                  });

            return vacationAbsencesParentDTO;
        }

        public async Task SaveVacationAbsencesData(string userName, VacationAbsencesDTO vacationAbsencesDTO, IList<ProjectPeriodTotalDTO> projectPeriodTotalDTO)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveVacationAbsencesData {0}, {1}, {2}, " +
            " {3}, {4}, {5}, {6}, {7}",
            userName,
            vacationAbsencesDTO.PIPSheetId,
            vacationAbsencesDTO.IsPercent,
            vacationAbsencesDTO.Amount,
            vacationAbsencesDTO.LostRevenue,
            vacationAbsencesDTO.IsOverride,
            vacationAbsencesDTO.IsOverrideUpdated,
            new SqlParameter("@InputLostRevenue", SqlDbType.Structured)
            {
                Value = IListToDataTableHelper.ToDataTables(projectPeriodTotalDTO),
                TypeName = "dbo.ProjectPeriodTotal"
            });
            await pipContext.SaveChangesAsync();
        }
    }
}
