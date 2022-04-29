using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class SharePipService : ISharePipService
    {
        private readonly ISharePipRepository repo;
        private readonly IEmailNotificationService emailNotificationService;
        public SharePipService(ISharePipRepository repo, IEmailNotificationService emailNotificationService)
        {
            this.repo = repo;
            this.emailNotificationService = emailNotificationService;
        }

        public async Task<List<SharePipDTO>> GetSharedPipData(int projectId)
        {
            return await this.repo.GetSharedPipData(projectId);
        }

        public async Task DeleteSharedPipData(int pipSheetId, int roleId, int accountId, int sharedWithUserId, string userName)
        {
            await this.repo.DeleteSharedPipData(pipSheetId, roleId, accountId, sharedWithUserId);

            //formulating object
            SharePipDTO deleteSharedPIP = new SharePipDTO
            {
                PipSheetId = pipSheetId,
                RoleId = roleId,
                AccountId = accountId,
                SharedWithUserId = sharedWithUserId,
            };

            // trigger email 
            await this.InitiateEmailProcess(deleteSharedPIP, OperationType.SharedPIPDelete, userName, null);

        }

        public async Task UpdateSharedPipData(SharePipDTO currentSharedPIP, string userName)
        {
            // pulling previous rights : 
            var previousSharedPIP = await this.GetSharedPipData(currentSharedPIP);

            await this.repo.UpdateSharedPipData(currentSharedPIP);

            if (currentSharedPIP.RoleId != previousSharedPIP.RoleId)
            {
                await this.InitiateEmailProcess(currentSharedPIP, OperationType.SharedPIPUpdate, userName, previousSharedPIP);
            }
        }

        public async Task<bool> SaveSharedPipData(List<SharePipDTO> sharedPip, string userName)
        {
            bool result = await this.repo.SaveSharedPipData(sharedPip, userName);
            if (!result)
            {
                // extract email candidates : 
                var receivers = sharedPip.GroupBy(x => x.SharedWithUserId).Select(group => group.First());

                // extract items shared 
                List<SharePipDTO> pipsheets = sharedPip.GroupBy(x => x.PipSheetId).Select(group => group.First()).ToList();

                foreach (SharePipDTO receiver in receivers)
                {
                    await this.InitiateEmailProcess(receiver, OperationType.SharedPIP, userName, pipsheets);
                }
            }

            return result;
        }

        public async Task<SharePipVersionDTO> GetSharePipVersionData(int projectId)
        {
            return await this.repo.GetSharePipVersionData(projectId);
        }

        private async Task InitiateEmailProcess(dynamic operationDTO, OperationType operationType, string senderEmaild, dynamic sharedPIP)
        {
            EmailDTO emailDto = await emailNotificationService.PreComposeEmail(operationDTO, operationType, senderEmaild);
            var processedEmailData = await emailNotificationService.ProcessEmail(emailDto);
            emailDto = await emailNotificationService.PostComposeEmail(processedEmailData, sharedPIP, operationType);
            await emailNotificationService.SendEmail(processedEmailData);
        }

        public async Task<SharePipDTO> GetSharedPipData(SharePipDTO sharedPIP)
        {
            return await repo.GetSharedPipData(sharedPIP);
        }
    }
}
