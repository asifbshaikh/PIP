using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository;
        }

        public async Task<ProjectMainDTO> GetProjectsList(string userName)
        {
            ProjectListMainDTO projectListMainDTO = new ProjectListMainDTO();
            List<int> projectIds = new List<int>();
            ProjectMainDTO projectMainDTO = new ProjectMainDTO();
            projectMainDTO.ProjectDTO = new List<ProjectDTO>();
            projectListMainDTO = await this.projectRepository.GetProjectsList(userName);

            // statusbased pipsheets          
            List<ProjectDTO> notSubmittedPipsheets = new List<ProjectDTO>();
            List<ProjectDTO> approvedPendingPipsheets = new List<ProjectDTO>();
            List<ProjectDTO> approvedPipSheets = new List<ProjectDTO>();

            if (projectListMainDTO.ProjectListDTO.Count > 0)
            {
                projectIds = projectListMainDTO.ProjectListDTO.Select(projectList => projectList.ProjectId).Distinct().ToList();

                projectIds.ForEach(projectId =>
                {
                    List<ProjectListDTO> projectListTemp = new List<ProjectListDTO>();
                    ProjectDTO projectDto = new ProjectDTO();
                    projectListTemp = projectListMainDTO.ProjectListDTO.FindAll(projectDataWithSamePId => projectDataWithSamePId.ProjectId == projectId);
                    if (projectListTemp.Count > 0)
                    {
                        projectDto.ProjectId = projectListTemp[0].ProjectId;
                        projectDto.SFProjectId = projectListTemp[0].SFProjectId;
                        projectDto.ProjectName = projectListTemp[0].ProjectName;
                        projectDto.AccountId = projectListTemp[0].AccountId;
                        projectDto.AccountName = projectListTemp[0].AccountName;
                        projectDto.ServiceLine = projectListTemp[0].ServiceLine;
                        projectDto.DeliveryType = projectListTemp[0].DeliveryType;
                        projectDto.BillingType = projectListTemp[0].BillingType;
                        projectDto.PipSheetId = projectListTemp[0].PipSheetId;
                        projectDto.IsDummy = projectListTemp[0].IsDummy;
                        projectDto.CurrencyId = projectListTemp[0].CurrencyId;
                        projectDto.PipSheetStatus = projectListTemp[0].PipSheetStatus;

                        //  segregation here 
                        if (projectDto.PipSheetStatus.Equals("Not Submitted"))
                        {
                            notSubmittedPipsheets.Add(projectDto);
                        }
                        else if (projectDto.PipSheetStatus.Equals("Approval Pending"))
                        {
                            approvedPendingPipsheets.Add(projectDto);
                        }
                        else
                        {
                            approvedPipSheets.Add(projectDto);
                        }
                    }
                });
                projectMainDTO.ProjectDTO.AddRange(notSubmittedPipsheets);
                projectMainDTO.ProjectDTO.AddRange(approvedPendingPipsheets);
                projectMainDTO.ProjectDTO.AddRange(approvedPipSheets);
            }
            projectMainDTO.IsEditor = projectListMainDTO.IsEditor;

            return projectMainDTO;
        }
    }
}
