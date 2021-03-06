using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ProjectHeaderService : IProjectHeaderService
    {
        private readonly IProjectHeaderRepository projectHeaderRepository;
        public ProjectHeaderService(IProjectHeaderRepository projectHeaderRepository)
        {
            this.projectHeaderRepository = projectHeaderRepository;
        }

        public async Task<ProjectHeaderCurrencyDTO> GetProjectHeaderData(int projectId, int pipSheetId)
        {
            return await this.projectHeaderRepository.GetProjectHeaderData(projectId, pipSheetId);
        }

        public async Task<RouteParamDTO> SaveProjectHeaderData(string userName, ProjectHeaderDTO projectHeader)
        {
            return await this.projectHeaderRepository.SaveProjectHeaderData(userName, projectHeader);
        }
        public async Task<CurrencyDTO> GetCurrencyConversionData(int countryId)
        {
            return await this.projectHeaderRepository.GetCurrencyConversionData(countryId);
        }
        public async Task<WorkflowStatusAndAccountSpecificRoleDTO> GetWorkflowStatusAccountRole(string userName, int pipSheetId, int accountId, int projectId = 0)
        {
            return await this.projectHeaderRepository.GetWorkflowStatusAccountRole(userName, pipSheetId, accountId, projectId);
        }

        public async Task<RoleAndAccountMainDTO> GetUserRoleForAllAccounts(string userName)
        {
            return await this.projectHeaderRepository.GetUserRoleForAllAccounts(userName);
        }

        public async Task<string> GetAutoGeneratedProjectId(int accountId, string accountCode)
        {
            return await this.projectHeaderRepository.GetAutoGeneratedProjectId(accountId, accountCode);
        }

        public async Task<List<ProjectDTO>> GetProjectsByAccountId(int accountId)
        {
            return await this.projectHeaderRepository.GetProjectsByAccountId(accountId);
        }
    }
}
