using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class PipVersionService : IPipVersionService
    {
        private readonly IPipVersionRepository pipVersionRepository;
        private readonly IEmailNotificationService emailNotificationService;
        private readonly ISharePipService sharePipService;
        public PipVersionService(IPipVersionRepository repo, IEmailNotificationService emailNotificationService, ISharePipService sharePipService)
        {
            this.pipVersionRepository = repo;
            this.emailNotificationService = emailNotificationService;
            this.sharePipService = sharePipService;
        }

        public async Task<int> CreateNewPipSheetVersion(string userName, int projectId, int pipsheetId)
        {
            return await pipVersionRepository.CreateNewPipSheetVersion(userName, projectId, pipsheetId);
        }

        public async Task DeletePipSheet(int pipSheetId, int projectId, string userName)
        {
            var sharedPIPData = await this.sharePipService.GetSharedPipData(projectId);
            await pipVersionRepository.DeletePipSheet(pipSheetId, projectId, userName);

            List<SharePipDTO> sharedPIPDetails = sharedPIPData.Where(p => p.PipSheetId == pipSheetId).ToList();

            if (sharedPIPDetails.Count > 0)
            {
                await this.InitiateEmailProcess(sharedPIPDetails, OperationType.PIPVersionDelete, userName);
            }


        }

        public async Task<SummaryPipVersionDTO> GetVersionDetailsOnSummary(int pipSheetId)
        {
            return await pipVersionRepository.GetVersionDetailsOnSummary(pipSheetId);
        }
        private async Task InitiateEmailProcess(dynamic operationDTO, OperationType operationType, string senderEmaild)
        {
            EmailDTO emailDto = await emailNotificationService.PreComposeEmail(operationDTO[0].ProjectId, operationType, senderEmaild);
            var processedEmailData = await emailNotificationService.ProcessEmail(emailDto);
            emailDto = await emailNotificationService.PostComposeEmail(processedEmailData, operationDTO, operationType);

            //  send email 
            await emailNotificationService.SendEmail(emailDto);
        }
    }
}
