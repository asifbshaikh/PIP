using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PLForecastParentDTO
    {
        public IList<PLForecastDTO> PLForecastDTO { get; set;}
        public IList<ProjectPeriodDTO> ProjectPeriodDTO { get; set; }
    }
}
