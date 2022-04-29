using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class UserRoleDTO : UserDTO
    {
        public IList<RoleDTO> Role { get; set; }
    }
}
