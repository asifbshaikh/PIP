using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SharedPipRoleDTO
    {
        public int UserId { get; set; }
        public string UID { get; set; }
        public int PipSheetId { get; set; }
        public bool IsEditor { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
