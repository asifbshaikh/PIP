using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// PriceAdjustmentYoyController
    /// </summary>
    [Route("api/priceAdjustment")]
    public class PriceAdjustmentYoyController : BaseController
    {
        private readonly IPriceAdjustmentYoyService priceAdjustmentYoyService;
        private readonly IColaWorkflowService colaWorkflowService;
        private readonly IAccountAuthService accountAuthService;


        /// <summary>
        /// PriceAdjustmentYoyController Constructor
        /// </summary>
        /// <param name="priceAdjustmentYoyService"></param>
        /// <param name="colaWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public PriceAdjustmentYoyController(IPriceAdjustmentYoyService priceAdjustmentYoyService, IColaWorkflowService colaWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.priceAdjustmentYoyService = priceAdjustmentYoyService;
            this.colaWorkflowService = colaWorkflowService;
            this.accountAuthService = accountAuthService;

        }

        /// <summary>
        /// Get Price Adjustment YOY data
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<PriceAdjustmentDTO>> GetPriceAdjustmentYoy(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.priceAdjustmentYoyService.GetPriceAdjustmentYoy(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Price Adjustment YOY data
        /// </summary>
        /// <param name="priceAdjustmentDTO"></param>
        [HttpPost]
        public async Task<ActionResult> SavePriceAdjustmentYoyData(PriceAdjustmentDTO priceAdjustmentDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, priceAdjustmentDTO.PriceAdjustmentYoyDTO.PipSheetId, false))
            {
                await this.colaWorkflowService.ProcessPriceAdjustmentYoySaving(userName, priceAdjustmentDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}