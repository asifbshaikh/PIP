using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class RateRepository : IRateRepository
    {
        private readonly PipContext pipContext;

        public RateRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<RateDTO> GetRate(int rateId)
        {
            return await this.pipContext.Rate
                    .Where(rate => rate.RateId == rateId)
                    .Select(rate => new RateDTO
                    {
                        RateId = rate.RateId,
                        dRate = rate.dRate
                    }).SingleOrDefaultAsync();
        }
    }
}
