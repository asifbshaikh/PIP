using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Reflection;
using USTGlobal.PIP.Api.Helpers;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using USTGlobal.PIP.ApplicationCore.Services;
using USTGlobal.PIP.ApplicationCore.WorkflowServices;
using USTGlobal.PIP.Infrastructure.Data;

namespace USTGlobal.PIP.Api
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string AllowSpecificOrigins = "AllowSpecificOrigins";

        /// <summary>
        /// IConfiguration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.AddLogging(configure => configure.AddSerilog());

            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));

            IdentityModelEventSource.ShowPII = true;

            services.AddCors(options =>
            {
                options.AddPolicy(AllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200",
                                    "https://xipl0515.xpanxion.co.in",
                                    "https://xipl0515.xpanxion.co.in:555",
                                    "https://pipdigital.ustdev.com",
                                    "https://pipdigital.ust-global.com")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials();
                });
            });

            services.AddDbContext<PipContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "PIP WebAPI"
                });
                c.IncludeXmlComments(GetXmlCommentsPath());
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                        },
                        new string[] {}
                    }
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            AddHelpers(services);
            AddRepos(services);
            AddServices(services);
        }

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<SerilogMiddleware>();
            app.UseAuthentication();
            app.UseCors(AllowSpecificOrigins);
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            //if (env.IsDevelopment())
            //{
            //    // Enable middleware to serve generated Swagger as a JSON endpoint.
            //    app.UseSwagger();

            //    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            //    // specifying the Swagger JSON endpoint.
            //    app.UseSwaggerUI(c =>
            //    {
            //        string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
            //        c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "PIP WebAPI");
            //    });
            //}
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "PIP WebAPI");
            });

            app.UseMvc();
        }

        /// <summary>
        /// AddRepos
        /// </summary>
        /// <param name="services"></param>
        public void AddRepos(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMasterRepository, MasterRepository>();
            services.AddScoped<IHolidayRepository, HolidayRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IPipSheetRepository, PipSheetRepository>();
            services.AddScoped<IResourcePlanningRepository, ResourcePlanningRepository>();
            services.AddScoped<IProjectHeaderRepository, ProjectHeaderRepository>();
            services.AddScoped<IEbitdaRepository, EbitdaRepository>();
            services.AddScoped<IPipVersionRepository, PipVersionRepository>();
            services.AddScoped<ILaborPricingRepository, LaborPricingRepository>();
            services.AddScoped<IVacationAbsencesRepository, VacationAbsencesRepository>();
            services.AddScoped<IPriceAdjustmentYoyRepository, PriceAdjustmentYoyRepository>();
            services.AddScoped<IExpenseAndAssetRepository, ExpenseAndAssetRepository>();
            services.AddScoped<IPartnerCostAndRevenueRepository, PartnerCostAndRevenueRepository>();
            services.AddScoped<IOtherPriceAdjustmentRepository, OtherPriceAdjustmentRepository>();
            services.AddScoped<IReimbursementAndSalesRepository, ReimbursementAndSalesRepository>();
            services.AddScoped<IRiskManagementRepository, RiskManagementRepository>();
            services.AddScoped<IFixBidAndMarginRepository, FixBidAndMarginRepository>();
            services.AddScoped<IClientPriceRepository, ClientPriceRepository>();
            services.AddScoped<ICapitalChargeRepository, CapitalChargeRepository>();
            services.AddScoped<ISummaryRepository, SummaryRepository>();
            services.AddScoped<IAdminMasterRepository, AdminMasterRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ISharedRepository, SharedRepository>();
            services.AddScoped<IApproverRepository, ApproverRepository>();
            services.AddScoped<IApproverRepository, ApproverRepository>();
            services.AddScoped<ISharePipRepository, SharePipRepository>();
            services.AddScoped<IReplicateRepository, ReplicatePIPSheetRepository>();
            services.AddScoped<IPIPSheetCommentRepository, PIPSheetCommentRepository>();
            services.AddScoped<IAdminPipCheckinRepository, AdminPipCheckinRepository>();
            services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IAccountAuthRepository, AccountAuthRepository>();
        }

        /// <summary>
        /// AddServices
        /// </summary>
        /// <param name="services"></param>
        public void AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMasterService, MasterService>();
            services.AddScoped<IHolidayService, HolidayService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IPipSheetService, PipSheetService>();
            services.AddScoped<IResourcePlanningService, ResourcePlanningService>();
            services.AddScoped<IProjectHeaderService, ProjectHeaderService>();
            services.AddScoped<IEbitdaService, EbitdaService>();
            services.AddScoped<IPipVersionService, PipVersionService>();
            services.AddScoped<ILaborPricingService, LaborPricingService>();
            services.AddScoped<IVacationAbsencesService, VacationAbsencesService>();
            services.AddScoped<IPriceAdjustmentYoyService, PriceAdjustmentYoyService>();
            services.AddScoped<IExpenseAndAssetService, ExpenseAndAssetService>();
            services.AddScoped<IPartnerCostAndRevenueService, PartnerCostAndRevenueService>();
            services.AddScoped<IOtherPriceAdjustmentService, OtherPriceAdjustmentService>();
            services.AddScoped<IReimbursementAndSalesService, ReimbursementAndSalesService>();
            services.AddScoped<IRiskManagementService, RiskManagementService>();
            services.AddScoped<IFixBidAndMarginService, FixBidAndMarginService>();
            services.AddScoped<IClientPriceService, ClientPriceService>();
            services.AddScoped<ICapitalChargeService, CapitalChargeService>();
            services.AddScoped<ISummaryService, SummaryService>();
            services.AddScoped<IEbitdaWorkflowService, EbitdaWorkflowService>();
            services.AddScoped<ILaborPricingWorkflowService, LaborPricingWorkflowService>();
            services.AddScoped<IVacationAbsenceWorkflowService, VacationAbsenceWorkflowService>();
            services.AddScoped<IColaWorkflowService, ColaWorkflowService>();
            services.AddScoped<IExpensesAndAssetsWorkflowService, ExpensesAndAssetsWorkflowService>();
            services.AddScoped<IPartnerCostAndRevenueWorkflowService, PartnerCostAndRevenueWorkflowService>();
            services.AddScoped<IReimbursementAndSalesWorkflowService, ReimbursementAndSalesWorkflowService>();
            services.AddScoped<IOtherPriceAdjustmentWorkflowService, OtherPriceAdjustmentWorkflowService>();
            services.AddScoped<IRiskManagementWorkflowService, RiskManagementWorkflowService>();
            services.AddScoped<IClientPriceWorkflowService, ClientPriceWorkflowService>();
            services.AddScoped<ICapitalChargeWorkflowService, CapitalChargeWorkflowService>();
            services.AddScoped<IProjectHeaderWorkflowService, ProjectHeaderWorkflowService>();
            services.AddScoped<IResourcePlanningWorkflowService, ResourcePlanningWorkflowService>();
            services.AddScoped<IProjectControlWorkflowService, ProjectControlWorkflowService>();
            services.AddScoped<IAdminMasterService, AdminMasterService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IApproverService, ApproverService>();
            services.AddScoped<ISharedService, SharedService>();
            services.AddScoped<ISharePipService, SharePipService>();
            services.AddScoped<IReplicatePIPSheetService, ReplicatePIPSheetService>();
            services.AddScoped<IPIPSheetCommentService, PIPSheetCommentService>();
            services.AddScoped<IAdminPipCheckinService, AdminPipCheckinService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IUploadExcelService, UploadExcelService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IEmail, Email>();
            services.AddScoped<SmtpModel>();
            services.AddScoped<IAccountAuthService, AccountAuthService>();
        }

        /// <summary>
        /// AddHelpers
        /// </summary>
        /// <param name="services"></param>
        public void AddHelpers(IServiceCollection services)
        {
            services.AddScoped<ITokenHelper, TokenHelper>();
        }

        /// <summary>
        /// GetXmlCommentsPath
        /// </summary>
        /// <returns></returns>
        private string GetXmlCommentsPath()
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            return xmlPath;
        }

    }
}
