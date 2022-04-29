using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using USTGlobal.PIP.EmailScheduler.Helpers;
using USTGlobal.PIP.EmailScheduler.Interfaces;

namespace USTGlobal.PIP.EmailScheduler.Services
{
    public class Email : IEmail
    {
        #region Private Variables

        private readonly SmtpModel smtpModel;
        private MailMessage mailMessage;
        private SmtpClient smtpClient;

        #endregion Private Variables

        /// <summary>Initializes a new instance of the <see cref="Email"/> class</summary>
        /// <param name="_smtpModel">SMTP Model</param>
        public Email(SmtpModel _smtpModel)
        {
            // Initializing smtp model
            smtpModel = _smtpModel;
        }

        /// <summary>Sends the mail asynchronously</summary>
        /// <param name="emailModel">Email Model</param>
        public async Task SendMailAsync(EmailModel emailModel)
        {
            try
            {
                SetModelProperties(emailModel);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>Sends the mail synchronously</summary>
        /// <param name="emailModel">Email Model</param>
        public void SendMail(EmailModel emailModel)
        {
            // Try-catch block for event logging
            try
            {
                SetModelProperties(emailModel);
                smtpClient.Send(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>Sets the model properties</summary>
        /// <param name="emailModel">Email Model</param>
        private void SetModelProperties(EmailModel emailModel)
        {
            mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailModel.From, emailModel.DisplayName);
            mailMessage.Subject = emailModel.Subject;
            mailMessage.Body = emailModel.Body;
            mailMessage.IsBodyHtml = emailModel.IsBodyHTML;
            mailMessage.Priority = emailModel.MailPriority;

            mailMessage.To.Add(emailModel.To);
            emailModel.Cc.ForEach(c => mailMessage.CC.Add(c));
            emailModel.Bcc.ForEach(b => mailMessage.Bcc.Add(b));
            emailModel.Attachments.ForEach(a => mailMessage.Attachments.Add(a));

            smtpClient = new SmtpClient(smtpModel.SmtpAddress, smtpModel.PortNumber);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpModel.Username, smtpModel.Password, "Xpanxion.com");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = smtpModel.EnableSSL;
        }
    }
}
