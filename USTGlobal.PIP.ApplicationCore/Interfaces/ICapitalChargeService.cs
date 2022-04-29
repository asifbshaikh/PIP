using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;


namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ICapitalChargeService
    {
        Task<CapitalChargeResultSetDTO> GetCapitalCharge(int pipSheetId);
        Task SaveCapitalCharge(string userName, CapitalChargeResultSetDTO capitalCharge);
        Task<CapitalChargeResultSetDTO> CalculateCapitalCharges(int pipSheetId);
    }
}
