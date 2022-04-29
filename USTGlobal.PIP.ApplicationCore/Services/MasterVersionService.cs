using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class MasterVersionService : IMasterVersionService
    {
        private readonly IMasterVersionRepository masterVersionRepository;

        public MasterVersionService(IMasterVersionRepository masterVersionRepo)
        {
            this.masterVersionRepository = masterVersionRepo;
        }

        public async Task<MasterVersionDTO> GetMasterVersionData(int masterVersionId)
        {
            return await this.masterVersionRepository.GetMasterVersionData(masterVersionId);
        }
    }
}
