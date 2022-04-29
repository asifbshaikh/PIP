using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class WorkflowStatusAndAccountSpecificRoleDTO
    {
        public PipSheetWorkflowStatus PipSheetWorkflowStatus { get; set; }
        public List<RoleAndAccountDTO> RoleAndAccountDTO { get; set; }
        public bool HasAccountLevelAccess { get; set; }
        public bool CanNavigate { get; set; }
        public bool isDummy { get; set; }
    }
}
