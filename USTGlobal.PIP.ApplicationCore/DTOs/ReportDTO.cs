using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportDTO
    {
        public string SFProjectId { get; set; }
        public string AccountName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public string DeliveryOwner { get; set; }
        public string Portfolio { get; set; }
        public string ServiceLine { get; set; }
        public string DeliveryType { get; set; }
        public string BillingType { get; set; }
        public string Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string KPI { get; set; }
        public int KPIId { get; set; }
        public string Location { get; set; }
        public string Billablity { get; set; }
        public string Role { get; set; }
        public string RoleGroup { get; set; }
        public string Band { get; set; }
        public string PhaseMilestone { get; set; }
        public decimal ProjectTotal { get; set; }
        public decimal ReportTotal { get; set; }
        public IList<PeriodReportDTO> PeriodFields { get; set; }
    }
}
