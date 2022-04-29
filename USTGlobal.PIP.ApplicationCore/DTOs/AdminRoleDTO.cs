using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class AdminRoleDTO
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int AccountId { get; set; }
        public bool fromAdminScreen { get; set; }
    }
}
