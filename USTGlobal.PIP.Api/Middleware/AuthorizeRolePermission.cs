using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.Infrastructure.Data;

namespace USTGlobal.PIP.Api.Middleware
{
    /// <summary>
    /// AuthorizeRolePermission
    /// </summary>
    public class AuthorizeRolePermission : AuthorizeAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// AuthorizeRoles string to get from controller
        /// </summary>
        public string AuthorizeRoles { get; set; } //AuthorizeRoles string to get from controller

        /// <summary>
        /// OnAuthorization
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //Validate if any AuthorizeRoles are passed when using attribute at controller or action level
            if (string.IsNullOrEmpty(AuthorizeRoles))
            {
                //Validation cannot take place without any AuthorizeRoles so returning unauthorized
                context.Result = new UnauthorizedResult();
                return;
            }


            //The below line can be used if you are reading permissions from token
            //var permissionsFromToken=context.HttpContext.User.Claims.Where(x=>x.Type=="Permissions").Select(x=>x.Value).ToList()

            //Identity.Name will have windows logged in user id, in case of Windows Authentication
            //Indentity.Name will have user name passed from token, in case of JWT Authenntication and having claim type "ClaimTypes.Name"
            var userName = context.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            var assignedAuthorizeRolesForUser = this.GetUserRolesData(userName);


            var requiredAuthorizeRoles = AuthorizeRoles.Split(","); //Multiple AuthorizeRoles can be received from controller, delimiter "," is used to get individual values
            foreach (var x in requiredAuthorizeRoles)
            {
                foreach (var role in assignedAuthorizeRolesForUser.RoleDTO)
                {
                    if (role.RoleName.Contains(x))
                        return; //User Authorized. Wihtout setting any result value and just returning is sufficent for authorizing user
                }
            }

            context.Result = new UnauthorizedResult();
            return;
        }

        /// <summary>
        /// GetUserRolesData
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public UserDataRoleDTO GetUserRolesData(string emailId)
        {
            PipContext pipContext = new PipContext(true);

            UserDataRoleDTO userDataRoleDTO = new UserDataRoleDTO();
            pipContext.LoadStoredProc("dbo.sp_GetUserData")
             .WithSqlParam("@UserName", emailId)
              .ExecuteStoredProc((userDataResultSet) =>
              {
                  userDataRoleDTO.UserDTO = userDataResultSet.ReadToList<UserDTO>();
                  userDataResultSet.NextResult();

                  userDataRoleDTO.RoleDTO = userDataResultSet.ReadToList<RoleDTO>().ToList();
                  userDataResultSet.NextResult();
              });
            return userDataRoleDTO;
        }
    }
}
