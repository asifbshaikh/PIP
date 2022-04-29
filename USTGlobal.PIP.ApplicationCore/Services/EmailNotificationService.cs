using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailNotificationRepository emailNotificationRepository;
        private readonly IEmailTemplateService emailTemplateService;
        private readonly IConfiguration configuration;
        private readonly IEmail email;

        public EmailNotificationService(IEmailNotificationRepository emailNotificationRepository,
            IEmailTemplateService emailTemplateService, IConfiguration configuration,
            IEmail email)
        {
            this.emailNotificationRepository = emailNotificationRepository;
            this.emailTemplateService = emailTemplateService;
            this.configuration = configuration;
            this.email = email;
        }

        public async Task<EmailDTO> ProcessEmail(EmailDTO processEmailDTO)
        {
            EmailDTO processedEmailDTO = new EmailDTO();
            processedEmailDTO = await this.emailNotificationRepository.ProcessEmail(processEmailDTO);
            return processedEmailDTO;
        }

        public async Task<bool> SendEmail(EmailDTO emailDTO)
        {
            var result = string.Empty;
            emailDTO.htmlString = await emailTemplateService.RenderToStringAsync(emailDTO.TemplateData.TemplateName, emailDTO);

            EmailModel emailModel = new EmailModel();
            emailModel.From = emailDTO.From;
            emailModel.DisplayName = configuration.GetSection("EmailConfiguration").GetSection("DisplayName").Value; ;
            emailModel.To = emailDTO.To;
            if (emailDTO.Cc != null)
            {
                emailModel.Cc.Add(emailDTO.Cc);
            }
            if (emailDTO.Bcc != null)
            {
                emailModel.Bcc.Add(emailDTO.Bcc);
            }

            emailModel.Subject = emailDTO.Subject;
            emailModel.MailPriority = MailPriority.High;
            emailModel.Body = emailDTO.htmlString;

            if (emailDTO.TemplateData.OperationName == Constants.ReportOperation)
            {
                if (emailModel.Attachments != null)
                {
                    emailModel.Attachments.Add(new Attachment(emailDTO.ReportFilePath));
                }
            }

            Thread t1 = new Thread(() =>
            {
                email.SendMailAsync(emailModel, emailDTO.ReportFilePath, emailDTO);
            });
            t1.Start();
            return true;
        }

        public async Task UpdateEmailStatus()
        {
            await this.emailNotificationRepository.UpdateEmailStatus();
        }

        public async Task<EmailDTO> PreComposeEmail(dynamic operationDTO, OperationType type, string senderEmailId, int AccountId = 0)
        {
            EmailDTO emailDTO = new EmailDTO
            {
                TemplateData = new PlaceHolderDTO()
            };

            switch (type)
            {
                case OperationType.Resend:
                case OperationType.Revoke:
                    emailDTO.TemplateData.ProjectId = operationDTO.ProjectId;
                    emailDTO.TemplateData.PIPSheetId = operationDTO.PIPSheetId;
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.OperationName = type == OperationType.Resend ? "Resend" : "Revoke";
                    break;

                case OperationType.RoleAndPermission:
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.ReceiverUserId = operationDTO.UserId;
                    emailDTO.TemplateData.OperationName = "E-R-R add";
                    emailDTO.TemplateData.AccountId = operationDTO.AccountId;

                    break;

                case OperationType.AdminAdd:
                case OperationType.FinanceApproverAdd:
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.ReceiverUserId = operationDTO.UserId;
                    emailDTO.TemplateData.AccountId = operationDTO.AccountId;
                    emailDTO.TemplateData.OperationName = operationDTO.fromAdminScreen == true ? "Admin add" : "Finance approver add";

                    break;

                case OperationType.AdminDelete:
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.ReceiverUserId = operationDTO;
                    emailDTO.TemplateData.AccountId = AccountId; // by default Admin Account id is  0
                    emailDTO.TemplateData.OperationName = "Admin delete";
                    break;

                case OperationType.FinanceApproverDelete:
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.ReceiverUserId = operationDTO;
                    emailDTO.TemplateData.AccountId = AccountId;
                    emailDTO.TemplateData.OperationName = "Finance approver delete";
                    break;

                case OperationType.AllReadOnly:
                case OperationType.DeleteAllReadOnly:
                    emailDTO.TemplateData.ReceiverUserId = operationDTO;
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.OperationName = type == OperationType.AllReadOnly ? "Define readonly add" : "Define readonly delete";
                    break;

                case OperationType.AdminCheckin:
                    emailDTO.TemplateData.ReceiverUId = operationDTO.CheckedOutByUID;
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.PIPSheetId = operationDTO.PipSheetId;
                    emailDTO.TemplateData.OperationName = "Check-In";
                    break;

                case OperationType.SharedPIP:
                case OperationType.SharedPIPUpdate:
                case OperationType.SharedPIPDelete:
                    emailDTO.TemplateData.ReceiverUserId = operationDTO.SharedWithUserId;
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.PIPSheetId = operationDTO.PipSheetId;
                    emailDTO.TemplateData.RoleId = operationDTO.RoleId;
                    emailDTO.TemplateData.OperationName = type == OperationType.SharedPIP ? "Shared" :
                                                   (type == OperationType.SharedPIPUpdate ? "Shared right update" : "Unshared");
                    break;

                case OperationType.PIPVersionDelete:
                    emailDTO.TemplateData.ProjectId = operationDTO;
                    emailDTO.TemplateData.SenderEmailId = senderEmailId;
                    emailDTO.TemplateData.OperationName = "Delete PIPSheet Version";
                    break;


                default:
                    break;
            }

            return emailDTO;
        }
        public async Task<EmailDTO> PostComposeEmail(EmailDTO processedEmailObject, dynamic operationDTO, OperationType type)
        {
            int? projectId = processedEmailObject.TemplateData.ProjectId;
            int? pipSheetId = processedEmailObject.TemplateData.PIPSheetId;
            int? accountId = processedEmailObject.TemplateData.AccountId;
            string headerOrigin = configuration.GetSection("EmailConfiguration").GetSection("HeaderOrigin").Value;

            switch (type)
            {
                case OperationType.Resend:
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "PIP" + " " + '"' + processedEmailObject.TemplateData.SFProjectId + '"' +
                        " " + "requested for resubmission";
                    processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Projects + Constants.Slash
                        + projectId + Constants.Slash + pipSheetId + Constants.Slash + accountId + Constants.Slash +
                        Constants.ReadOnly + Constants.Slash + Constants.Staff;
                    break;

                case OperationType.Revoke:
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "PIP" + " " + '"' + processedEmailObject.TemplateData.SFProjectId + '"' +
                        " " + "Approval is revoked";
                    processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Projects + Constants.Slash
                       + projectId + Constants.Slash + pipSheetId + Constants.Slash + accountId + Constants.Slash +
                       Constants.ReadOnly + Constants.Slash + Constants.Staff;
                    break;

                case OperationType.RoleAndPermission:
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "Updated Roles in PIP application";
                    if (processedEmailObject.TemplateData.NewAccessName.Contains("Editor") ||
                        processedEmailObject.TemplateData.NewAccessName.Contains("ReadOnly"))
                    {
                        processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Projects;
                    }
                    else if (processedEmailObject.TemplateData.NewAccessName.Contains("Reviewer"))
                    {
                        processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Approver;
                    }
                    break;

                case OperationType.AdminAdd:
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "Assigned Role of Admin in PIP Application";
                    processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Administration;
                    break;


                case OperationType.AdminDelete:
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "Revoked Admin Rights in PIP application";
                    break;

                case OperationType.FinanceApproverAdd:
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "Assigned Role of Finance Approver in PIP application";
                    processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Approver;
                    break;

                case OperationType.FinanceApproverDelete:
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "Revoked Role of Finance Approver in PIP application";
                    break;

                case OperationType.AllReadOnly:
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "Assigned Read Only access for all Accounts in PIP Application";
                    processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Projects;
                    break;


                case OperationType.DeleteAllReadOnly:
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "Revoked Read Only Access for All Accounts in PIP Application";
                    processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Projects;
                    break;

                case OperationType.AdminCheckin:
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "Check-In Done By Admin";
                    break;

                case OperationType.SharedPIP:
                    processedEmailObject.Versions = new List<int>();
                    processedEmailObject.Roles = new List<string>();
                    foreach (var pip in operationDTO)
                    {
                        processedEmailObject.Versions.Add(pip.VersionNumber);
                        processedEmailObject.Roles.Add(pip.RoleId == 3 ? "Editor" : "Read-Only");
                    }
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "PIP" + " " + processedEmailObject.TemplateData.SFProjectId + " " + "shared with you";
                    processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Projects + Constants.Slash
                       + projectId + Constants.Slash + pipSheetId + Constants.Slash + accountId + Constants.Slash +
                       Constants.ReadOnly + Constants.Slash + Constants.Staff;
                    break;

                case OperationType.SharedPIPUpdate:
                    processedEmailObject.TemplateData.OldAccessName = operationDTO.RoleId == 3 ? "Editor" : "Read-Only";
                    processedEmailObject.TemplateData.NewAccessName = processedEmailObject.TemplateData.RoleId == 3 ? "Editor" : "Read-Only";
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "PIP" + " " + processedEmailObject.TemplateData.SFProjectId + " " + "updated access";
                    processedEmailObject.Link = headerOrigin + Constants.Slash + Constants.Projects + Constants.Slash
                       + projectId + Constants.Slash + pipSheetId + Constants.Slash + accountId + Constants.Slash +
                       Constants.ReadOnly + Constants.Slash + Constants.Staff;
                    break;

                case OperationType.SharedPIPDelete:
                    processedEmailObject.TemplateData.AccessName = processedEmailObject.TemplateData.RoleId == 3 ? "Editor" : "Read-Only";
                    processedEmailObject.To = processedEmailObject.TemplateData.ReceiverEmailId;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "PIP" + " " + processedEmailObject.TemplateData.SFProjectId + " " + "unshared with you";
                    break;


                case OperationType.PIPVersionDelete:
                    string toRecipients = "", to = "";
                    int version = operationDTO[0].VersionNumber;
                    foreach (var item in operationDTO)
                    {
                        toRecipients += string.Concat(item.SharedWithUserEmail);
                        toRecipients += string.Concat(",");
                    }
                    to = toRecipients.Remove(toRecipients.Length - 1, 1);
                    processedEmailObject.To = to;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = "PIP version" + " " + version + " " + "of" + " " +
                        processedEmailObject.TemplateData.SFProjectId + " Deleted";
                    processedEmailObject.TemplateData.VersionNumber = version;
                    break;

                case OperationType.Report:
                    processedEmailObject.To = processedEmailObject.To;
                    processedEmailObject.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                    processedEmailObject.Subject = processedEmailObject.ReportFileName;
                    break;

                default:
                    break;
            }

            return processedEmailObject;
        }
    }
}
