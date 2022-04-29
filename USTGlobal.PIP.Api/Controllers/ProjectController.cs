using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// ProjectController
    /// </summary>
    [AuthorizeRolePermission(AuthorizeRoles = "Editor,Readonly")]
    [Route("api/project")]
    public class ProjectController : BaseController
    {
        private readonly IProjectService projectService;
        private readonly IAccountAuthService accountAuthService;

        private readonly IAdminPipCheckinService adminPipCheckinService;
        private readonly IPipSheetService pipsheetService;

        /// <summary>
        /// ProjectController Constructor
        /// </summary>
        /// <param name="projectService"></param>
        /// <param name="adminPipCheckinService"></param>
        /// <param name="pipsheetService"></param>
        /// <param name="accountAuthService"></param>
        public ProjectController(IProjectService projectService, IAdminPipCheckinService adminPipCheckinService
        , IPipSheetService pipsheetService, IAccountAuthService accountAuthService)
        {
            this.projectService = projectService;
            this.adminPipCheckinService = adminPipCheckinService;
            this.pipsheetService = pipsheetService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Projects List
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("list")]
        public async Task<ActionResult<ProjectMainDTO>> GetProjectsList()
        {
            string userName = GetUserName();
            bool authStatus = await accountAuthService.GetProjectListAuthCheck(userName);

            if (authStatus)
            {
                return await this.projectService.GetProjectsList(userName);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get all Checked-Out PipSheet Versions based on ProjectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpGet, Route("{projectId}/versions/checkedOut")]
        public async Task<IList<CheckOutPipVersionDTO>> GetCheckedOutVersions(int projectId)
        {
            return await this.adminPipCheckinService.GetCheckedOutVersions(projectId);
        }

        /// <summary>
        /// Get Header Data based on ProjectId and PipSheetId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        [HttpGet, Route("{projectId}/pipSheet/{pipSheetId}/header1Data")]
        public async Task<ActionResult<HeaderInfoDTO>> GetHeader1Data(int projectId, int pipSheetId)
        {
            string userName = GetUserName();
            bool authStatus = await accountAuthService.GetHeader1DataAuth(userName, projectId, pipSheetId);

            if (authStatus)
            {
                return await this.pipsheetService.GetHeader1Data(projectId, pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get PipSheet version data based on ProjectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet, Route("{projectId}/pipSheet/versions")]
        public async Task<ActionResult<PipSheetVersionMainDTO>> GetPIPSheetVersionData(int projectId)
        {
            string userName = GetUserName();
            bool authStatus = await accountAuthService.GetPipVersionAuth(userName, projectId);

            if (authStatus)
            {
                return await this.pipsheetService.GetPIPSheetVersionData(projectId, userName);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
