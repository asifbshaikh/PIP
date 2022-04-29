using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class BillingScheduleDetailDTO
    {
        public int PipSheetId { get; set; }
        public decimal BlendedLaborCostPerHr { get; set; }
        public decimal BlendedBillRate { get; set; }
        public IList<ProjectPeriodDTO> projectPeriodDTO { get; set; }
        public List<CashFlowParentDTO> cashFlowParentDTO { get; set; }
    }
}
