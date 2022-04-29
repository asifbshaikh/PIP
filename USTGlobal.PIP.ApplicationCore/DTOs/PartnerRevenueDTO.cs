using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerRevenueDTO : BaseDTO
    {
        public int UId { get; set; }
        public int? PartnerCostUId { get; set; }
        public int PartnerRevenueId { get; set; }
        public int PipSheetId { get; set; }
        public int? MilestoneId { get; set; }
        public string Description { get; set; }
        public decimal? RevenueAmount { get; set; }
        public bool IsDeleted { get; set; }
        public bool SetMargin { get; set; }
        public decimal? MarginPercent { get; set; }
    }
}
