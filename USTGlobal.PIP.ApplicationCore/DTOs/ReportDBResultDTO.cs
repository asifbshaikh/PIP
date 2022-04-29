using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportDBResultDTO
    {
        public IList<ReportFirstHalfDTO> ReportFirstHalf { get; set; }
        public IList<ReportPLForecastDTO> PLForecast { get; set; }
        public IList<ReportPLForecastPeriodDTO> PLForecastPeriods { get; set; }
        public IList<ReportPeriodDTO> ReportPeriods { get; set; }
        public IList<ReportKPIDTO> ReportKPIs { get; set; }
        public IList<ClientPriceDTO> ReportInvoicingSchedule { get; set; }
        public IList<ClientPricePeriodDTO> ReportInvoicingSchedulePeriodDetails { get; set; }
        public IList<ReportResourceDTO> ReportResources { get; set; }
        public IList<ReportResourcePeriodDetailDTO> ReportResourcePeriodDetails { get; set; }
        public ReportEmail ReportEmail { get; set; }
    }
}
