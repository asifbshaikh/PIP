using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IVacationAbsencesService
    {
        Task<VacationAbsencesParentDTO> GetVacationAbsences(int pipSheetId);
        Task SaveVacationAbsencesData(string userName, VacationAbsencesParentDTO vacationAbsencesParentDTO);
        Task<VacationAbsencesParentDTO> CalculateVacationAbsences(int pipSheetId, string userName);
    }
}
