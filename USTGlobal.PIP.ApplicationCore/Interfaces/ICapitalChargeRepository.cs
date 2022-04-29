using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ICapitalChargeRepository
    {
        Task<CapitalChargeResultSetDTO> GetCapitalCharge(int pipSheetId);
        Task SaveCapitalCharge(string userName, CapitalChargeResultSetDTO capitalCharge, decimal? totalProjectCost);

    }
}
