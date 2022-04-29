using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// UserController
    /// </summary>
    [Route("api/user")]
    public class UserController : BaseController
    {
        private readonly IAdminService adminService;
        private readonly IApproverService approverService;
        private readonly IUserService userService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// UserController Constructor
        /// </summary>
        /// <param name="adminService"></param>
        /// <param name="approverService"></param>
        /// <param name="userService"></param>
        /// <param name="accountAuthService"></param>
        public UserController(IAdminService adminService, IApproverService approverService, IUserService userService,
            IAccountAuthService accountAuthService)
        {
            this.adminService = adminService;
            this.approverService = approverService;
            this.userService = userService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// GetUserData
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<UserRoleDTO> GetUserData()
        {
            string userName = GetUserName();
            return await this.userService.GetUserData(userName);
        }

        /// <summary>
        /// Save User Data
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<string>> SaveUserData([FromBody] UserDTO userDTO)
        {
            if ((!Regex.IsMatch(userDTO.FirstName, @"^^(\w+ ?)*$") || !(userDTO.FirstName.Length <= 100)) ||
             (!Regex.IsMatch(userDTO.LastName, @"^^(\w+ ?)*$") || !(userDTO.LastName.Length <= 100)) ||
             (!Regex.IsMatch(userDTO.UID, @"^^(\w+ ?)*$") || !(userDTO.UID.Length <= 10)) ||
             (!Regex.IsMatch(userDTO.Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
             )
            {
                return BadRequest();
            }
            else
            {
                string userName = GetUserName();
                await this.userService.SaveUserData(userDTO, userName);
                return Ok();
            }
        }

        /// <summary>
        /// Get ReadOnly Users list
        /// </summary>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpGet, Route("readOnly")]
        public async Task<List<UserRoleReadOnly>> GetReadOnlyUserList()
        {
            return await this.adminService.GetReadOnlyUserList();
        }

        /// <summary>
        /// Get Admin and Finance Approver Users
        /// </summary>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpGet, Route("admin")]
        public async Task<List<UserDTO>> GetAdminUsers()
        {
            return await this.adminService.GetUsers();
        }

        /// <summary>
        /// Delete User Role based on UserId, AccountId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accountId"></param>
        /// <param name="fromAdminScreen"></param>
        /// <returns></returns>
        [HttpDelete, Route("{userId}/account/{accountId}/fromAdminScreen/{fromAdminScreen}")]
        public async Task<ActionResult> DeleteUserRole(int userId, int accountId, bool fromAdminScreen)
        {
            string userName = GetUserName();

            List<RoleAndAccountDTO> roleAndAccounts = await accountAuthService.GetUserAccountLevelRoles(userName);
            if (roleAndAccounts.Find(x => (x.AccountId == accountId && x.RoleId == 2) || x.RoleId == 1) != null)
            {
                this.adminService.isFromAdminScreen = fromAdminScreen;
                await this.adminService.DeleteUserRole(userId, accountId, userName);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Assign ReadOnly Role for all Accounts for UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpPost, Route("role/setReadOnly")]
        public async Task AssignReadOnlyRoleForAllAccounts([FromBody] int userId)
        {
            string userName = GetUserName();
            await this.adminService.AssignReadOnlyRoleForAllAccounts(userId, userName);
        }

        /// <summary>
        /// Delete ReadOnly Role for all accounts for UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpDelete, Route("{userId}/role/readOnly")]
        public async Task DeleteReadOnlyRoleForAllAccounts(int userId)
        {
            string userName = GetUserName();
            await this.adminService.DeleteReadOnlyRoleForAllAccounts(userId, userName);
        }

        /// <summary>
        /// Get Approvers PipSheet data
        /// </summary>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Reviewer,Finance Approver")]
        [HttpGet, Route("approver")]
        public async Task<List<ApproverDTO>> GetApproversData()
        {
            string userName = GetUserName();
            return await this.approverService.GetApproversData(userName);
        }

        /// <summary>
        /// Save Multiple User Data
        /// </summary>
        /// <returns></returns>
        [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
        [HttpPost, Route("uploadMultipleUsers")]
        public async Task<List<UserListResultDTO>> UploadMultipleUsersData()
        {
            string userName = GetUserName();
            var formFile = Request.Form.Files[0];
            byte[] byteArray = null;
            using (var stream = new MemoryStream())
            {
                formFile.CopyTo(stream);
                byteArray = stream.ToArray();
            }
            return await this.userService.UploadMultipleUserData(userName, byteArray);
        }
    }
}
