using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class AdminMasterService : IAdminMasterService
    {
        private readonly IAdminMasterRepository adminMasterRepository;
        public AdminMasterService(IAdminMasterRepository adminMasterRepository)
        {
            this.adminMasterRepository = adminMasterRepository;
        }

        public async Task<List<LocationDTO>> GetLocations()
        {
            return await this.adminMasterRepository.GetLocations();
        }
        public async Task<List<LocationDTO>> GetPastLocationVersions(int locationId)
        {
            return await this.adminMasterRepository.GetPastLocationVersions(locationId);
        }
        public async Task<int> SaveLocation(LocationDTO locationDTO)
        {
            return await this.adminMasterRepository.SaveLocation(locationDTO);
        }
        public async Task DiscardLocationVersion(int locationId)
        {
            await this.adminMasterRepository.DiscardLocationVersion(locationId);
        }

        public async Task<LocationDTO> GetInactiveLocationVersion(int locationId)
        {
            return await this.adminMasterRepository.GetInactiveLocationVersion(locationId);
        }
    }
}
