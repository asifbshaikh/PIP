using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PartnerCostDTO :BaseDTO
    {
        public int UId { get; set; }
        public int PartnerCostId { get; set; }
        public int PipSheetId { get; set; }
        public int? MilestoneId { get; set; }
        public string Description { get; set; }
        public decimal? PaidAmount { get; set; }
        public bool IsDeleted { get; set; }
        public bool SetMargin { get; set; }
        public decimal? MarginPercent { get; set; }
    }
}
