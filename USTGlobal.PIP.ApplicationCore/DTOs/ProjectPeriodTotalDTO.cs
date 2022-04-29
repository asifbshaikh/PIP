using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectPeriodTotalDTO
    {
        public int ProjectPeriodId { get; set; }
        public int PipSheetId { get; set; }
        public int BillingPeriodId { get; set; }
        public decimal FTE { get; set; }
        public decimal Revenue { get; set; }
        public decimal LostRevenue { get; set; }
        public decimal OtherPriceAdjustment { get; set; }
        public decimal ClientPrice { get; set; }
        public decimal FeesAtRisk { get; set; }
        public decimal CapitalCharge { get; set; }
        public decimal NetEstimatedRevenue { get; set; }
        public decimal PartnerRevenue { get; set; }
        public decimal Reimbursement { get; set; }
        public decimal SalesDiscount { get; set; }
        public decimal AssetSubTotalExpense { get; set; }
        public decimal CappedCost { get; set; }
        public decimal PartnerCost { get; set; }
        public decimal EbitdaSeatCost { get; set; }
        public decimal Inflation { get; set; }
    }
}
