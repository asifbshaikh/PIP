using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LaborPricingEbidtaCalculationDTO
    {
        public int ProjectResourcePeriodDetailsId { get; set; }
        public int ProjectResourceId { get; set; }
        public int LocationId { get; set; }
        public int ResourceId { get; set; }
        public decimal FTEValue { get; set; }
        public decimal SharedSeatsUsePercent { get; set; }
        public decimal OverheadAmount { get; set; }
        public decimal EbitdaSeatCost { get; set; }
        public int Which { get; set; }
    }
}
