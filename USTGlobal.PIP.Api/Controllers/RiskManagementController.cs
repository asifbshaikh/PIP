using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// RiskManagementController
    /// </summary>
    [Route("api/riskManagement")]
    public class RiskManagementController : BaseController
    {
        private readonly IRiskManagementService riskManagementService;
        private readonly IRiskManagementWorkflowService riskManagementWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// RiskManagementController Constructor
        /// </summary>
        /// <param name="riskManagementService"></param>
        /// <param name="riskManagementWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public RiskManagementController(IRiskManagementService riskManagementService, IRiskManagementWorkflowService riskManagementWorkflowService
            , IAccountAuthService accountAuthService)
        {
            this.riskManagementService = riskManagementService;
            this.riskManagementWorkflowService = riskManagementWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Risk Management data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns>An object containing risk management data </returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<RiskManagementCalcDTO>> GetRiskManagement(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.riskManagementService.GetRiskManagement(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Risk Management data
        /// </summary>
        /// <param name="riskManagementCalcDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveRiskManagement([FromBody] RiskManagementCalcDTO riskManagementCalcDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, riskManagementCalcDTO.riskManagement.PipSheetId, false))
            {
                await this.riskManagementWorkflowService.ProcessRiskManagementSaving(userName, riskManagementCalcDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}

