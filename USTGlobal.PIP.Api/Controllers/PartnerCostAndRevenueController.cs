using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// PartnerCostAndRevenueController
    /// </summary>
    [Route("api/partnerCostAndRevenue")]
    public class PartnerCostAndRevenueController : BaseController
    {
        private readonly IPartnerCostAndRevenueService partnerCostAndRevenueService;
        private readonly IPartnerCostAndRevenueWorkflowService partnerCostAndRevenueWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// PartnerCostAndRevenueController Constructor
        /// </summary>
        /// <param name="partnerCostAndRevenueService"></param>
        /// <param name="partnerCostAndRevenueWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public PartnerCostAndRevenueController(IPartnerCostAndRevenueService partnerCostAndRevenueService, IPartnerCostAndRevenueWorkflowService partnerCostAndRevenueWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.partnerCostAndRevenueService = partnerCostAndRevenueService;
            this.partnerCostAndRevenueWorkflowService = partnerCostAndRevenueWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get partner cost and Revenue data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<PartnerCostAndRevenueDTO>> GetPartnerCostAndRevenue(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.partnerCostAndRevenueService.GetPartnerCostAndRevenue(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Partner cost and Revenue data
        /// </summary>
        /// <param name="partnerCostAndRevenueDTO"></param>
        [HttpPost]
        public async Task<ActionResult> SavePartnerCostAndRevenueData([FromBody] PartnerCostAndRevenueDTO partnerCostAndRevenueDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, partnerCostAndRevenueDTO.PartnerCost[0].PipSheetId, false))
            {
                await partnerCostAndRevenueWorkflowService.ProcessPartnerCostAndRevenueSaving(userName, partnerCostAndRevenueDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
