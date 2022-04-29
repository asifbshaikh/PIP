using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// OtherPriceAdjustmentController
    /// </summary>
    [Route("api/otherPriceAdjustment")]
    public class OtherPriceAdjustmentController : BaseController
    {
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;
        private readonly IOtherPriceAdjustmentWorkflowService otherPriceAdjustmentWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// OtherPriceAdjustmentController Constructor
        /// </summary>
        /// <param name="otherPriceAdjustmentService"></param>
        /// <param name="otherPriceAdjustmentWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public OtherPriceAdjustmentController(IOtherPriceAdjustmentService otherPriceAdjustmentService, IOtherPriceAdjustmentWorkflowService otherPriceAdjustmentWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
            this.otherPriceAdjustmentWorkflowService = otherPriceAdjustmentWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Other Price Adjustment data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<OtherPriceAdjustmentMainDTO>> GetOtherPriceAdjustment(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.otherPriceAdjustmentWorkflowService.GetOtherPriceAdjustment(pipSheetId, userName);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save OtherPriceAdjustment data
        /// </summary>
        /// <param name="otherPriceAdjustmentMainDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveOtherPriceAdjustmentData([FromBody] OtherPriceAdjustmentMainDTO otherPriceAdjustmentMainDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, otherPriceAdjustmentMainDTO.OtherPriceAdjustmentParent[0].PipSheetId, false))
            {
                await this.otherPriceAdjustmentWorkflowService.ProcessOtherPriceAdjustmentSaving(userName, otherPriceAdjustmentMainDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
