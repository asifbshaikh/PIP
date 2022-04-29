using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// LogsController
    /// </summary>
    [AllowAnonymous]
    [Route("api/logs")]
    public class LogsController : BaseController
    {
        readonly ILogger logger = Log.ForContext<LogsController>();

        /// <summary>
        /// SaveLogs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void SaveLogs([FromBody] LogsDTO logsDTO)
        {
            try
            {
                throw new ClientException("LoggedIn User '" + logsDTO.Additional[0] + "' raised ClientException." + logsDTO.Message);
            }
            catch (ClientException ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
