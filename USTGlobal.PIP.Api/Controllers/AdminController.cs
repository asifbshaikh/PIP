using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// AdminController
    /// </summary>
    [AuthorizeRolePermission(AuthorizeRoles = "Admin,Finance Approver")]
    [Route("api/admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService adminService;
        private readonly IAdminPipCheckinService adminPipCheckinService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// AdminController constructor
        /// </summary>
        /// <param name="adminService"></param>
        /// <param name="adminPipCheckinService"></param>
        /// <param name="accountAuthService"></param>
        public AdminController(IAdminService adminService, IAdminPipCheckinService adminPipCheckinService, IAccountAuthService accountAuthService)
        {
            this.adminService = adminService;
            this.accountAuthService = accountAuthService;
            this.adminPipCheckinService = adminPipCheckinService;
        }

        /// <summary>
        /// Save Admin and Finance Approver Roles
        /// </summary>
        /// <param name="adminRoleDTO"></param>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpPost, Route("role")]
        public async Task SaveAdminRole([FromBody] AdminRoleDTO adminRoleDTO)
        {
            string userName = GetUserName();
            this.adminService.isFromAdminScreen = adminRoleDTO.fromAdminScreen;
            await this.adminService.SaveAdminRole(adminRoleDTO, userName);
        }

        /// <summary>
        /// Save finance approver roles
        /// </summary>
        /// <param name="adminRoleDTO"></param>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin,Finance Approver")]
        [HttpPost, Route("role/financePOC")]
        public async Task<IActionResult> SaveFinancePOCRole([FromBody] AdminRoleDTO adminRoleDTO)
        {
            string userName = GetUserName();
            List<RoleAndAccountDTO> roleAndAccounts = await accountAuthService.GetUserAccountLevelRoles(userName);

            if (roleAndAccounts.Find(x => (x.AccountId == adminRoleDTO.AccountId && x.RoleId == 2) || (x.RoleId == 1)) != null)
            {
                this.adminService.isFromAdminScreen = adminRoleDTO.fromAdminScreen;
                await this.adminService.SaveAdminRole(adminRoleDTO, userName);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Editor, Reviewer, ReadOnly Roles
        /// </summary>
        /// <param name="roleDto"></param>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin,Finance Approver")]
        [HttpPost, Route("role/nonAdmin")]
        public async Task<ActionResult> SaveNonAdminRole([FromBody] RoleManagementDTO roleDto)
        {
            string userName = GetUserName();
            List<RoleAndAccountDTO> roleAndAccounts = await accountAuthService.GetUserAccountLevelRoles(userName);

            if (roleAndAccounts.Find(x => (x.AccountId == roleDto.AccountId && x.RoleId == 2) || (x.RoleId == 1)) != null)
            {
                await this.adminService.SaveUserRoles(roleDto, userName);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get all Users and associated Roles based on AccountId
        /// </summary>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpGet, Route("allUsersAndRoles")]
        public async Task<List<RoleManagementDTO>> getAllUsersAndAssociatedRoles()
        {
            return await this.adminService.getAllUsersAndAssociatedRoles();
        }

        /// <summary>
        /// Save Shared PIP Roles Editor and ReadOnly
        /// </summary>
        /// <param name="sharedPipRole"></param>
        /// <returns></returns>
        [HttpPost, Route("role/sharedPip")]
        public async Task SaveSharedPipRole([FromBody] SharedPipRoleDTO sharedPipRole)
        {
            string userName = GetUserName();
            await this.adminService.SaveSharedPipRole(sharedPipRole, userName);
        }

        /// <summary>
        /// Check-In all Checked-Out PIP Versions
        /// </summary>
        /// <param name="checkInPipVersions"></param>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpPost, Route("pipVersion")]
        public async Task SaveCheckedInVersions([FromBody] IList<CheckOutPipVersionDTO> checkInPipVersions)
        {
            string userName = GetUserName();
            await this.adminPipCheckinService.SaveCheckedInVersions(checkInPipVersions, userName);
        }
    }
}
