using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class ExpenseAndAssetRepository : IExpenseAndAssetRepository
    {

        private readonly PipContext pipContext;
        public ExpenseAndAssetRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<ExpenseAndAssetParentDTO> GetExpenseAndAsset(int pipSheetId)
        {
            ExpenseAndAssetParentDTO expenseAndAssetParentDTO = new ExpenseAndAssetParentDTO();
            List<DirectExpensePeriodDTO> directExpensePeriodDTO = new List<DirectExpensePeriodDTO>();
            expenseAndAssetParentDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO = new ExpenseAndAssetSeatCostAndStdOverheadCalcDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetAssetAndDirectExpenses")
                            .WithSqlParam("@PIPSheetId", pipSheetId)
                            .ExecuteStoredProcAsync((expenseAndAssetResultSet) =>
                            {
                                expenseAndAssetParentDTO.AssetDTO = expenseAndAssetResultSet.ReadToList<AssetDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetParentDTO.DirectExpenseDTO = expenseAndAssetResultSet.ReadToList<DirectExpenseParentDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                directExpensePeriodDTO = expenseAndAssetResultSet.ReadToList<DirectExpensePeriodDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetParentDTO.ProjectPeriodDTO = expenseAndAssetResultSet.ReadToList<ProjectPeriodDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetParentDTO.ProjectMilestoneDTO = expenseAndAssetResultSet.ReadToList<ProjectMilestoneDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetParentDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.ResourcePlanningPipSheetDTO = expenseAndAssetResultSet.ReadToList<ResourcePlanningPipSheetDTO>();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetParentDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.LaborPricingEbidtaCalculationDTO = expenseAndAssetResultSet.ReadToList<LaborPricingEbidtaCalculationDTO>();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetParentDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.HolidayList = expenseAndAssetResultSet.ReadToList<HolidayDTO>();
                                expenseAndAssetResultSet.NextResult();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetParentDTO.PeriodLaborRevenueDTO = expenseAndAssetResultSet.ReadToList<PeriodLaborRevenueDTO>().ToList();
                            });
            foreach (DirectExpenseParentDTO directExpenseDTO in expenseAndAssetParentDTO.DirectExpenseDTO)
            {
                List<DirectExpensePeriodDTO> directExpensePeriodDTOs = new List<DirectExpensePeriodDTO>();
                directExpensePeriodDTOs.AddRange(directExpensePeriodDTO.Where(directExpensePeriod => directExpensePeriod.DirectExpenseId == directExpenseDTO.DirectExpenseId).ToList());
                directExpenseDTO.DirectExpensePeriodDTO = directExpensePeriodDTOs;
            }
            return expenseAndAssetParentDTO;
        }

        public async Task<ExpenseAndAssetSaveDependencyDTO> GetExpenseAndAssetForSaveDependency(int pipSheetId)
        {
            ExpenseAndAssetSaveDependencyDTO expenseAndAssetSaveDependencyDTO = new ExpenseAndAssetSaveDependencyDTO();
            List<DirectExpensePeriodDTO> directExpensePeriodDTO = new List<DirectExpensePeriodDTO>();
            expenseAndAssetSaveDependencyDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO = new ExpenseAndAssetSeatCostAndStdOverheadCalcDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetAssetAndDirectExpenses")
                            .WithSqlParam("@PIPSheetId", pipSheetId)
                            .ExecuteStoredProcAsync((expenseAndAssetResultSet) =>
                            {
                                expenseAndAssetSaveDependencyDTO.AssetDTO = expenseAndAssetResultSet.ReadToList<AssetDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetSaveDependencyDTO.DirectExpenseDTO = expenseAndAssetResultSet.ReadToList<DirectExpenseParentDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                directExpensePeriodDTO = expenseAndAssetResultSet.ReadToList<DirectExpensePeriodDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetSaveDependencyDTO.ProjectPeriodDTO = expenseAndAssetResultSet.ReadToList<ProjectPeriodDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetSaveDependencyDTO.ProjectMilestoneDTO = expenseAndAssetResultSet.ReadToList<ProjectMilestoneDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetSaveDependencyDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.ResourcePlanningPipSheetDTO = expenseAndAssetResultSet.ReadToList<ResourcePlanningPipSheetDTO>();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetSaveDependencyDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.LaborPricingEbidtaCalculationDTO = expenseAndAssetResultSet.ReadToList<LaborPricingEbidtaCalculationDTO>();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetSaveDependencyDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.HolidayList = expenseAndAssetResultSet.ReadToList<HolidayDTO>();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetSaveDependencyDTO.BasicAssetDTO = expenseAndAssetResultSet.ReadToList<BasicAssetDTO>().ToList();
                                expenseAndAssetResultSet.NextResult();

                                expenseAndAssetSaveDependencyDTO.PeriodLaborRevenueDTO = expenseAndAssetResultSet.ReadToList<PeriodLaborRevenueDTO>().ToList();
                            });
            foreach (DirectExpenseParentDTO directExpenseDTO in expenseAndAssetSaveDependencyDTO.DirectExpenseDTO)
            {
                List<DirectExpensePeriodDTO> directExpensePeriodDTOs = new List<DirectExpensePeriodDTO>();
                directExpensePeriodDTOs.AddRange(directExpensePeriodDTO.Where(directExpensePeriod => directExpensePeriod.DirectExpenseId == directExpenseDTO.DirectExpenseId).ToList());
                directExpenseDTO.DirectExpensePeriodDTO = directExpensePeriodDTOs;
            }
            return expenseAndAssetSaveDependencyDTO;
        }


        public async Task SaveExpenseAndAssetData(string userName, IList<DirectExpensePeriodDTO> directExpensePeriods, IList<DirectExpenseDTO> directExpense, List<AssetDTO> assetDTO)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveAssetAndDirectExpenses {0}, {1}, {2}, {3} ",
                userName,
                new SqlParameter("@InputAssets", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(assetDTO),
                    TypeName = "dbo.ProjectAsset"
                },
                new SqlParameter("@InputDirectExpenses", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(directExpense),
                    TypeName = "dbo.DirectExpense"
                },
                new SqlParameter("@InputDirectExpensePeriodDetails", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(directExpensePeriods),
                    TypeName = "dbo.DirectExpensePeriodDetails"
                });
            await pipContext.SaveChangesAsync();
        }
    }
}
