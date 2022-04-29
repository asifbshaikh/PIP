using System.Collections.Generic;
namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PipSheetVersionMainDTO
    {
        public List<PipSheetVersionDTO> PipSheetVersionDTO { get; set; }
        public int ProjectWorkflowStatus { get; set; }
    }
}
