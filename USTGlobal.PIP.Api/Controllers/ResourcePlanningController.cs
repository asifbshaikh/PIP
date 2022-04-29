using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// ResourcePlanningController
    /// </summary>
    [Route("api/resourcePlanning")]
    public class ResourcePlanningController : BaseController
        {
        private readonly IResourcePlanningService resourcePlanningService;
        private readonly IResourcePlanningWorkflowService resourcePlanningWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// ResourcePlanningController Constructor
        /// </summary>
        /// <param name="resourcePlanningService"></param>
        /// <param name="resourcePlanningWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public ResourcePlanningController(IResourcePlanningService resourcePlanningService, IResourcePlanningWorkflowService resourcePlanningWorkflowService,
             IAccountAuthService accountAuthService)
        {
            this.resourcePlanningService = resourcePlanningService;
            this.resourcePlanningWorkflowService = resourcePlanningWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Save ResourcePlanning Data
        /// </summary>
        /// <param name="resourcePlanningDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveResourcePlanningData([FromBody] List<ResourcePlanningDTO> resourcePlanningDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, resourcePlanningDTO[0].PipSheetId, false))
            {
                await resourcePlanningWorkflowService.ProcessResourcePlanningSave(userName, resourcePlanningDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get ResourcePlanning Data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<ResourcePlanningMainDTO>> GetResourcePlanningData(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await resourcePlanningService.GetResourcePlanningData(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}