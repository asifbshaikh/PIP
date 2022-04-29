using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using USTGlobal.PIP.EmailScheduler.DTOs;
using USTGlobal.PIP.EmailScheduler.Enum;
using USTGlobal.PIP.EmailScheduler.Helpers;
using USTGlobal.PIP.EmailScheduler.Interfaces;
using USTGlobal.PIP.EmailScheduler.Services;

namespace USTGlobal.PIP.EmailScheduler
{
    class Program
    {
        private static EmailDTO PreComposeEmail()
        {
            EmailDTO emailDTO = null;
            try
            {
                emailDTO = new EmailDTO();
                emailDTO.TemplateData = new PlaceHolderDTO();
                emailDTO.TemplateData.OperationName = Constants.SchedulerComment;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message.ToString());
            }
            return emailDTO;
        }

        private static PipSheetMainDTO ProcessEmailData(PipContext pipContext, EmailDTO emailDTO)
        {
            PipSheetMainDTO pipSheetMainDTO = null;

            try
            {
                pipSheetMainDTO = new PipSheetMainDTO();
                pipSheetMainDTO.PipSheetCommentDTO = new List<PipSheetCommentDTO>();
                pipSheetMainDTO.PipSheetGroupDTO = new List<PipSheetGroupDTO>();

                pipContext.LoadStoredProc("dbo.sp_ProcessEmailData")
                      .WithSqlParam("@InputTemplateData", new SqlParameter("@InputTemplateData", SqlDbType.Structured)
                      {
                          Value = IListToDataTableHelper.ToDataTable(emailDTO.TemplateData),
                          TypeName = "dbo.TemplateData"
                      })
                     .ExecuteStoredProc((emailResultSet) =>
                     {
                         pipSheetMainDTO.PipSheetCommentDTO = emailResultSet.ReadToList<PipSheetCommentDTO>().ToList();
                         emailResultSet.NextResult();

                         pipSheetMainDTO.PipSheetGroupDTO = emailResultSet.ReadToList<PipSheetGroupDTO>().ToList();
                     });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message.ToString());
            }
            return pipSheetMainDTO;
        }

        private static void PostComposeEmail(PipSheetMainDTO processedEmailData, IServiceScopeFactory scopeFactory, PipContext pipContext, IConfiguration configuration)
        {

            try
            {
                EmailDTO emailDTO = new EmailDTO();
                var comments = processedEmailData.PipSheetCommentDTO.GroupBy(comment =>
                comment.PIPSheetId).ToList();
                var emailIds = processedEmailData.PipSheetGroupDTO.GroupBy(email => email.PipSheetId).ToList();
                string HeaderOrigin = configuration.GetSection("EmailConfiguration").GetSection("HeaderOrigin").Value;

                comments.ForEach(comment =>
                {
                    emailDTO.pipSheetCommentDTO = new List<PipSheetCommentDTO>();
                    emailDTO.TemplateData = new PlaceHolderDTO();
                    emailDTO.pipSheetCommentDTO.AddRange(comment);

                    var emails = emailIds.Find(email => email.Key == comment.Key).ToList();
                    string toRecipients = "";
                    emails.ForEach(email =>
                    {
                        toRecipients += string.Concat(email.EmailId);
                        toRecipients += string.Concat(",");
                    });
                    emailDTO.To = toRecipients.Remove(toRecipients.Length - 1, 1);
                    emailDTO.Subject = "Comments added for PIP" + " " +
                    emailDTO.pipSheetCommentDTO[0].SFProjectId + " " + ", version" + " " +
                    emailDTO.pipSheetCommentDTO[0].VersionNumber;
                    emailDTO.TemplateData.PipSheetStatus = emailDTO.pipSheetCommentDTO[0].PipSheetStatus;
                    emailDTO.Link = HeaderOrigin + Constants.Slash + Constants.Projects + Constants.Slash
                            + emailDTO.pipSheetCommentDTO[0].ProjectId + Constants.Slash +
                            emailDTO.pipSheetCommentDTO[0].PIPSheetId + Constants.Slash +
                            emailDTO.pipSheetCommentDTO[0].AccountId + Constants.Slash +
                            Constants.ReadOnly + Constants.Slash + Constants.Staff;
                    emailDTO.TemplateData.TemplateName = emailDTO.pipSheetCommentDTO[0].TemplateName;
                    emailDTO.TemplateData.OperationName = emailDTO.pipSheetCommentDTO[0].OperationName;
                    emailDTO.TemplateData.SFProjectId = emailDTO.pipSheetCommentDTO[0].SFProjectId;
                    emailDTO.TemplateData.VersionNumber = emailDTO.pipSheetCommentDTO[0].VersionNumber;
                    emailDTO.From = configuration.GetSection("EmailConfiguration").GetSection("From").Value; ;

                    //Send email per PipSheet
                    SendEmail(emailDTO, scopeFactory, pipContext, configuration);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message.ToString());
            }
        }

        private static void SendEmail(EmailDTO emailDTO, IServiceScopeFactory scopeFactory, PipContext pipContext, IConfiguration configuration)
        {
            try
            {
                using (var serviceScope = scopeFactory.CreateScope())
                {
                    var helper = serviceScope.ServiceProvider.GetRequiredService<EmailTemplateService>();
                    emailDTO.htmlString = helper.RenderViewToStringAsync("SchedulerComment", emailDTO);
                }

                SmtpModel smtpModel = new SmtpModel();
                smtpModel.EnableSSL = Convert.ToBoolean(configuration.GetSection("EmailConfiguration").GetSection("EnableSSL").Value);
                smtpModel.SmtpAddress = configuration.GetSection("EmailConfiguration").GetSection("SmtpAddress").Value;
                smtpModel.Username = configuration.GetSection("EmailConfiguration").GetSection("Username").Value;
                smtpModel.Password = configuration.GetSection("EmailConfiguration").GetSection("Password").Value;
                smtpModel.PortNumber = Convert.ToInt32(configuration.GetSection("EmailConfiguration").GetSection("PortNumber").Value);

                EmailModel emailModel = new EmailModel();
                emailModel.From = emailDTO.From;
                emailModel.DisplayName = configuration.GetSection("EmailConfiguration").GetSection("DisplayName").Value;
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

                IEmail email = new Email(smtpModel);
                email.SendMail(emailModel);
                UpdateCommentStatus(emailDTO, pipContext);
                SaveEmail(emailDTO, EmailStatus.Success, Constants.SuccessMessage, pipContext);
            }
            catch (Exception ex)
            {
                SaveEmail(emailDTO, EmailStatus.Failed, ex.InnerException + ex.Message, pipContext);
                Log.Error(ex, ex.Message.ToString());
            }
        }

        private static void UpdateCommentStatus(EmailDTO emailDTO, PipContext pipContext)
        {
            try
            {
                emailDTO.pipSheetCommentDTO.ForEach(comment => { comment.EmailStatusId = 1; });

                pipContext.Database.ExecuteSqlCommand(" exec dbo.sp_UpdateCommentStatus {0} ",
                 new SqlParameter("@InputPipSheetComment", SqlDbType.Structured)
                 {
                     Value = IListToDataTableHelper.ToDataTables(emailDTO.pipSheetCommentDTO),
                     TypeName = "dbo.PipSheetComment"
                 });
                pipContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message.ToString());
            }
        }

        private static void SaveEmail(EmailDTO emailDTO, EmailStatus type, string message, PipContext pipContext)
        {
            try
            {
                pipContext.Database.ExecuteSqlCommand(" exec dbo.sp_SaveEmail {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                  emailDTO.TemplateData.OperationName
                  , emailDTO.htmlString
                  , type
                  , emailDTO.To
                  , emailDTO.Subject
                  , message
                  , emailDTO.TemplateData.SenderEmailId);
                pipContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message.ToString());
            }
        }

        private static IServiceScopeFactory InitializeServices(string customApplicationBasePath = null)
        {
            ServiceProvider serviceProvider = null;
            try
            {
                // Initialize the necessary services
                var services = new ServiceCollection();
                ConfigureDefaultServices(services, customApplicationBasePath);

                serviceProvider = services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message.ToString());
            }
            return serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        private static void ConfigureDefaultServices(IServiceCollection services, string customApplicationBasePath)
        {
            try
            {
                string applicationName;
                IFileProvider fileProvider;
                if (!string.IsNullOrEmpty(customApplicationBasePath))
                {
                    applicationName = Path.GetFileName(customApplicationBasePath);
                    fileProvider = new PhysicalFileProvider(customApplicationBasePath);
                }
                else
                {
                    applicationName = Assembly.GetEntryAssembly().GetName().Name;
                    fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                }

                services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
                {
                    ApplicationName = applicationName,
                    WebRootFileProvider = fileProvider,
                });

                services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.FileProviders.Clear();
                    options.FileProviders.Add(fileProvider);
                });
                var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
                services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
                services.AddSingleton<DiagnosticSource>(diagnosticSource);
                services.AddLogging();
                services.AddMvc();
                services.AddTransient<EmailTemplateService>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message.ToString());
            }
        }

        static void Main(string[] args)
        {
            IConfiguration configuration = SetupStaticLogger();

            try
            {
                var serviceScopeFactory = InitializeServices();
                PipContext pipContext = new PipContext(configuration.GetSection("EmailConfiguration").GetSection("ConnectionString").Value);
                EmailDTO emailDTO = PreComposeEmail();

                PipSheetMainDTO processedEmailData = ProcessEmailData(pipContext, emailDTO);
                if (processedEmailData.PipSheetCommentDTO.Count > 0 && processedEmailData.PipSheetGroupDTO.Count > 0)
                {
                    PostComposeEmail(processedEmailData, serviceScopeFactory, pipContext, configuration);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration SetupStaticLogger()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return configuration;
        }
    }
}
