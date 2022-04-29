using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectSummaryMainDTO : ProjectSummaryDTO
    {
        public CurrencyConversionDTO CurrencyConversion { get; set; }
    }
}
