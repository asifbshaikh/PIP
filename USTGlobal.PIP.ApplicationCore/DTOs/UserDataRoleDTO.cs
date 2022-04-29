using System.Collections.Generic;
namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class UserDataRoleDTO
    {
        public IList<UserDTO> UserDTO { get; set; }
        public IList<RoleDTO> RoleDTO { get; set; }
    }
}
