using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class FixBidCalcDTO : BaseDTO
    {
        public int CostMarginId { get; set; }
        public int DescriptionId { get; set; }
        public decimal TotalCost { get; set; }
    }
}
