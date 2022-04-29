using USTGlobal.PIP.ApplicationCore.DTOs;
using System.Threading.Tasks;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ICapitalChargeWorkflowService
    {
        Task ProcessCapitalChargeSaving(string userName, CapitalChargeResultSetDTO capitalCharge);
    }
}
