using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IExpenseAndAssetService
    {
        Task<ExpenseAndAssetDTO> GetExpenseAndAsset(int pipSheetId);
        Task SaveExpenseAndAssetData(string userName, ExpenseAndAssetDTO expenseAndAssetDto);
        Task<ExpenseAndAssetDTO> CreateExpenseAndAssetObject(ExpenseAndAssetSaveDependencyDTO expenseAndAssetSaveDependencyDTO);
        Task<ExpenseAndAssetSaveDependencyDTO> GetExpenseAndAssetForSaveDependency(int pipSheetId);
    }
}
