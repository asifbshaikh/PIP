using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class BeatTaxImpactDTO
    {
        public int Year { get; set; }
        public decimal BeatTaxImpactPercent { get; set; }
        public decimal BeatTaxAmount { get; set; }
    }
}
