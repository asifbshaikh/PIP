using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// ExpenseAndAssetController
    /// </summary>
    [Route("api/expenseAndAsset")]
    public class ExpenseAndAssetController : BaseController
    {
        private readonly IExpenseAndAssetService expenseAndAssetService;
        private readonly IExpensesAndAssetsWorkflowService expenseAndAssetsWorkflowService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// ExpenseAndAssetController Constructor
        /// </summary>
        /// <param name="expenseAndAssetService"></param>
        /// <param name="expenseAndAssetsWorkflowService"></param>
        /// <param name="accountAuthService"></param>
        public ExpenseAndAssetController(IExpenseAndAssetService expenseAndAssetService, IExpensesAndAssetsWorkflowService expenseAndAssetsWorkflowService,
            IAccountAuthService accountAuthService)
        {
            this.expenseAndAssetService = expenseAndAssetService;
            this.expenseAndAssetsWorkflowService = expenseAndAssetsWorkflowService;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Direct Expense and Asset data based on PipSheetId
        /// </summary>
        /// <param name="pipSheetId"></param>
        [HttpGet, Route("{pipSheetId}")]
        public async Task<ActionResult<ExpenseAndAssetDTO>> GetExpenseAndAsset(int pipSheetId)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, pipSheetId, true))
            {
                return await this.expenseAndAssetService.GetExpenseAndAsset(pipSheetId);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Save Direct Expense and Asset data
        /// </summary>
        /// <param name="expenseAndAssetDto"></param>
        [HttpPost]
        public async Task<ActionResult> SaveExpenseAndAssetData([FromBody]ExpenseAndAssetDTO expenseAndAssetDto)
        {
            string userName = GetUserName();
            if (await this.accountAuthService.GetPipSheetLevelAuthorization(userName, expenseAndAssetDto.DirectExpenseDTO[0].PipSheetId, false))
            {
                await this.expenseAndAssetsWorkflowService.ProcessExpenseAndAssetSaving(userName, expenseAndAssetDto);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}