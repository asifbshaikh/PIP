using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectSummaryDTO
    {        
        public decimal TotalLaborRevenue { get; set; }
        public decimal? VacationAbsence { get; set; }
        public decimal TotalPartnerRevenue { get; set; }
        public decimal TotalPartnerCost { get; set; }
        public decimal NetPartnerRevenue { get; set; }
        public decimal TotalExpenseReimbursement { get; set; }
        public decimal TotalQualifyingDiscounts { get; set; }
        public decimal? RiskOrMarginCalculation { get; set; }
        public decimal TotalOtherRevenue { get; set; }
    }
}
