using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PLForecastDTO
    {
        public int DescriptionId { get; set; }
        public int RowSectionId { get; set; }
        public decimal? Total { get; set; }
        public IList<PLForecastPeriodDTO> PLForecastPeriodDTO { get; set; }
    }
}
