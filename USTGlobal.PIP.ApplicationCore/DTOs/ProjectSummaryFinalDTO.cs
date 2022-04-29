using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectSummaryFinalDTO
    {
        public ProjectSummaryDTO ProjectSummaryDTO { get; set; }
        public CurrencyConversionDTO CurrencyConversion { get; set; }
    }
}
