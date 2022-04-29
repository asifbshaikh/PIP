using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ReplicatePIPSheetService : IReplicatePIPSheetService
    {
        private readonly IReplicateRepository replicateRepository;

        public ReplicatePIPSheetService(IReplicateRepository replicateRepository)
        {
            this.replicateRepository = replicateRepository;
        }

        public async Task<RouteParamDTO> ReplicatePIPSheet(string userName, ReplicatePIPSheetDTO replicatePIPSheet)
        {
            return await this.replicateRepository.ReplicatePIPSheet(userName, replicatePIPSheet);
        }
    }
}
