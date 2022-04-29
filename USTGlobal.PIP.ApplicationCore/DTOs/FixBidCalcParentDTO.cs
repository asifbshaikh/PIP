using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class FixBidCalcParentDTO : FixBidCalcDTO
    {
        public List<FixBidCalcPeriodDTO> PeriodDetails { get; set; }

    }
}
