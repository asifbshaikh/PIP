using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IRateRepository
    {
        Task<RateDTO> GetRate(int rateId);
    }
}
