using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IRiskManagementRepository
    {
        Task<RiskManagementParentCalcDTO> GetRiskManagement(int pipSheetId);
        Task SaveRiskManagement(string userName, CalculatedValueDTO calculatedValueDTO, RiskManagementDTO riskManagementDTO, IList<RiskManagementPeriodDetailDTO> riskManagementPeriodDetailDTO);
    }
}
