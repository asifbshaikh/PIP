using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IEmailNotificationService
    {
        Task<EmailDTO> ProcessEmail(EmailDTO emailDTO);
        Task<bool> SendEmail(EmailDTO emailDTO);
        Task UpdateEmailStatus();
        Task<EmailDTO> PreComposeEmail(dynamic operationDTO, OperationType type, string senderEmailId, int AccountId = 0);
        Task<EmailDTO> PostComposeEmail(EmailDTO processedEmailObject, dynamic operationDTO, OperationType type);
    }
}
