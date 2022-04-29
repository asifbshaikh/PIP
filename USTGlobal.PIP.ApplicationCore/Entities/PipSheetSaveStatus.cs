using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class PipSheetSaveStatus
    {
        public int PipSheetId { get; set; }
        public bool ResourcePlanning { get; set; }
        public bool EbitdaAndOverhead { get; set; }
        public bool LaborPricing { get; set; }
        public bool VacationAbsences { get; set; }
        public bool COLA { get; set; }
        public bool ExpensesAndAssets { get; set; }
        public bool PartnerCostAndRevenue { get; set; }
        public bool ReimbursementAndSales { get; set; }
        public bool OtherPriceAdjustment { get; set; }
        public bool RiskManagement { get; set; }
        public bool ClientPrice { get; set; }
        public bool CapitalCharge { get; set; }
        public bool FixBidAndMargin { get; set; }
    }
}
