using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SalesDiscountParentDTO : SalesDiscountDTO
    {
        public IList<SalesDiscountPeriodDTO> SalesDiscountPeriods { get; set; }

    }
}
