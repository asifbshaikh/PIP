using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ExpenseAndAssetSaveDependencyDTO
    {
        public List<AssetDTO> AssetDTO { get; set; }
        public List<DirectExpenseParentDTO> DirectExpenseDTO { get; set; }
        public List<ProjectPeriodDTO> ProjectPeriodDTO { get; set; }
        public List<ProjectMilestoneDTO> ProjectMilestoneDTO { get; set; }
        public ExpenseAndAssetSeatCostAndStdOverheadCalcDTO ExpenseAndAssetSeatCostAndStdOverheadCalcDTO { get; set; }
        public List<BasicAssetDTO> BasicAssetDTO { get; set; }
        public List<PeriodLaborRevenueDTO> PeriodLaborRevenueDTO { get; set; }
    }
}
