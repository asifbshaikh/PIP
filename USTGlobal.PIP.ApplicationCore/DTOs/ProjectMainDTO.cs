using System.Collections.Generic;
namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectMainDTO
    {
        public List<ProjectDTO> ProjectDTO { get; set; }
        public bool IsEditor { get; set; }
    }
}
