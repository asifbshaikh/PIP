using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// SummaryController
    /// </summary>
    [Route("api/summary")]
    public class SummaryController : BaseController
    {
        private readonly ISummaryService summaryService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// SummaryController Constructor
        /// </summary>
        /// <param name="summaryService"></param>
        /// <param name="accountAuthService"></param>
        public SummaryController(ISummaryService summaryService, IAccountAuthService accountAuthService)
        {
            this.summaryService = summaryService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Billing Schedule data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/billingSchedule")]
        public async Task<ActionResult<BillingScheduleDetailDTO>> GetBillingScheduleDetail(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.summaryService.GetBillingScheduleDetail(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get PL Forecast data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/plForecast")]
        public async Task<ActionResult<PLForecastParentDTO>> GetPLForecastData(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.summaryService.GetPLForecastData(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save PL Forecast data on PipSheet Submit
        /// </summary>
        /// <param name="plForecastDTO"></param>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpPost, Route("{pipSheetId}/plForecastData")]
        public async Task<IActionResult> SavePLForecastData([FromBody] IList<PLForecastDTO> plForecastDTO, int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, false))
            {
                await this.summaryService.SavePLForecastData(userName, plForecastDTO, pipSheetId);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get Key Performance Indicators data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/kpi")]
        public async Task<ActionResult<KeyPerformanceIndicatorDTO>> GetKeyPerformanceIndicatorsData(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.summaryService.GetKeyPerformanceIndicatorsData(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get Location Wise data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/locationWiseDetails")]
        public async Task<ActionResult<LocationWiseSummaryDTO>> GetLocationWiseDetails(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.summaryService.GetLocationWiseDetails(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get Total Deal Financials data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/totalDealFinancials")]
        public async Task<ActionResult<List<TotalDealFinancialsDTO>>> GetTotalDealFinancials(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.summaryService.GetTotalDealFinancials(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
