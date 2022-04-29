using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository areaRepository;

        public AreaService(IAreaRepository areaRepo)
        {
            this.areaRepository = areaRepo;
        }

        public async Task<AreaDTO> GetArea(int areaId)
        {
            return await this.areaRepository.GetArea(areaId);
        }
    }
}
