using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PipSheetVersionListAndRoleDTO
    {
        public List<PipSheetVersionListDTO> PipSheetVersionListDTO { get; set; }
        public List<RoleNameDTO> RoleNameDTO { get; set; }
        public int ProjectWorkflowStatus { get; set; }
    }
}
