using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class TotalDealFinancialsDBResultDTO
    {
        public CalculatedValueDTO CalculatedValue { get; set; }
        public IList<DealFinancialsYearTotalsDTO> DealFinancialsYearTotalDTO { get; set; }
        public IList<BeatTaxImpactDTO> BeatTaxYearDTO { get; set; }
        public decimal LocalToUSDCurrencyFactor { get; set; }
        public decimal TotalBeatTaxImpactPercent { get; set; }
    }
}
