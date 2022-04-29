using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class AreaRepository : IAreaRepository
    {
        private readonly PipContext pipContext;

        public AreaRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<AreaDTO> GetArea(int areaId)
        {
            return await this.pipContext.Area
                    .Where(area => area.AreaId == areaId)
                    .Select(area => new AreaDTO
                    {
                        AreaId = area.AreaId,
                        AreaName = area.AreaName
                    }).SingleOrDefaultAsync();
        }
    }
}
