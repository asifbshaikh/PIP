using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// VacationAbsencesController
    /// </summary>
    [Route("api/vacationAbsences")]    
    public class VacationAbsencesController : BaseController
    {
        private readonly IVacationAbsencesService vacationAbsencesService;
        private readonly IVacationAbsenceWorkflowService vacationAbsenceWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// VacationAbsencesController Constructor
        /// </summary>
        /// <param name="vacationAbsencesService"></param>
        /// <param name="vacationAbsenceWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public VacationAbsencesController(IVacationAbsencesService vacationAbsencesService, IVacationAbsenceWorkflowService vacationAbsenceWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.vacationAbsencesService = vacationAbsencesService;
            this.vacationAbsenceWorkflowService = vacationAbsenceWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Vacation Absences data based on PipsheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<VacationAbsencesParentDTO>> GetVacationAbsences(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.vacationAbsencesService.GetVacationAbsences(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Vacation Absences data
        /// </summary>
        /// <param name="vacationAbsencesParentDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveVacationAbsencesData([FromBody] VacationAbsencesParentDTO vacationAbsencesParentDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, vacationAbsencesParentDTO.PIPSheetId, false))
            {
                await this.vacationAbsenceWorkflowService.ProcessVacationAbsencesSaving(userName, vacationAbsencesParentDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}