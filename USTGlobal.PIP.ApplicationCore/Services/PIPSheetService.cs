using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class PipSheetService : IPipSheetService
    {
        private readonly IPipSheetRepository pIPSheetRepository;
        private readonly IMasterRepository masterRepository;
        private readonly IResourcePlanningService resourcePlanningService;
        private readonly IEmailNotificationService emailNotificationService;
        private readonly ISummaryService summaryService;

        public PipSheetService(IPipSheetRepository pipsheetRepository, IMasterRepository masterRepository,
            IResourcePlanningService resourcePlanningService, IEmailNotificationService emailNotificationService,
            ISummaryService summaryService)
        {
            this.pIPSheetRepository = pipsheetRepository;
            this.resourcePlanningService = resourcePlanningService;
            this.masterRepository = masterRepository;
            this.emailNotificationService = emailNotificationService;
            this.summaryService = summaryService;
        }

        public async Task<PipSheetDTO> GetByPIPSheetId(int pipSheetId)
        {
            return await pIPSheetRepository.GetByPIPSheetId(pipSheetId);
        }

        public async Task<ProjectControlDTO> GetProjectControlData(int pipSheetId)
        {
            return await pIPSheetRepository.GetProjectControlData(pipSheetId);
        }

        public async Task<bool> SaveProjectControlData(string userName, ProjectControlDTO projectControlDTO)
        {
            int pipsheetId = projectControlDTO.PIPSheetListDTO[0].PIPSheetId;
            return await pIPSheetRepository.SaveProjectControlData(userName, projectControlDTO);
        }

        public async Task UpdatePIPSheetCurrency(int pipSheetId, int currencyId)
        {
            await pIPSheetRepository.UpdatePIPSheetCurrency(pipSheetId, currencyId);
        }

        public async Task<HeaderInfoDTO> GetHeader1Data(int projectId, int pipSheetId)
        {
            HeaderInfoDTO headerInfoDTO = new HeaderInfoDTO();
            headerInfoDTO = await pIPSheetRepository.GetHeader1Data(projectId, pipSheetId);

            //  GrossProfitDTO grossProfitDTO = await this.pIPSheetRepository.GetGrossProfit(pipSheetId);
            if (headerInfoDTO.HeaderEbitda != null)
            {
                headerInfoDTO.HeaderEbitda.FeesAtRisk = headerInfoDTO.HeaderEbitda.FeesAtRisk * (-1);
                headerInfoDTO.HeaderEbitda.EstimatedGrossProfit = headerInfoDTO.HeaderEbitda.TotalNetEstimatedRevenue - headerInfoDTO.HeaderEbitda.TotalProjectCost;

                //Conditions to calculate project gpm percent
                headerInfoDTO.HeaderEbitda.ProjectGPMPercent = CalculateGPPercent(headerInfoDTO.HeaderEbitda.TotalProjectCost, headerInfoDTO.HeaderEbitda.TotalNetEstimatedRevenue);

                //Condition to calculate project EBITDA percent
                if (headerInfoDTO.HeaderEbitda.TotalNetEstimatedRevenue <= 0)
                    headerInfoDTO.HeaderEbitda.ProjectEBITDAPercent = 0;
                else
                    headerInfoDTO.HeaderEbitda.ProjectEBITDAPercent = Math.Round(((headerInfoDTO.HeaderEbitda.EstimatedGrossProfit - headerInfoDTO.HeaderEbitda.SeatCostEbitda) / headerInfoDTO.HeaderEbitda.TotalNetEstimatedRevenue) * 100, 4);
            }
            return headerInfoDTO;
        }

        private decimal CalculateGPPercent(decimal projectCost, decimal estimatedRevenue)
        {
            if (estimatedRevenue - projectCost == 0)
            {
                return 0;
            }
            else if (estimatedRevenue >= 0 && projectCost < 0)
            {
                return 100;
            }
            else if (estimatedRevenue <= 0 && projectCost >= 0)
            {
                return -100;
            }
            else if (projectCost < 0)
            {
                return (-1 * (estimatedRevenue - projectCost) * 100 / Math.Abs(estimatedRevenue == 0 ? 1 : estimatedRevenue));
            }
            else
            {
                return ((estimatedRevenue - projectCost) * 100 / Math.Abs(estimatedRevenue == 0 ? 1 : estimatedRevenue));
            }
        }

        public async Task<PipSheetVersionMainDTO> GetPIPSheetVersionData(int projectId, string userName)
        {
            PipSheetVersionListAndRoleDTO pipSheetVersionListAndRoleDTO = new PipSheetVersionListAndRoleDTO();
            PipSheetVersionMainDTO pipSheetVersionMainDTO = new PipSheetVersionMainDTO();
            pipSheetVersionMainDTO.PipSheetVersionDTO = new List<PipSheetVersionDTO>();
            pipSheetVersionListAndRoleDTO = await pIPSheetRepository.GetPIPSheetVersionData(projectId, userName);

            if (pipSheetVersionListAndRoleDTO.PipSheetVersionListDTO.Count > 0)
            {
                pipSheetVersionListAndRoleDTO.PipSheetVersionListDTO.ForEach(pipSheetVersionList =>
                {
                    PipSheetVersionDTO singlePIPSheetVersionDTO = new PipSheetVersionDTO();
                    singlePIPSheetVersionDTO.ProjectId = pipSheetVersionList.ProjectId;
                    singlePIPSheetVersionDTO.PipSheetId = pipSheetVersionList.PipSheetId;
                    singlePIPSheetVersionDTO.AccountId = pipSheetVersionList.AccountId;
                    singlePIPSheetVersionDTO.VersionNumber = pipSheetVersionList.VersionNumber;
                    singlePIPSheetVersionDTO.Status = pipSheetVersionList.Status;
                    singlePIPSheetVersionDTO.ModifiedBy = pipSheetVersionList.ModifiedBy;
                    singlePIPSheetVersionDTO.ModifiedOn = pipSheetVersionList.ModifiedOn;
                    singlePIPSheetVersionDTO.UserComments = pipSheetVersionList.UserComments;
                    singlePIPSheetVersionDTO.ApproverComments = pipSheetVersionList.ApproverComments;
                    singlePIPSheetVersionDTO.ApprovedBy = pipSheetVersionList.ApprovedBy;
                    singlePIPSheetVersionDTO.ApprovedOn = pipSheetVersionList.ApprovedOn;
                    singlePIPSheetVersionDTO.ResendComments = pipSheetVersionList.ResendComments;
                    singlePIPSheetVersionDTO.ResendBy = pipSheetVersionList.ResendBy;
                    singlePIPSheetVersionDTO.ResendOn = pipSheetVersionList.ResendOn;
                    singlePIPSheetVersionDTO.IsCheckedOut = pipSheetVersionList.IsCheckedOut;
                    singlePIPSheetVersionDTO.CheckedInOutBy = pipSheetVersionList.CheckedInOutBy;
                    singlePIPSheetVersionDTO.CheckedInOutByName = pipSheetVersionList.CheckedInOutByName;
                    singlePIPSheetVersionDTO.ApproverStatusId = pipSheetVersionList.ApproverStatusId;
                    singlePIPSheetVersionDTO.HasAccountLevelAccess = pipSheetVersionList.HasAccountLevelAccess;
                    singlePIPSheetVersionDTO.SFProjectId = pipSheetVersionList.SFProjectId;

                    singlePIPSheetVersionDTO.RoleName = new List<string>();
                    singlePIPSheetVersionDTO.RoleName.AddRange(pipSheetVersionListAndRoleDTO.RoleNameDTO.Where(roleName =>
                    roleName.PipSheetId == pipSheetVersionList.PipSheetId).Select(x => x.RoleName));
                    pipSheetVersionMainDTO.PipSheetVersionDTO.Add(singlePIPSheetVersionDTO);
                });
            }
            pipSheetVersionMainDTO.ProjectWorkflowStatus = pipSheetVersionListAndRoleDTO.ProjectWorkflowStatus;
            return pipSheetVersionMainDTO;
        }

        public async Task<bool> SubmitPIPSheet(PipSheetMainDTO pipSheetMain, string userName)
        {
            SubmitPipSheetDTO submitPipSheetDTO = await this.GetPIPSheetStatus(pipSheetMain);
            if (submitPipSheetDTO.IsSuccess)
            {
                await pIPSheetRepository.SubmitPIPSheet(pipSheetMain, userName);

                OperationType ot = pipSheetMain.IsResend ? OperationType.Resend : OperationType.Revoke;
                if (pipSheetMain.IsResend || pipSheetMain.IsRevise)
                {
                    await this.InitiateEmailProcess(pipSheetMain, ot, userName);
                }
                else
                {
                    PLForecastParentDTO pLForecast = await this.summaryService.GetPLForecastData(pipSheetMain.PIPSheetId);
                    await this.summaryService.SavePLForecastData(userName, pLForecast.PLForecastDTO, pipSheetMain.PIPSheetId);
                }
            }
            return submitPipSheetDTO.IsSuccess;
        }

        public async Task<CurrencyDTO> GetCurrencyConversionData(int pipSheetId)
        {
            return await pIPSheetRepository.GetCurrencyConversionData(pipSheetId);
        }

        public async Task<ProjectControlDTO> ReAssignHoursPerDayHoursPerMonth(int projectId, ProjectControlDTO projectControl)
        {
            List<LocationDTO> locationList = await this.masterRepository.GetLocations(projectId, projectControl.PIPSheetListDTO[0].PIPSheetId);

            foreach (var pl in projectControl.ProjectLocationListDTO)
            {
                pl.HoursPerDay = locationList.Find(x => x.LocationId == pl.LocationId).HoursPerDay;
                pl.HoursPerMonth = locationList.Find(x => x.LocationId == pl.LocationId).HoursPerMonth;
                pl.IsOverride = Constants.IsOverride;
            }
            return projectControl;
        }

        public async Task UpdatePIPSheetCheckIn(PipCheckInDTO pipCheckIn, string userName)
        {
            await pIPSheetRepository.UpdatePIPSheetCheckIn(pipCheckIn, userName);
        }

        public async Task<PipCheckInDTO> GetPipCheckInCheckOut(int pipSheetId)
        {
            return await pIPSheetRepository.GetPipCheckInCheckOut(pipSheetId);
        }

        public async Task<SubmitPipSheetDTO> GetPIPSheetStatus(PipSheetMainDTO pipSheetMain)
        {
            PipSheetStatusDTO pipSheetStatusDTO = new PipSheetStatusDTO();
            pipSheetStatusDTO = await pIPSheetRepository.GetPipsheetStatus(pipSheetMain.PIPSheetId);
            SubmitPipSheetDTO submitPipSheetDTO = new SubmitPipSheetDTO();
            submitPipSheetDTO.ApproverName = pipSheetStatusDTO.ApproversName;
            submitPipSheetDTO.SFProjectId = pipSheetStatusDTO.SFProjectId;

            submitPipSheetDTO = GetPipSheetStatusBasedOnConditions(pipSheetMain, pipSheetStatusDTO);
            if (submitPipSheetDTO.IsAlreadyApproved || submitPipSheetDTO.IsAlreadyResend || submitPipSheetDTO.IsAlreadyRevised)
            {
                return submitPipSheetDTO;
            }
            else
            {
                submitPipSheetDTO.IsSuccess = true;
                return submitPipSheetDTO;
            }
        }

        private SubmitPipSheetDTO GetPipSheetStatusBasedOnConditions(PipSheetMainDTO pipSheetMain, PipSheetStatusDTO pipSheetStatusDTO)
        {
            SubmitPipSheetDTO submitPipSheetDTO = new SubmitPipSheetDTO();
            submitPipSheetDTO.ApproverName = pipSheetStatusDTO.ApproversName;
            submitPipSheetDTO.SFProjectId = pipSheetStatusDTO.SFProjectId;
            if ((pipSheetMain.IsResend && pipSheetStatusDTO.PipSheetStatusId == 1 && pipSheetStatusDTO.ApproverStatusId == 1) ||
                (pipSheetMain.IsApprove && pipSheetStatusDTO.PipSheetStatusId == 1 && pipSheetStatusDTO.ApproverStatusId == 1))
            {
                // already resent
                submitPipSheetDTO.IsSuccess = false;
                submitPipSheetDTO.IsAlreadyResend = true;
            }
            else if ((pipSheetMain.IsResend && pipSheetStatusDTO.PipSheetStatusId == 2 && pipSheetStatusDTO.ApproverStatusId == 2) ||
                (pipSheetMain.IsApprove && pipSheetStatusDTO.PipSheetStatusId == 2 && pipSheetStatusDTO.ApproverStatusId == 2))
            {
                // already approved so cannot resend   
                submitPipSheetDTO.IsSuccess = false;
                submitPipSheetDTO.IsAlreadyApproved = true;
            }
            else if ((pipSheetMain.IsRevise && pipSheetStatusDTO.PipSheetStatusId == 1 && pipSheetStatusDTO.ApproverStatusId == 1) ||
                (pipSheetMain.IsRevise && pipSheetStatusDTO.PipSheetStatusId == 3 && pipSheetStatusDTO.ApproverStatusId == 3))
            {
                // already revised 
                submitPipSheetDTO.IsSuccess = false;
                submitPipSheetDTO.IsAlreadyRevised = true;
            }
            return submitPipSheetDTO;
        }

        private async Task InitiateEmailProcess(dynamic operationDTO, OperationType operationType, string senderEmaild)
        {
            EmailDTO emailDTO = new EmailDTO();
            EmailDTO processedEmailData = new EmailDTO();
            emailDTO = await emailNotificationService.PreComposeEmail(operationDTO, operationType, senderEmaild);
            processedEmailData = await emailNotificationService.ProcessEmail(emailDTO);

            // setting up data directly here 
            processedEmailData = await emailNotificationService.PostComposeEmail(processedEmailData, null, operationType);
            await emailNotificationService.SendEmail(processedEmailData);
        }

        public async Task<List<PipOverrideDTO>> GetPipOverrides(int pipSheetId)
        {
            return await this.pIPSheetRepository.GetPipOverrides(pipSheetId);
        }
    }
}
