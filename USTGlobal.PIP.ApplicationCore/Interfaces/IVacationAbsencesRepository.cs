using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IVacationAbsencesRepository
    {
        Task<VacationAbsencesParentDTO> GetVacationAbsences(int pipSheetId);
        Task SaveVacationAbsencesData(string userName, VacationAbsencesDTO vacationAbsencesDTO, IList<ProjectPeriodTotalDTO> projectPeriodTotalDTO);
    }
}
