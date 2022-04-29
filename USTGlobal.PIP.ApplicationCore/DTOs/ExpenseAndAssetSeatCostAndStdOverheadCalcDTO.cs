using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ExpenseAndAssetSeatCostAndStdOverheadCalcDTO
    {
        public IList<LaborPricingEbidtaCalculationDTO> LaborPricingEbidtaCalculationDTO { get; set; }
        public IList<ResourcePlanningPipSheetDTO> ResourcePlanningPipSheetDTO { get; set; }
        public IList<HolidayDTO> HolidayList { get; set; }
    }
}
