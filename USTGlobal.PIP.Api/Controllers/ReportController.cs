using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;



namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// ReportController
    /// </summary>
    [AuthorizeRolePermission(AuthorizeRoles = "Finance Approver,Editor")]
    [Route("api/report")]
    public class ReportController : BaseController
    {
        /// <summary>
        /// IConfiguration
        /// </summary>
        public IConfiguration configuration { get; }

        private readonly IReportService reportService;
        private readonly IAccountAuthService accountAuthService;

        /// <summary>
        /// ReportController Constructor
        /// </summary>
        /// <param name="reportService"></param>
        /// <param name="configuration"></param>
        /// <param name="accountAuthService"></param>
        public ReportController(IReportService reportService, IConfiguration configuration, IAccountAuthService accountAuthService)
        {
            this.reportService = reportService;
            this.configuration = configuration;
            this.accountAuthService = accountAuthService;
        }

        /// <summary>
        /// Get Projects based on Accounts selected
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("account/projects")]
        public async Task<ActionResult<List<AccountBasedProjectDTO>>> GetAccountBasedProjects([FromBody] IList<AccountDTO> accountsList)
        {
            bool isAuthorized = false;
            string userName = GetUserName();
            List<RoleAndAccountDTO> roleAndAccounts = await accountAuthService.GetUserAccountLevelRoles(userName);

            if (roleAndAccounts.Find(x => x.RoleId == 2) != null)           // Finance Approver
            {
                isAuthorized = true;
            }
            else
            {
                foreach (var account in accountsList)
                {
                    if (roleAndAccounts.Find(x => x.AccountId == account.AccountId && x.RoleId == 3) != null)
                    {
                        isAuthorized = true;
                    }
                    else
                    {
                        isAuthorized = false;
                        break;
                    }
                }
            }

            if (isAuthorized)
                return await this.reportService.GetAccountBasedProjects(accountsList);
            else
                return Unauthorized();
        }

        /// <summary>
        /// Get CustomReport KPI List
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("customReportKPI")]
        public async Task<ActionResult<List<ReportKPIDTO>>> GetCustomReportKPIList()
        {
            string userName = GetUserName();
            List<RoleAndAccountDTO> roleAndAccounts = await accountAuthService.GetUserAccountLevelRoles(userName);

            if (roleAndAccounts.Find(x => x.RoleId == 2 || x.RoleId == 3) != null)           // Finance Approver
            {
                return await this.reportService.GetCustomReportKPIList();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Generate Report based on Report Type
        /// Report Type : 1 => Project Summary View Report, 2 => Project Detailed Level Report, 3 => Project Resource Level, 4 => Custom Report
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [HttpPost, Route("project")]
        public async Task<ActionResult<bool>> GenerateProjectReport([FromBody] ReportRequestDTO report)
        {
            string userName = GetUserName();
            bool isReportGenerationAccess = false;

            List<RoleAndAccountDTO> roleAndAccounts = await accountAuthService.GetUserAccountLevelRoles(userName);
            if (roleAndAccounts.Find(x => x.RoleId == 2) != null)           // Finance Approver
            {
                isReportGenerationAccess = true;
            }
            else
            {
                foreach (var account in report.SelectedAccounts)
                {
                    if (roleAndAccounts.Find(x => x.AccountId == account.AccountId && x.RoleId == 3) != null)        // If Editor for any account
                    {
                        isReportGenerationAccess = true;
                        break;
                    }
                }
            }

            if (isReportGenerationAccess)
            {
                string reportsFolderPath = configuration.GetValue<string>("ReportsPath:path");
                return await this.reportService.GenerateProjectReport(report, userName, reportsFolderPath);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get Authorized Accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("account/authorized")]
        public async Task<ActionResult<List<AccountId>>> GetAuthorizedAccounts()
        {
            string userName = GetUserName();
            List<RoleAndAccountDTO> roleAndAccounts = await accountAuthService.GetUserAccountLevelRoles(userName);

            if (roleAndAccounts.Find(x => x.RoleId == 2 || x.RoleId == 3) != null)           // Finance Approver
            {
                return await this.reportService.GetAuthorizedAccounts(userName);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
