using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IEbitdaRepository
    {
        Task<List<EbitdaDTO>> GetEbitdaAndStandardOverhead(int pipSheetId);
        Task UpdateEbitda(string userName, List<EbitdaDTO> ebitdadata, bool isOverridenValueLessThanRefUSD);
    }
}
