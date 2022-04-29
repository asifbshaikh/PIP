using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// EbitdaController
    /// </summary>
    [Route("api/ebitda")]
    [ApiController]
    public class EbitdaController : BaseController
    {
        private readonly IEbitdaService ebitdaService;
        private readonly IEbitdaWorkflowService ebitdaWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// EbitdaController Constructor
        /// </summary>
        /// <param name="ebitdaService"></param>
        /// <param name="ebitdaWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public EbitdaController(IEbitdaService ebitdaService, IEbitdaWorkflowService ebitdaWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.ebitdaService = ebitdaService;
            this.ebitdaWorkflowService = ebitdaWorkflowService;
            this.accountAuthService = accountAuthService;

        }

        /// <summary>
        /// Get Ebitda and Std. Overhead data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<List<EbitdaDTO>>> GetEbitdaAndStandardOverhead(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.ebitdaService.GetEbitdaAndStandardOverhead(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Updates OverheadAmount and SharedSeatsUsePercent
        /// </summary>
        /// <param name="ebitdaData"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> UpdateEbitda([FromBody] List<EbitdaDTO> ebitdaData)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, ebitdaData[0].PipSheetId, false))
            {
                await this.ebitdaWorkflowService.ProcessEbitdaSaving(userName, ebitdaData);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
