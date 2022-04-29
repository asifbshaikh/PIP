using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// ClientPriceController
    /// </summary>
    [Route("api/clientPrice")]
    public class ClientPriceController : BaseController
    {
        private readonly IClientPriceService clientPriceService;
        private readonly IClientPriceWorkflowService clientPriceWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// ClientPriceController Constructor
        /// </summary>
        /// <param name="clientPriceService"></param>
        /// <param name="clientPriceWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public ClientPriceController(IClientPriceService clientPriceService, IClientPriceWorkflowService clientPriceWorkflowService
            , IAccountAuthService accountAuthService)
        {
            this.clientPriceService = clientPriceService;
            this.clientPriceWorkflowService = clientPriceWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Client Price data based on PipsheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<ClientPriceMainDTO>> GetClientPrice(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.clientPriceService.GetClientPriceData(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Client Price data
        /// </summary>
        /// <param name="clientPriceMainDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveClientPrice([FromBody] ClientPriceMainDTO clientPriceMainDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, clientPriceMainDTO.ClientPriceDTO[0].PipSheetId, false))
            {
                await this.clientPriceWorkflowService.ProcessClientPriceSaving(clientPriceMainDTO, userName);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
