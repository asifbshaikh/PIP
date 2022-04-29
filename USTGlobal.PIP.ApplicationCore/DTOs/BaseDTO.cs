using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class BaseDTO
    {
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
