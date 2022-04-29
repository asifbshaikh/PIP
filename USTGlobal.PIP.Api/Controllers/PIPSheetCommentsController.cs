using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// PIPSheetCommentsController
    /// </summary>
    [Route("api/comments")]
    [ApiController]
    public class PIPSheetCommentsController : BaseController
    {
        private readonly IPIPSheetCommentService pipsheetCommentService;
        private readonly IAccountAuthService accountAuthService;
        private readonly IPipSheetService pipsheetService;

        /// <summary>
        /// PIPSheetCommentsController Constructor
        /// </summary>
        /// <param name="pipsheetCommentService"></param>
        /// <param name="accountAuthService"></param>
        /// <param name="pipsheetService"></param>
        public PIPSheetCommentsController(IPIPSheetCommentService pipsheetCommentService,
        IAccountAuthService accountAuthService, IPipSheetService pipsheetService)
        {
            this.pipsheetCommentService = pipsheetCommentService;
            this.accountAuthService = accountAuthService;
            this.pipsheetService = pipsheetService;
        }

        /// <summary>
        /// Get PIPSheet Comments
        /// </summary>
        /// <param name="pipsheetId"></param>
        /// <returns></returns>

        [HttpGet, Route("{pipsheetId}")]
        public async Task<ActionResult<List<PIPSheetCommentDTO>>> GetPIPSheetComments(int pipsheetId)
        {
            string userName = GetUserName();
            bool authStatus = await accountAuthService.GetPipSheetLevelAuth(userName, pipsheetId);

            if (authStatus)
            {
                return await pipsheetCommentService.GetPIPSheetComments(pipsheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save PIPSheet Comments
        /// </summary>
        /// <param name="comment"></param>
        /// <returns>comment id</returns>
        [HttpPost]
        public async Task<ActionResult<int>> SavePIPSheetComment([FromBody]PIPSheetCommentDTO comment)
        {
            string userName = GetUserName();
            bool checkIfAnyVersionApproved = false;
            bool authStatus = false;
            PipSheetVersionMainDTO pipSheetVersionMainDTO = new PipSheetVersionMainDTO();
            pipSheetVersionMainDTO = await this.pipsheetService.GetPIPSheetVersionData(comment.ProjectId, userName);

            pipSheetVersionMainDTO.PipSheetVersionDTO.ForEach(version =>
            {
                if (version.Status == Constants.Approved)
                {
                    checkIfAnyVersionApproved = true;
                }
            });

            if (!checkIfAnyVersionApproved)
            {
                authStatus = await accountAuthService.GetPipSheetCommentAuth(userName, comment.PIPSheetId);
            }
            if (authStatus)
            {
                return await pipsheetCommentService.SavePIPSheetComment(comment, userName);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Delete PIPSheet Comment
        /// </summary>
        /// <param name="pipSheetCommentId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{pipSheetCommentId}/project/{projectId}")]
        public async Task<ActionResult<bool>> DeletePIPSheetComment(int pipSheetCommentId, int projectId)
        {
            string userName = GetUserName();
            bool checkIfAnyVersionApproved = false;
            bool authStatus = false;
            PipSheetVersionMainDTO pipSheetVersionMainDTO = new PipSheetVersionMainDTO();
            pipSheetVersionMainDTO = await this.pipsheetService.GetPIPSheetVersionData(projectId, userName);

            pipSheetVersionMainDTO.PipSheetVersionDTO.ForEach(version =>
            {
                if (version.Status == Constants.Approved)
                {
                    checkIfAnyVersionApproved = true;
                }
            });

            if (!checkIfAnyVersionApproved)
            {
                authStatus = await accountAuthService.GetDeletePipCommentAuth(userName, pipSheetCommentId);
            }
            if (authStatus)
            {
                return await pipsheetCommentService.DeletePIPSheetComment(pipSheetCommentId, userName);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
