using System;
using System.Collections.Generic;
using System.IO;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class EmailDTO
    {
        public int EmailNotificationId { get; set; }
        public PlaceHolderDTO TemplateData { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Link { get; set; }
        public List<int> Versions { get; set; }
        public List<string> Roles { get; set; }
        public string htmlString { get; set; }

        // Report Fields
        public string ReportName { get; set; }
        public string SelectedAccounts { get; set; }
        public string SelectedProjects { get; set; }
        public string ReportCurrency { get; set; }
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }
        public string ReportFileName { get; set; }
        public string ReportFilePath { get; set; }
        public string SelectedKPIs { get; set; }
        public bool IsResourceLevelReport { get; set; }
    }
}
