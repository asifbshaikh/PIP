using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class VacationAbsencesParentDTO : VacationAbsencesDTO
    {
        public IList<PeriodLostRevenueDTO> PeriodLostRevenue { get; set; }

    }
}
