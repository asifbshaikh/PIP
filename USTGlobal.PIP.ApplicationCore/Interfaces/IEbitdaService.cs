using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IEbitdaService
    {
        Task<List<EbitdaDTO>> GetEbitdaAndStandardOverhead(int pipSheetId);
        Task UpdateEbitda(string userName, List<EbitdaDTO> ebitdadata);
        List<EbitdaDTO> CreateEbitdaObject(int pipsheetId, List<EbitdaDTO> ebitdaDTO);
    }
}
