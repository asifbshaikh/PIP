using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportKPIDTO
    {
        public int KPIId { get; set; }
        public string KPIName { get; set; }
        public int? PLForecastLabelId { get; set; }
    }
}
