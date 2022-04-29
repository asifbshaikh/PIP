using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// ReimbursementAndSalesController
    /// </summary>
    [Route("api/reimbursementAndSales")]
    public class ReimbursementAndSalesController : BaseController
    {
        private readonly IReimbursementAndSalesService reimbursementAndSalesService;
        private readonly IReimbursementAndSalesWorkflowService reimbursementAndSalesWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// ReimbursementAndSales Constructor
        /// </summary>
        /// <param name="reimbursementAndSalesService"></param>
        /// <param name="reimbursementAndSalesWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public ReimbursementAndSalesController(IReimbursementAndSalesService reimbursementAndSalesService, IReimbursementAndSalesWorkflowService reimbursementAndSalesWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.reimbursementAndSalesService = reimbursementAndSalesService;
            this.reimbursementAndSalesWorkflowService = reimbursementAndSalesWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Reimbursement And Sales data
        /// </summary>
        /// <param name="pipSheetId"></param>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<ReimbursementAndSalesDTO>> GetReimbursementAndSalesDetails(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.reimbursementAndSalesService.GetReimbursementAndSalesDetails(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Reimbursement And Sales data
        /// </summary>
        /// <param name="reimbursementAndSalesDTO"></param>
        [HttpPost]
        public async Task<ActionResult> SaveReimbursementAndSalesDetails([FromBody] ReimbursementAndSalesDTO reimbursementAndSalesDTO)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, reimbursementAndSalesDTO.Reimbursements[0].PipSheetId, false))
            {
                await this.reimbursementAndSalesWorkflowService.ProcessReimbursementAndSalesSaving(userName, reimbursementAndSalesDTO);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}