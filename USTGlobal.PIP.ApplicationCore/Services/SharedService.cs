using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class SharedService : ISharedService
    {
        private readonly ISharedRepository sharedRepository;

        public SharedService(ISharedRepository sharedRepository)
        {
            this.sharedRepository = sharedRepository;
        }

        public async Task<OverrideNotificationDTO> GetOverrideNotificationStatus(int pipSheetId)
        {
            return await this.sharedRepository.GetOverrideNotificationStatus(pipSheetId);
        }
    }
}
