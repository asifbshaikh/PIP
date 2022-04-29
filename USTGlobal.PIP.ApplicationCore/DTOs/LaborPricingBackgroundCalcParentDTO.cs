using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LaborPricingBackgroundCalcParentDTO
    {
        public LaborPricingBackgroundCalculationDTO LaborPricingBackgroundCalculationDTO { get; set; }
        public IList<LaborPricingEbidtaCalculationDTO> LaborPricingEbidtaCalculationDTO { get; set; }
        public IList<ResourcePlanningPipSheetDTO> ResourcePlanningPipSheetDTO { get; set; }
        public IList<HolidayDTO> HolidayList { get; set; }
        public MarginDTO MarginDTO { get; set; }
        public decimal TotalAssesedRiskOverrun { get; set; }
        public decimal PaymentLag { get; set; }
        public decimal FeesAtRisk { get; set; }
        public ProjectPeriodTotalDTO ProjectPeriodTotalDTO { get; set; }
        public RiskManagementDTO RiskManagementDTO { get; set; }
        public decimal TotalPartnerCost { get; set; }
        public decimal TotalDirectExpense { get; set; }
        public IList<LocationEbitdaSeatCostDTO> LocationEbitdaSeatCost { get; set; }
    }
}
