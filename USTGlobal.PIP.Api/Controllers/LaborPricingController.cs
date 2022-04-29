using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// LaborPricingController
    /// </summary>
    [Route("api/laborPricing")]
    public class LaborPricingController : BaseController
    {
        private readonly ILaborPricingService laborPricingService;
        private readonly ILaborPricingWorkflowService laborPricingWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// MarginController Constructor
        /// </summary>
        /// <param name="laborPricingService"></param>
        /// <param name="laborPricingWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public LaborPricingController(ILaborPricingService laborPricingService, ILaborPricingWorkflowService laborPricingWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.laborPricingService = laborPricingService;
            this.laborPricingWorkflowService = laborPricingWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Labor pricing data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<LaborPricingDTO>> GetLaborPricingData(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.laborPricingService.GetLaborPricingData(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Labor pricing data
        /// </summary>
        /// <param name="laborPricingDTO"></param>
        /// /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveLaborPricingData(LaborPricingDTO laborPricingDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, laborPricingDTO.marginDTO.PipSheetId, false))
            {
                await this.laborPricingWorkflowService.ProcessLaborPricingSaving(userName, laborPricingDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }

        }

        /// <summary>
        /// Calculate Background Fields (G13, G14, G15, G16)
        /// </summary>
        /// <param name="calculateDTO"></param>
        /// <returns></returns>
        [HttpPost, Route("backgroundFields")]
        public async Task<ActionResult<LaborPricingBackgroundCalculationDTO>> CalculateBackgroundFields([FromBody] CalculateBackgroundFieldsDTO calculateDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, calculateDTO.PipSheetId, true))
            {
                return await this.laborPricingService.CalculateBackgroundFields(calculateDTO.PipSheetId,
            calculateDTO.IsMarginSet, calculateDTO.Which, calculateDTO.MarginPercent, calculateDTO.IsInitLoad,
            calculateDTO.InflatedCappedCost, calculateDTO.TotalInflation);
            }
            else
            {
                return Unauthorized();
            }
        }
    }

}