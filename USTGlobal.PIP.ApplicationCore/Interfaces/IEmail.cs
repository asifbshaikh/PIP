using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IEmail
    {
        /// <summary>
        /// Sends the mail asynchronously
        /// </summary>
        /// <param name="templateEmail"></param>
        /// <returns></returns>
        Task SendMailAsync(EmailModel templateEmail, string reportFilePath, EmailDTO emailDTO);

        /// <summary>
        /// Sends the mail synchronously
        /// </summary>
        /// <param name="templateEmail"></param>
        void SendMail(EmailModel templateEmail);
    }
}
