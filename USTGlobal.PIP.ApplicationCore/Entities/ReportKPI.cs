using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ReportKPI
    {
        public int ReportKPIId { get; set; }
        public string KPIName { get; set; }
        public bool? SummaryViewReport { get; set; }
        public bool? DetailedLevelReport { get; set; }
        public bool? ResourceLevelReport { get; set; }
        public int? PLForecastLabelId { get; set; }
    }
}
