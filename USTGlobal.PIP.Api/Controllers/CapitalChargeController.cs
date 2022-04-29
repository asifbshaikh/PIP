using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// CapitalChargeController
    /// </summary>
    [Route("api/capitalCharge")]
    public class CapitalChargeController : BaseController
    {
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ICapitalChargeWorkflowService capitalChargeWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// CapitalChargeController Constructor
        /// </summary>
        /// <param name="capitalChargeService"></param>
        /// <param name="capitalChargeWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public CapitalChargeController(ICapitalChargeService capitalChargeService, ICapitalChargeWorkflowService capitalChargeWorkflowService
            , IAccountAuthService accountAuthService)
        {
            this.capitalChargeService = capitalChargeService;
            this.capitalChargeWorkflowService = capitalChargeWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Capital Charge data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns>An object containing capital charge</returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<CapitalChargeResultSetDTO>> GetCapitalCharge(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.capitalChargeService.GetCapitalCharge(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Capital Charge data
        /// </summary>
        /// <param name="capitalCharge"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveCapitalCharge([FromBody] CapitalChargeResultSetDTO capitalCharge)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, capitalCharge.capitalChargeDTO.PipSheetId, false))
            {
                await this.capitalChargeWorkflowService.ProcessCapitalChargeSaving(userName, capitalCharge);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}