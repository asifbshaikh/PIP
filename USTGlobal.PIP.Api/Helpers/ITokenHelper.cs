using System.Collections.Generic;
using System.Security.Claims;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.Api.Helpers
{
    /// <summary>
    /// ITokenHelper
    /// </summary>
    public interface ITokenHelper
    {
        /// <summary>
        /// CreateClaims
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        IEnumerable<Claim> CreateClaims(UserRoleDTO user);
        /// <summary>
        /// GetClaimsFromIdentity
        /// </summary>
        /// <returns></returns>
        ApiContext GetClaimsFromIdentity();
    }
}