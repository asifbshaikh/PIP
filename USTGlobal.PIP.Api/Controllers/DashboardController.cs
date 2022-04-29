using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// Dashboard Controller
    /// </summary>
    [AuthorizeRolePermission(AuthorizeRoles = "Admin,Finance Approver,Editor,Reviewer,Readonly")]
    [Route("api/dashboard")]
    public class DashboardController : BaseController
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// DashboardController Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public DashboardController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Get Dashboard About Tool File
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("file")]
        public async Task<FileDTO> GetDashboardFile()
        {
            FileDTO fileDTO = new FileDTO();
            var fileUrl = Configuration.GetValue<string>("DashboardFile:path");

            try
            {
                if (await DoesFileExist(fileUrl))
                {
                    fileDTO.FilePath = fileUrl;
                    fileDTO.IsWebUrl = true;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
            }
            return fileDTO;
        }

        /// <summary>
        /// Checks if File exists on url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<bool> DoesFileExist(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
    }
}
