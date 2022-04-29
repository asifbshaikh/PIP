using System.Threading.Tasks;
using USTGlobal.PIP.EmailScheduler.Helpers;

namespace USTGlobal.PIP.EmailScheduler.Interfaces
{
    public interface IEmail
    {
        /// <summary>
        /// Sends the mail asynchronously
        /// </summary>
        /// <param name="templateEmail"></param>
        /// <returns></returns>
        Task SendMailAsync(EmailModel templateEmail);

        /// <summary>
        /// Sends the mail synchronously
        /// </summary>
        /// <param name="templateEmail"></param>
        void SendMail(EmailModel templateEmail);
    }
}
