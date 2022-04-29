using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SharePipVersionDTO
    {
        public ProjectDTO projectDTO { get; set; }
        public IList<PipSheetDTO> PipSheetDTO { get; set; }
        public IList<UserDTO> UserDTO { get; set; }
    }
}
