using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class SharedRepository : ISharedRepository
    {
        private readonly PipContext pipContext;

        public SharedRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<PipSheetSaveStatusDTO> GetPipSheetSaveStatus(int pipSheetId)
        {
            return await this.pipContext.PipSheetSaveStatus
                        .Where(p => p.PipSheetId == pipSheetId)
                        .ProjectToType<PipSheetSaveStatusDTO>()
                        .SingleOrDefaultAsync();
        }

        public async Task<OverrideNotificationDTO> GetOverrideNotificationStatus(int pipSheetId)
        {
            return await this.pipContext.OverrideNotification
                         .Where(o => o.PipSheetId == pipSheetId)
                         .ProjectToType<OverrideNotificationDTO>()
                         .SingleOrDefaultAsync();
        }
    }
}
