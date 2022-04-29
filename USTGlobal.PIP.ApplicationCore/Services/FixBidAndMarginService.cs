using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class FixBidAndMarginService : IFixBidAndMarginService
    {
        private readonly IFixBidAndMarginRepository fixBidAndMarginRepository;
        public FixBidAndMarginService(IFixBidAndMarginRepository fixBidAndMarginRepository)
        {
            this.fixBidAndMarginRepository = fixBidAndMarginRepository;
        }

        public async Task<FixBidAndMarginDTO> CalculateAndSaveFixBidData(int pipSheetId, string userName)
        {
            FixBidAndMarginDTO fixBidAndMarginDTO =  await fixBidAndMarginRepository.CalculateAndSaveFixBidData(pipSheetId, userName);
            return fixBidAndMarginDTO;
        }
    }
}
