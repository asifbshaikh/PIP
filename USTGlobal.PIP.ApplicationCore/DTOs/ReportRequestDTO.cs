using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<AccountDTO> SelectedAccounts { get; set; }
        public IList<AccountBasedProjectDTO> SelectedProjects { get; set; }
        public IList<ReportKPIDTO> SelectedKPIs { get; set; }
        public int ReportType { get; set; }
        public bool IsUSDCurrency { get; set; }
    }
}
