using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class KeyPerformanceIndicatorDTO
    {
        public decimal OnShoreFTEPercent { get; set; }
        public decimal GrossMarginPercent { get; set; }
        public decimal EbitdaPercent { get; set; }
        public decimal ServiceLineBlendedTargetEbitda { get; set; }
        public decimal VariancePercent { get; set; }
        public decimal CostContingencyPercent { get; set; }
        public string FirstMonthPositiveCashFlow { get; set; }
        public IList<ServiceLineRevenueDTO> ServiceLineRevenueList { get; set; }
        public IList<ServiceLineEbitdaDTO> ServiceLineEbitdaPercentList { get; set; }
        
    }
}
