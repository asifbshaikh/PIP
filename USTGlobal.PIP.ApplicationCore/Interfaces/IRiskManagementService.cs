using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IRiskManagementService
    {
        Task<RiskManagementCalcDTO> GetRiskManagement(int pipSheetId);
        Task SaveRiskManagement(string userName, RiskManagementCalcDTO riskManagementCalcDTO);
        Task<RiskManagementCalcDTO> CalculateRiskManagementData(int pipSheetId, string userName);

        //Task ProcessSaveDependency(int pipSheetId, string userName);

        //Task ProcessRiskManagementSaving(string userName, RiskManagementCalcDTO riskManagementCalcDTO);


    }
}
