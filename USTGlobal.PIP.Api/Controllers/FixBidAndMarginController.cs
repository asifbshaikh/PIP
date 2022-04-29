using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// FixBidAndMarginController
    /// </summary>
    [Route("api/fixBidAndMargin")]
    public class FixBidAndMarginController : BaseController
    {
        private readonly IFixBidAndMarginService fixBidAndMarginService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// FixBidAndMarginController Constructor
        /// </summary>
        /// <param name="fixBidAndMarginService"></param>
        /// <param name="accountAuthService"></param>
        public FixBidAndMarginController(IFixBidAndMarginService fixBidAndMarginService, IAccountAuthService accountAuthService)
        {
            this.fixBidAndMarginService = fixBidAndMarginService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Calculate, Get and Save Fix Bid and Margin Data
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<FixBidAndMarginDTO>> CalculateAndSaveFixBidData(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await fixBidAndMarginService.CalculateAndSaveFixBidData(pipSheetId, userName);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
