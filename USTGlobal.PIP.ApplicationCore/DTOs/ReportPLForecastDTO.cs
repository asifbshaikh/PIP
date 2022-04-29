using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportPLForecastDTO
    {
        public int PLForecastId { get; set; }
        public string Label { get; set; }
        public int PLForecastLabelId { get; set; }
        public int PipSheetId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
