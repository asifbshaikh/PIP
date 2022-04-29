using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IEmailNotificationRepository
    {
        Task<EmailDTO> ProcessEmail(EmailDTO processEmailDTO);
        Task SaveEmail(EmailDTO emailDTO, EmailStatus type, string message);
        Task UpdateEmailStatus();
    }
}
