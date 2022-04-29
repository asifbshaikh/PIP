using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectControlDTO
    {
        public IList<PipSheetDTO> PIPSheetListDTO { get; set; }
        public IList<ProjectLocationDTO> ProjectLocationListDTO { get; set; }
        public IList<ProjectMilestoneDTO> ProjectMilestoneListDTO { get; set; }
    }
}
