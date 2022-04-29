using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class InvestmentViewResultSetDTO
    {
        public InvestmentViewDTO investmentView { get; set; }
        public IList<CorporateTargetDTO> corporateTarget { get; set; }
        public CurrencyConversionDTO CurrencyConversion { get; set; }
    }
}
