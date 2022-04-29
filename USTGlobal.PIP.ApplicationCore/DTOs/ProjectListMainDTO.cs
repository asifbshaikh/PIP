using System.Collections.Generic;
namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectListMainDTO
    {
        public List<ProjectListDTO> ProjectListDTO { get; set; }
        public bool IsEditor { get; set; }
    }
}
