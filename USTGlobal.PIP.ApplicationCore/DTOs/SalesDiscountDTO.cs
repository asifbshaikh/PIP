using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SalesDiscountDTO : BaseDTO
    {
        public int UId { get; set; }
        public int SalesDiscountId { get; set; }
        public int PipSheetId { get; set; }
        public int? MilestoneId { get; set; }
        public string Description { get; set; }
        public decimal? Discount { get; set; }
        public bool isDeleted { get; set; }

    }
}
