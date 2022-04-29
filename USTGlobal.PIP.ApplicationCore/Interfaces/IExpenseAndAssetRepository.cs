using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IExpenseAndAssetRepository
    {
        Task<ExpenseAndAssetParentDTO> GetExpenseAndAsset(int pipSheetId);
        Task SaveExpenseAndAssetData(string userName, IList<DirectExpensePeriodDTO> directExpensePeriods, IList<DirectExpenseDTO> directExpense, List<AssetDTO> assetDTO);
        Task<ExpenseAndAssetSaveDependencyDTO> GetExpenseAndAssetForSaveDependency(int pipSheetId);
    }
}
