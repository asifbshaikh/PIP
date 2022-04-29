using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Helpers;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// AuthController
    /// </summary>
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private readonly AppSettings config;
        private readonly ITokenHelper tokenHelper;
        private readonly IUserService userService;

        /// <summary>
        /// AuthController Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="tokenHelper"></param>
        /// <param name="repoService"></param>
        public AuthController(IOptions<AppSettings> config, ITokenHelper tokenHelper, IUserService repoService)
        {
            this.config = config.Value;
            this.tokenHelper = tokenHelper;
            this.userService = repoService;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            try
            {
                bool userStatus = await this.userService.VerifyUserData(user.UserName);

                if (userStatus)
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.SecretKey));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    UserRoleDTO userData = await this.userService.GetUserData(user.UserName);
                    if (userData != null)
                    {
                        // get user data from database / AD
                        var userRoleDTO = new UserRoleDTO()
                        {
                            UserId = userData.UserId,
                            Email = userData.Email,
                            Role = userData.Role
                        };

                        var claims = tokenHelper.CreateClaims(userRoleDTO);

                        var tokenOptions = new JwtSecurityToken(
                            issuer: config.ApiUrl,
                            audience: config.AppUrl,
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(config.TokenExpiryTime),
                            signingCredentials: signinCredentials
                        );

                        string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                        return Ok(new { ApiToken = tokenString, userData.UserId });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("{0} Exception occured    .", exception);
            }

            return Ok(new { Code = 200 });
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("logout")]
        public IActionResult Logout()
        {
            return Ok(new { Code = 200 } );
        }
    }
}