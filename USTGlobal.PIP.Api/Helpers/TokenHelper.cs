using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.Api.Helpers
{
    /// <summary>
    /// TokenHelper
    /// </summary>
    public class TokenHelper : ITokenHelper
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// TokenHelper Constructor
        /// </summary>
        /// <param name="contextAccessor"></param>
        public TokenHelper(IHttpContextAccessor contextAccessor)
        {
            this.httpContextAccessor = contextAccessor;
        }

        /// <summary>
        /// CreateClaims
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public IEnumerable<Claim> CreateClaims(UserRoleDTO user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roleClaims = user.Role.Select(role => new Claim(ClaimTypes.Role, role.RoleName.ToString()));
            claims.AddRange(roleClaims);
            return claims;
        }

        /// <summary>
        /// GetClaimsFromIdentity
        /// </summary>
        /// <returns></returns>
        public ApiContext GetClaimsFromIdentity()
        {
            int userId = int.Parse(this.httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email).Value);

            string[] roles = this.httpContextAccessor.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(x => (x.Value))
                .ToArray();

            return new ApiContext()
            {
                UserId = userId,
                Roles = roles
            };
        }
    }
}