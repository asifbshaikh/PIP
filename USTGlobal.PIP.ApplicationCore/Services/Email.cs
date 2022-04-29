using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class Email : IEmail
    {
        #region Private Variables

        private readonly SmtpModel smtpModel;
        private MailMessage mailMessage;
        private SmtpClient smtpClient;
        private readonly IEmailNotificationRepository emailNotificationRepository;
        private readonly IConfiguration configuration;

        #endregion Private Variables

        /// <summary>Initializes a new instance of the <see cref="Email"/> class</summary>
        /// <param name="_smtpModel">SMTP Model</param>
        public Email(SmtpModel _smtpModel, IEmailNotificationRepository emailNotificationRepository,
            IConfiguration configuration)
        {
            // Initializing smtp model
            smtpModel = _smtpModel;
            this.emailNotificationRepository = emailNotificationRepository;
            this.configuration = configuration;
        }

        /// <summary>Sends the mail asynchronously</summary>
        /// <param name="emailModel">Email Model</param>
        public async Task SendMailAsync(EmailModel emailModel, string reportFilePath, EmailDTO emailDTO)
        {
            // Try-catch block for event logging
            try
            {
                SetModelProperties(emailModel);
                await smtpClient.SendMailAsync(mailMessage);
                if (File.Exists(reportFilePath))
                {
                    //File.Delete(reportFilePath);
                }
                if (emailDTO.TemplateData.OperationName == Constants.ReportOperation)
                {
                    // Logic to delete the excel sheet
                    //FileInfo reportFile = emailDTO.ReportFile;
                    //reportFile.Delete();
                }
                else
                {
                    await this.emailNotificationRepository.SaveEmail(emailDTO, EmailStatus.Success, Constants.SuccessMessage);
                }
            }
            catch (Exception ex)
            {
                await this.emailNotificationRepository.SaveEmail(emailDTO, EmailStatus.Failed, ex.InnerException + ex.Message);
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
                //eventLogger.LogError("Error In Sending Email | Exception : {0}", exception);
                throw;
            }
        }

        /// <summary>Sets the model properties</summary>
        /// <param name="emailModel">Email Model</param>
        private void SetModelProperties(EmailModel emailModel)
        {
            int portNumber = Convert.ToInt32(configuration.GetSection("EmailConfiguration").GetSection("PortNumber").Value);
            bool enableSSL = Convert.ToBoolean(configuration.GetSection("EmailConfiguration").GetSection("EnableSSL").Value);
            string smtpAddress = configuration.GetSection("EmailConfiguration").GetSection("SmtpAddress").Value;
            string username = configuration.GetSection("EmailConfiguration").GetSection("Username").Value;
            string password = configuration.GetSection("EmailConfiguration").GetSection("Password").Value;
            string domain = configuration.GetSection("EmailConfiguration").GetSection("Domain").Value;
            string displayName = configuration.GetSection("EmailConfiguration").GetSection("DisplayName").Value; ;

            mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailModel.From, displayName);
            mailMessage.Subject = emailModel.Subject;
            mailMessage.Body = emailModel.Body;
            mailMessage.IsBodyHtml = emailModel.IsBodyHTML;
            mailMessage.Priority = emailModel.MailPriority;

            mailMessage.To.Add(emailModel.To);
            emailModel.Cc.ForEach(c => mailMessage.CC.Add(c));
            emailModel.Bcc.ForEach(b => mailMessage.Bcc.Add(b));
            emailModel.Attachments.ForEach(a => mailMessage.Attachments.Add(a));

            smtpClient = new SmtpClient(smtpAddress, portNumber);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(username, password, domain);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = enableSSL;
        }
    }
}
