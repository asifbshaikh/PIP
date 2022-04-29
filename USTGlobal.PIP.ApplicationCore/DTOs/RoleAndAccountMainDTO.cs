using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class RoleAndAccountMainDTO
    {
        public List<AccountId> AccountLevelAccessIds { get; set; }
        public List<RoleAndAccountDTO> RoleAndAccountDTO { get; set; }
        public List<RoleAndAccountDTO> SharedAccountRoles { get; set; }
    }
}
