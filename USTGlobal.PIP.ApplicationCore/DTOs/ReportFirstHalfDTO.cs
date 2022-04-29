using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportFirstHalfDTO
    {
        public int PipSheetId { get; set; }
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
    }
}
