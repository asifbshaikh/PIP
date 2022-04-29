using System;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly PipContext pipContext;

        public ProjectRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<ProjectListMainDTO> GetProjectsList(string userName)
        {
            ProjectListMainDTO projectListMainDTO = new ProjectListMainDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetProjectList")
               .WithSqlParam("@UserName", userName)
               .ExecuteStoredProcAsync((projectListresult) =>
               {
                   projectListMainDTO.ProjectListDTO = projectListresult.ReadToList<ProjectListDTO>().ToList();
                   projectListresult.NextResultAsync();

                   projectListMainDTO.IsEditor = Convert.ToBoolean(projectListresult.ReadToValue<Boolean>());
               });
            return projectListMainDTO;
        }
    }
}
