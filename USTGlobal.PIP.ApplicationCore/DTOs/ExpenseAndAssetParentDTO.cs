using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ExpenseAndAssetParentDTO
    {
        public List<AssetDTO> AssetDTO { get; set; }
        public List<DirectExpenseParentDTO> DirectExpenseDTO { get; set; }
        public List<ProjectPeriodDTO> ProjectPeriodDTO { get; set; }
        public List<ProjectMilestoneDTO> ProjectMilestoneDTO { get; set; }
        public ExpenseAndAssetSeatCostAndStdOverheadCalcDTO ExpenseAndAssetSeatCostAndStdOverheadCalcDTO { get; set; }
        public List<PeriodLaborRevenueDTO> PeriodLaborRevenueDTO { get; set; }
    }
}
