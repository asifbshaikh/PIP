using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// ProjectHeaderController
    /// </summary>
    [Route("api/projectHeader")]
    public class ProjectHeaderController : BaseController
    {
        private readonly IProjectHeaderService projectHeaderService;
        private readonly IProjectHeaderWorkflowService projectHeaderWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// ProjectHeaderController Constructor
        /// </summary>
        /// <param name="projectHeaderService"></param>
        /// <param name="projectHeaderWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public ProjectHeaderController(IProjectHeaderService projectHeaderService, IProjectHeaderWorkflowService projectHeaderWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.projectHeaderService = projectHeaderService;
            this.projectHeaderWorkflowService = projectHeaderWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Project Header data based on ProjectId and PipSheetId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pipSheetId"></param>
        /// <returns>Project header data</returns>
        [HttpGet, Route("project/{projectId}/pipSheet/{pipSheetId}")]
        public async Task<ActionResult<ProjectHeaderCurrencyDTO>> GetProjectHeaderData(int projectId, int pipSheetId)
        {
            if (projectId > 0 && pipSheetId > 0)
            {
                string userName = GetUserName();
                if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
                {
                    return await this.projectHeaderService.GetProjectHeaderData(projectId, pipSheetId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return await this.projectHeaderService.GetProjectHeaderData(projectId, pipSheetId);
            }

        }

        /// <summary>
        /// Save Project Header data
        /// </summary>
        /// <param name="projectHeader"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<RouteParamDTO>> SaveProjectHeaderData([FromBody] ProjectHeaderDTO projectHeader)
        {
            string userName = GetUserName();
            if (projectHeader.ProjectId > 0 && projectHeader.PIPSheetId > 0)
            {
                if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, projectHeader.PIPSheetId, false))
                {
                    return await this.projectHeaderWorkflowService.ProcessProjectHeaderSave(userName, projectHeader);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return await this.projectHeaderWorkflowService.ProcessProjectHeaderSave(userName, projectHeader);
            }

        }

        /// <summary>
        /// Get PipSheet WorkflowStatus And AccountSpecific Roles based on PipSheetId, AccountId and ProjectId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <param name="accountId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet, Route("pipSheet/{pipSheetId}/account/{accountId}/project/{projectId?}/workflowStatusAccountRole")]
        public async Task<WorkflowStatusAndAccountSpecificRoleDTO> GetWorkflowStatusAccountRole(int pipSheetId, int accountId, int projectId = 0)
        {
            string userName = GetUserName();
            return await this.projectHeaderService.GetWorkflowStatusAccountRole(userName, pipSheetId, accountId, projectId);
        }

        /// <summary>
        /// Get user role for all accounts
        /// </summary>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Finance Approver,Editor,Reviewer,Readonly")]
        [HttpGet, Route("GetUserRoleForAllAccounts")]
        public async Task<RoleAndAccountMainDTO> GetUserRoleForAllAccounts()
        {
            string userName = GetUserName();
            return await this.projectHeaderService.GetUserRoleForAllAccounts(userName);
        }
    }
}
