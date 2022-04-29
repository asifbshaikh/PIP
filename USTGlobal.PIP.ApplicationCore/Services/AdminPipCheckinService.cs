using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class AdminPipCheckinService : IAdminPipCheckinService
    {
        private readonly IAdminPipCheckinRepository adminPipCheckinRepository;
        private readonly IEmailNotificationService emailNotificationService;
        public AdminPipCheckinService(IAdminPipCheckinRepository adminPipCheckinRepository, IEmailNotificationService emailNotificationService)
        {
            this.adminPipCheckinRepository = adminPipCheckinRepository;
            this.emailNotificationService = emailNotificationService;
        }

        public async Task<IList<AccountBasedProjectDTO>> GetAccountBasedProjects(int accountId)
        {
            return await this.adminPipCheckinRepository.GetAccountBasedProjects(accountId);
        }

        public async Task<IList<CheckOutPipVersionDTO>> GetCheckedOutVersions(int projectId)
        {
            return await this.adminPipCheckinRepository.GetCheckedOutVersions(projectId);
        }

        public async Task SaveCheckedInVersions(IList<CheckOutPipVersionDTO> checkInPipVersions, string userName)
        {
            await this.adminPipCheckinRepository.SaveCheckedInVersions(checkInPipVersions, userName);
            var pipsheets = checkInPipVersions.GroupBy(x => x.CheckedOutByUID).Select(group => group.First());

            foreach (var pipsheet in pipsheets)
            {
                List<int> pipVersions = new List<int>();
                pipVersions.AddRange(checkInPipVersions.Where(version => version.CheckedOutByUID == pipsheet.CheckedOutByUID)
                       .Select(x => x.VersionNumber));

                await this.InitiateEmailProcess(pipsheet, OperationType.AdminCheckin, userName, pipVersions);
            }
        }

        private async Task InitiateEmailProcess(dynamic operationDTO, OperationType operationType, string senderEmaild, List<int> versions = null)
        {
            EmailDTO emailDto = await emailNotificationService.PreComposeEmail(operationDTO, operationType, senderEmaild);
            var processedEmailData = await emailNotificationService.ProcessEmail(emailDto);
            processedEmailData.Versions = versions;
            emailDto = await emailNotificationService.PostComposeEmail(processedEmailData, versions, operationType);
            await emailNotificationService.SendEmail(emailDto);
        }
    }
}
