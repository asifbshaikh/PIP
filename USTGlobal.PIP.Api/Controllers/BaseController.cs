using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// BaseController
    /// </summary>
    [AuthorizeRolePermission(AuthorizeRoles = "Finance Approver,Editor,Reviewer,Readonly")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// GetApiContext
        /// </summary>
        /// <returns></returns>
        protected ApiContext GetApiContext()
        {
            int userId = int.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value);

            string[] roles = HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(x => (x.Value))
                .ToArray();

            return new ApiContext()
            {
                UserId = userId,
                Roles = roles
            };
        }

        /// <summary>
        /// GetUserName
        /// </summary>
        /// <returns></returns>
        protected string GetUserName()
        {
            return HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        }
    }
}
