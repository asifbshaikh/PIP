using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IVacationAbsenceWorkflowService
    {
        Task ProcessVacationAbsencesSaving(string userName, VacationAbsencesParentDTO vacationAbsencesParentDTO);
    }
}
