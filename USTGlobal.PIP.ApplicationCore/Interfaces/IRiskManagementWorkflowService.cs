
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IRiskManagementWorkflowService
    {
        Task ProcessRiskManagementSaving(string userName, RiskManagementCalcDTO riskManagementCalcDTO);
    }
}
