using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReimbursementParentDTO : ReimbursementDTO
    {
        public IList<ReimbursementPeriodDTO> ReimbursementPeriods { get; set; }
    }
}
