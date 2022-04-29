using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// PIPSheetController
    /// </summary>
    [Route("api/pipSheet")]
    [ApiController]
    public class PipSheetController : BaseController
    {
        private readonly IPipSheetService pipsheetService;
        private readonly IProjectControlWorkflowService projectControlWorkflowService;
        private readonly IPipVersionService pipVersionService;
        private readonly IReplicatePIPSheetService replicatePIPSheetService;
        private readonly IProjectHeaderWorkflowService projectHeaderWorkflowService;
        private readonly IResourcePlanningService resourcePlanningService;
        private readonly ISharedService sharedService;
        private readonly IUserService userService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// PipSheetController Constructor
        /// </summary>
        /// <param name="pipsheetService"></param>
        /// <param name="projectControlWorkflowService"></param>
        /// <param name="pipVersionService"></param>
        /// <param name="replicatePIPSheetService"></param>
        /// <param name="projectHeaderWorkflowService"></param>
        /// <param name="resourcePlanningService"></param>
        /// <param name="sharedService"></param>
        /// <param name="userService"></param>
        /// <param name="accountAuthService"></param>
        public PipSheetController(IPipSheetService pipsheetService
            , IProjectControlWorkflowService projectControlWorkflowService
            , IPipVersionService pipVersionService
            , IReplicatePIPSheetService replicatePIPSheetService
            , IProjectHeaderWorkflowService projectHeaderWorkflowService
            , IResourcePlanningService resourcePlanningService
            , ISharedService sharedService
            , IUserService userService
            , IAccountAuthService accountAuthService)
        {
            this.pipsheetService = pipsheetService;
            this.projectControlWorkflowService = projectControlWorkflowService;
            this.pipVersionService = pipVersionService;
            this.replicatePIPSheetService = replicatePIPSheetService;
            this.projectHeaderWorkflowService = projectHeaderWorkflowService;
            this.resourcePlanningService = resourcePlanningService;
            this.sharedService = sharedService;
            this.userService = userService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// GetSharedData
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/sharedData")]
        public async Task<ActionResult<SharedDataDTO>> GetSharedData(int pipSheetId)
        {
            string userName = GetUserName();
            if (pipSheetId != 0)
            {
                if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
                {
                    var userRole = await this.userService.GetUserData(userName);
                    if (userRole != null)
                    {
                        return await this.userService.GetSharedData(userName, pipSheetId);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                var userRole = await this.userService.GetUserData(userName);
                if (userRole != null)
                {
                    return await this.userService.GetSharedData(userName, pipSheetId);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get Project Control Data
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/projectControl")]
        public async Task<ActionResult<ProjectControlDTO>> GetProjectControlData(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.pipsheetService.GetProjectControlData(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Project Control Data
        /// </summary>
        /// <param name="projectControlDTO"></param>
        [HttpPost, Route("projectControl")]
        public async Task<ActionResult> SaveProjectControlData([FromBody] ProjectControlDTO projectControlDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, projectControlDTO.PIPSheetListDTO[0].PIPSheetId, false))
            {
                await this.projectControlWorkflowService.ProcessProjectControlSave(userName, projectControlDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Submit PIPSheet
        /// </summary>
        /// <param name="pipSheetMain"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SubmitPIPSheet([FromBody] PipSheetMainDTO pipSheetMain)
        {
            bool flag = false;
            string userName = GetUserName();

            if (pipSheetMain.IsSubmit && pipSheetMain.IsCheckedOut == true)
            {
                bool authStatus = await accountAuthService.GetUserAuthStatusForSubmit(userName, pipSheetMain);
                if (authStatus)
                {
                    flag = await this.pipsheetService.SubmitPIPSheet(pipSheetMain, userName);
                }
            }
            else if (pipSheetMain.IsApprove && pipSheetMain.IsCheckedOut == true)
            {
                bool authStatus = await accountAuthService.GetUserAuthStatusForSubmit(userName, pipSheetMain);
                if (authStatus)
                {
                    flag = await this.pipsheetService.SubmitPIPSheet(pipSheetMain, userName);
                }
            }
            else if (pipSheetMain.IsResend && pipSheetMain.IsCheckedOut == true)
            {
                bool authStatus = await accountAuthService.GetUserAuthStatusForSubmit(userName, pipSheetMain);
                if (authStatus)
                {
                    flag = await this.pipsheetService.SubmitPIPSheet(pipSheetMain, userName);
                }
            }
            else if (pipSheetMain.IsRevise && pipSheetMain.IsCheckedOut == true)
            {
                bool authStatus = await accountAuthService.GetUserAuthStatusForSubmit(userName, pipSheetMain);
                if (authStatus)
                {
                    flag = await this.pipsheetService.SubmitPIPSheet(pipSheetMain, userName);
                }
            }
            if (flag)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get currency conversion data based on PIPSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>       
        [HttpGet, Route("{pipSheetId}/currencyConversion")]
        public async Task<ActionResult<CurrencyDTO>> GetCurrencyConversionData(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.pipsheetService.GetCurrencyConversionData(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Check In PIP Sheet
        /// </summary>
        /// <param name="pipCheckIn"></param>
        /// <returns></returns>
        [HttpPut, Route("checkIn")]
        public async Task<IActionResult> UpdatePIPSheetCheckIn([FromBody] PipCheckInDTO pipCheckIn)
        {
            string userName = GetUserName();
            bool authStatus = await accountAuthService.GetEditorLevelCheck(userName, pipCheckIn.PIPSheetId);

            if (authStatus)
            {
                await pipsheetService.UpdatePIPSheetCheckIn(pipCheckIn, userName);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get PipSheet status
        /// </summary>
        /// <param name="pipSheetMain"></param>
        /// <returns></returns>
        [HttpPost, Route("status")]
        public async Task<ActionResult<SubmitPipSheetDTO>> GetPIPSheetStatus([FromBody] PipSheetMainDTO pipSheetMain)
        {
            string userName = GetUserName();
            bool authStatus = await accountAuthService.GetCheckIfAuth(userName, pipSheetMain.PIPSheetId);

            if (authStatus)
            {
                return await this.pipsheetService.GetPIPSheetStatus(pipSheetMain);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get Pip Override Fields
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/pipOverrides")]
        public async Task<ActionResult<IList<PipOverrideDTO>>> GetPipOverrides(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.pipsheetService.GetPipOverrides(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Create new Pip Sheet Version
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, Route("version")]
        public async Task<ActionResult<int>> CreateNewPipSheetVersion([FromBody]VersionDTO data)
        {
            string userName = GetUserName();
            bool authCheck = await accountAuthService.GetAccountLevelEditorLevelCheck(userName, data.PipsheetId);

            if (authCheck)
            {
                return await this.pipVersionService.CreateNewPipSheetVersion(userName, data.ProjectId, data.PipsheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Delete PipSheet based on PipSheetId and ProjectId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{pipSheetId}/project/{projectId}")]
        public async Task<ActionResult> DeletePipSheet(int pipSheetId, int projectId)
        {
            string userName = GetUserName();
            bool authCheck = await accountAuthService.GetAccountEditorCheckForDeletePip(userName, pipSheetId);

            if (authCheck)
            {
                await this.pipVersionService.DeletePipSheet(pipSheetId, projectId, userName);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get PipSheet Version details on Summary
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/versionSummary")]
        public async Task<ActionResult<SummaryPipVersionDTO>> GetPipVersionSummaryDetails(int pipSheetId)
        {
            string userName = GetUserName();
            bool authCheck = await accountAuthService.GetAccountLevelEditorLevelCheck(userName, pipSheetId);

            if (authCheck)
            {
                return await this.pipVersionService.GetVersionDetailsOnSummary(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Replicate PIPSheet
        /// </summary>
        /// <param name="replicatePIPSheet"></param>
        /// <returns></returns>
        [HttpPost, Route("replicate")]
        public async Task<ActionResult<RouteParamDTO>> ReplicatePIPSheet([FromBody] ReplicatePIPSheetDTO replicatePIPSheet)
        {
            string userName = GetUserName();
            bool authStatus = await accountAuthService.GetAccountLevelEditorCheck(userName, replicatePIPSheet.AccountId);

            if (authStatus)
            {
                RouteParamDTO routeParamDTO = await this.replicatePIPSheetService.ReplicatePIPSheet(userName, replicatePIPSheet);
                if (routeParamDTO.ErrorCode != -1)
                {
                    ProjectHeaderCurrencyDTO projectHeaderData = await this.projectHeaderWorkflowService.GetProjectHeaderData(routeParamDTO.ProjectId, routeParamDTO.PipSheetId);

                    //assigning incoming account id from replicate to save dependency.
                    projectHeaderData.ProjectHeader.IsFromReplicate = true;
                    routeParamDTO = await this.projectHeaderWorkflowService.ProcessProjectHeaderSave(userName, projectHeaderData.ProjectHeader);
                }
                return routeParamDTO;
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get Locations based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/locations")]
        public async Task<ActionResult<List<LocationDTO>>> GetProjectLocations(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await resourcePlanningService.GetProjectLocations(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get Project Milestones based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/milestones")]
        public async Task<ActionResult<List<ProjectMilestoneDTO>>> GetProjectMilestones(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await resourcePlanningService.GetProjectMilestones(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get Override Notification status based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{pipSheetId}/overrideNotificationStatus")]
        public async Task<ActionResult<OverrideNotificationDTO>> GetOverrideNotificationStatus(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.sharedService.GetOverrideNotificationStatus(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
