using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class MasterVersionRepository : IMasterVersionRepository
    {
        private readonly PipContext pipContext;

        public MasterVersionRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<MasterVersionDTO> GetMasterVersionData(int masterVersionId)
        {
            return await this.pipContext.MasterVersion
                    .Where(masterVersion => masterVersion.MasterVersionId == masterVersionId)
                    .Select(masterVersion => new MasterVersionDTO
                    {
                        MasterVersionId = masterVersion.MasterVersionId,
                        VersionName = masterVersion.VersionName,
                        ValidFrom = masterVersion.ValidFrom,
                        ValidTo = masterVersion.ValidTo
                    }).SingleOrDefaultAsync();
        }
    }
}
