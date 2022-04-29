using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using USTGlobal.PIP.ApplicationCore.Services;
using Xunit;

namespace USTGlobal.PIP.ApplicationCore.Test
{
    public class ProjectRepositoryUnitTests
    {
        //string userName = "";

        //[Fact]
        //public void GetProjectsList_Should_ReturnAllProjects()
        //{

        //    //Arrange
        //    List<ProjectListDTO> projectDTO = GetProjectsDTO();

        //    //Act
        //    var projectRepository = Substitute.For<IProjectRepository>();
        //    projectRepository.GetProjectsList(userName).Returns(projectDTO);
        //    var projectService = new ProjectService(projectRepository);

        //    //Assert
        //    //projectService.GetProjectsList().Should().HaveCount(projectDTO.Count);
        //    projectService.GetProjectsList(userName).Should().BeSameAs(projectDTO);
        //}

        //[Fact]
        //public void GetProjectsList_Should_NotBeNullOrEmpty()
        //{
        //    //Arrange
        //    List<ProjectListDTO> projectDTO = GetProjectsDTO();

        //    //Act
        //    var projectRepository = Substitute.For<IProjectRepository>();
        //    projectRepository.GetProjectsList(userName).Returns(projectDTO);
        //    var projectService = new ProjectService(projectRepository);

        //    //Assert
        //    //projectService.GetProjectsList().Should().NotBeNullOrEmpty();
        //}

        //[Fact]
        //public void GetProjectsList_Should_BeNull_When_ProjectsNotFound()
        //{
        //    //Arrange
        //    List<ProjectListDTO> projectDTO = null;

        //    //Act
        //    var projectRepository = Substitute.For<IProjectRepository>();
        //    projectRepository.GetProjectsList(userName).Returns(projectDTO);
        //    var projectService = new ProjectService(projectRepository);

        //    //Assert
        //    projectService.GetProjectsList(userName).Should().BeNull();
        //}

        [Fact]
        public void GetByProjectId_Should_ReturnCorrectProject_When_MatchingIDPassed()
        {
            //Arrange
            List<ProjectHeaderDTO> listprojectHeaderDTO = GetProjectHeaderDTO();
            int ProjectId = 1;
            int PipSheetId = 1;
            ProjectHeaderDTO projectHeaderDTO = new ProjectHeaderDTO();
            projectHeaderDTO = listprojectHeaderDTO.Find(a => a.ProjectId == ProjectId);

            //Act
            var projectHeaderRepository = Substitute.For<IProjectHeaderRepository>();
            //projectHeaderRepository.GetProjectHeaderData(ProjectId, PipSheetId).Returns(projectHeaderDTO);
            var projectHeaderService = new ProjectHeaderService(projectHeaderRepository);

            //Assert
            projectHeaderService.GetProjectHeaderData(ProjectId, PipSheetId).Should().NotBeNull();
            projectHeaderService.GetProjectHeaderData(ProjectId, PipSheetId).Should().BeSameAs(projectHeaderDTO);
        }

        [Fact]
        public void GetByProjectId_Should_ReturnNotFound_When_WrongIDPassed()
        {
            //Arrange
            List<ProjectHeaderDTO> listprojectHeaderDTO = new List<ProjectHeaderDTO>();
            listprojectHeaderDTO = GetProjectHeaderDTO();
            int ProjectId = 100;              //Id not present in the dummy projects list
            int PipSheetId = 1;
            ProjectHeaderDTO projectHeaderDTO = new ProjectHeaderDTO();
            projectHeaderDTO = listprojectHeaderDTO.Find(a => a.ProjectId == ProjectId);

            //Act
            var projectHeaderRepository = Substitute.For<IProjectHeaderRepository>();
            //projectHeaderRepository.GetProjectHeaderData(ProjectId, PipSheetId).Returns(projectHeaderDTO);
            var projectHeaderService = new ProjectHeaderService(projectHeaderRepository);

            //Assert
            projectHeaderService.GetProjectHeaderData(ProjectId, PipSheetId).Should().BeNull();
        }

        [Fact]
        public void GetByProjectId_Should_ReturnNotFound_When_NoIDPassed()
        {
            //Arrange
            List<ProjectHeaderDTO> listprojectHeaderDTO = new List<ProjectHeaderDTO>();
            listprojectHeaderDTO = GetProjectHeaderDTO();
            int ProjectId = 0;
            int PipSheetId = 0;
            ProjectHeaderDTO projectHeaderDTO = new ProjectHeaderDTO();
            projectHeaderDTO = listprojectHeaderDTO.Find(a => a.ProjectId == ProjectId);

            //Act
            var projectHeaderRepository = Substitute.For<IProjectHeaderRepository>();
            //projectHeaderRepository.GetProjectHeaderData(ProjectId, PipSheetId).Returns(projectHeaderDTO);
            var projectHeaderService = new ProjectHeaderService(projectHeaderRepository);

            //Assert
            projectHeaderService.GetProjectHeaderData(ProjectId, PipSheetId).Should().BeNull();
        }


        public List<ProjectListDTO> GetProjectsDTO()
        {
            List<ProjectListDTO> projectDTO = new List<ProjectListDTO>();

            projectDTO.Add(new ProjectListDTO
            {
                SFProjectId = "USTG-1234-56-75",
                AccountName = "UST Global Group",
                BillingType = "T and E with Cap",
                ProjectName = "PIP Sheets",
                DeliveryType = "Staff Augmentation",
                ServiceLine = "Edge Services: Agile Framework",
            });

            projectDTO.Add(new ProjectListDTO
            {
                SFProjectId = "ATHE-9875-12-56",
                AccountName = "Athena",
                BillingType = "Flat Fee Monthly",
                ProjectName = "Salesforce",
                DeliveryType = "Managed Services / SLA Based",
                ServiceLine = "Business: BI/DW Solutions",
            }
            );

            return projectDTO;
        }

        public List<ProjectHeaderDTO> GetProjectHeaderDTO()
        {
            List<ProjectHeaderDTO> projectHeaderDTO = new List<ProjectHeaderDTO>();

            projectHeaderDTO.Add(new ProjectHeaderDTO
            {
                ProjectId = 1,
                SfProjectId = "USTG-1234-56-75",
                AccountId = 1,
                ProjectBillingTypeId = 1,
                DeliveryOwner = "Test",
                ProjectName = "PIP Sheets",
                ContractingEntityId = 1,
                ProjectDeliveryTypeId = 1,
                ServiceLineId = 27,
                ServicePortfolioId = 4,
                SubmittedBy = "1",
                SubmittedDate = DateTime.UtcNow
            });
            return projectHeaderDTO;
        }
    }
}
