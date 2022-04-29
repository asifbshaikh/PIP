using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IAdminMasterRepository
    {
        Task<List<LocationDTO>> GetLocations();
        Task<List<LocationDTO>> GetPastLocationVersions(int locationId);
        Task<int> SaveLocation(LocationDTO locationDTO);
        Task DiscardLocationVersion(int locationId);
        Task<LocationDTO> GetInactiveLocationVersion(int locationId);
    }
}
