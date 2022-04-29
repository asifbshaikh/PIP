using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class EmailNotificationRepository : IEmailNotificationRepository
    {
        private readonly PipContext pipContext;
        private readonly IEmailTemplateService emailTemplateService;
        private readonly IConfiguration configuration;

        public EmailNotificationRepository(PipContext pipContext, IEmailTemplateService emailTemplateService,
            IConfiguration configuration)
        {
            this.pipContext = pipContext;
            this.emailTemplateService = emailTemplateService;
            this.configuration = configuration;
        }

        public async Task<EmailDTO> ProcessEmail(EmailDTO processEmailDTO)
        {
            EmailDTO processedEmailDTO = new EmailDTO();
            EmailIdDTO emailIdDTO = new EmailIdDTO();

            await pipContext.LoadStoredProc("dbo.sp_ProcessEmailData")
                  .WithSqlParam("@InputTemplateData", new SqlParameter("@InputTemplateData", SqlDbType.Structured)
                  {
                      Value = IListToDataTableHelper.ToDataTable(processEmailDTO.TemplateData),
                      TypeName = "dbo.TemplateData"
                  })
                  .ExecuteStoredProcAsync((emailResultSet) =>
                  {
                      processedEmailDTO.TemplateData = emailResultSet.ReadToList<PlaceHolderDTO>().FirstOrDefault();
                      emailResultSet.NextResult();

                      emailIdDTO = emailResultSet.ReadToList<EmailIdDTO>().FirstOrDefault();
                      processedEmailDTO.To = emailIdDTO.EmailIds;
                  });
            return processedEmailDTO;
        }

        public async Task SaveEmail(EmailDTO emailDTO, EmailStatus type, string message)
        {
            string connectionString = configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            PipContext pipContext1 = new PipContext(true);

            await pipContext1.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveEmail {0}, {1}, {2}, {3}, {4}, {5}, {6}",
               emailDTO.TemplateData.OperationName
               , emailDTO.htmlString
               , type
               , emailDTO.To
               , emailDTO.Subject
               , message
               , emailDTO.TemplateData.SenderEmailId);
            await pipContext1.SaveChangesAsync();
        }

        public Task UpdateEmailStatus()
        {
            throw new NotImplementedException();
        }
    }
}
