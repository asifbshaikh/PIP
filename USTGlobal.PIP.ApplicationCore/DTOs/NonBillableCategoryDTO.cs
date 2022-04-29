using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class NonBillableCategoryDTO
    {
        public int SerialId { get; set; }
        public int? NonBillableCategoryId { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Comments { get; set; }
        public int Status { get; set; }
    }
}
