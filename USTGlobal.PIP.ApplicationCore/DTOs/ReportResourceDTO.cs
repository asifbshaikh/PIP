using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReportResourceDTO
    {
        public int ProjectResourceId { get; set; }
        public int PipSheetId { get; set; }
        public decimal CappedCostPerResource { get; set; }
        public decimal RevenuePerResource { get; set; }   
        public decimal BillRatePerResource { get; set; }
        public decimal TotalHoursPerResource { get; set; }
        public decimal CostHrsPerResource { get; set; }
        public decimal CostRatePerResource { get; set; }
        public decimal FTEPerResource { get; set; }
        public string Billablity { get; set; }
        public string Location { get; set; }
        public string Role { get; set; }
        public string RoleGroup { get; set; }
        public string Band { get; set; }
        public string PhaseMilestone { get; set; }
        public int PeriodsCount { get; set; }
    }
}
