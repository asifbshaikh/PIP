using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class RateService : IRateService
    {
        private readonly IRateRepository rateRepository;

        public RateService(IRateRepository rateRepo)
        {
            this.rateRepository = rateRepo;
        }

        public async Task<RateDTO> GetRate(int rateId)
        {
            return await this.rateRepository.GetRate(rateId);
        }
    }
}
