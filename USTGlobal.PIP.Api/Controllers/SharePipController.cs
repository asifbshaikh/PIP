using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// SharePipController
    /// </summary>
    [AuthorizeRolePermission(AuthorizeRoles = "Editor")]
    [Route("api/sharePipSheet")]
    public class SharePipController : BaseController
    {
        private readonly ISharePipService sharePipService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// SharePipController Constructor
        /// </summary>
        /// <param name="sharePipService"></param>
        /// <param name="accountAuthService"></param>
        public SharePipController(ISharePipService sharePipService, IAccountAuthService accountAuthService)
        {
            this.sharePipService = sharePipService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Shared Pip data based on ProjectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet, Route("project/{projectId}")]
        public async Task<ActionResult<List<SharePipDTO>>> GetSharedPipData(int projectId)
        {
            string userName = GetUserName();
            List<RoleAccountProjectDTO> roleAndAccounts = await accountAuthService.GetUserAccountProjectRoles(userName);

            if (roleAndAccounts.Find(x => x.ProjectId == projectId && (x.RoleId == 3)) != null)
            {
                return await this.sharePipService.GetSharedPipData(projectId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Delete Share Pip Access of the PIP Version based on PipSheetId, RoleId, AccountId and UserId
        /// </summary>
        /// <param name="pipSheetId"></param>
        /// <param name="roleId"></param>
        /// <param name="accountId"></param>
        /// <param name="sharedWithUserId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{pipSheetId}/role/{roleId}/account/{accountId}/user/{sharedWithUserId}")]
        public async Task DeleteSharedPipData(int pipSheetId, int roleId, int accountId, int sharedWithUserId)
        {
            string userName = GetUserName();
            await this.sharePipService.DeleteSharedPipData(pipSheetId, roleId, accountId, sharedWithUserId, userName);
        }

        /// <summary>
        /// Update Shared Pip Access
        /// </summary>
        /// <param name="sharedPip"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> UpdateSharedPipData([FromBody] SharePipDTO sharedPip)
        {
            string userName = GetUserName();
            List<RoleAccountProjectDTO> roleAndAccounts = await accountAuthService.GetUserAccountProjectRoles(userName);

            if (roleAndAccounts.Find(x => x.ProjectId == sharedPip.ProjectId && (x.RoleId == 3)) != null)
            {
                await this.sharePipService.UpdateSharedPipData(sharedPip, userName);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Shared Pip Access
        /// </summary>
        /// <param name="sharedPip"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<bool>> SaveSharedPipData([FromBody] List<SharePipDTO> sharedPip)
        {
            string userName = GetUserName();
            List<RoleAccountProjectDTO> roleAndAccounts = await accountAuthService.GetUserAccountProjectRoles(userName);

            if (roleAndAccounts.Find(x => x.ProjectId == sharedPip[0].ProjectId && (x.RoleId == 3)) != null)
            {
                return await this.sharePipService.SaveSharedPipData(sharedPip, userName);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get SharePip VersionData based on ProjectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet, Route("{projectId}/version")]
        public async Task<ActionResult<SharePipVersionDTO>> GetSharePipVersionData(int projectId)
        {
            string userName = GetUserName();
            List<RoleAccountProjectDTO> roleAndAccounts = await accountAuthService.GetUserAccountProjectRoles(userName);

            if (roleAndAccounts.Find(x => x.ProjectId == projectId && (x.RoleId == 3)) != null)
            {
                return await this.sharePipService.GetSharePipVersionData(projectId);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
