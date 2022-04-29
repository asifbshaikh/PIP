using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IExpensesAndAssetsWorkflowService
    {
        Task ProcessExpenseAndAssetSaving(string userName, ExpenseAndAssetDTO expenseAndAssetDto);
    }
}
