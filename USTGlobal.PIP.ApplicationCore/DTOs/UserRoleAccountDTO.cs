using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class UserRoleAccountDTO : UserDTO
    {
        public IList<RoleAndAccountDTO> RoleAndAccountDTO { get; set; }
    }
}
