using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.Api.Middleware;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// AdminMasterController
    /// </summary>
    [AuthorizeRolePermission(AuthorizeRoles = "Admin")]
    [Route("api/adminMaster")]
    public class AdminMasterController : BaseController
    {
        private readonly IAdminMasterService adminMasterService;

        /// <summary>
        /// AdminMasterController constructor
        /// </summary>
        /// <param name="adminMasterService"></param>
        public AdminMasterController(IAdminMasterService adminMasterService)
        {
            this.adminMasterService = adminMasterService;
        }

        /// <summary>
        /// Get Locations
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("locations")]
        public async Task<List<LocationDTO>> GetLocations()
        {
            return await this.adminMasterService.GetLocations();
        }

        /// <summary>
        /// Get Past Location Versions based on LocationId
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [HttpGet, Route("location/{locationId}/pastVersions")]
        public async Task<List<LocationDTO>> GetPastLocationVersions(int locationId)
        {
            return await this.adminMasterService.GetPastLocationVersions(locationId);
        }

        /// <summary>
        /// Get Inactive Location Versions
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [HttpGet, Route("location/{locationId}/inactiveVersion")]
        public async Task<LocationDTO> GetInactiveLocationVersion(int locationId)
        {
            return await this.adminMasterService.GetInactiveLocationVersion(locationId);
        }

        /// <summary>
        /// Save Location
        /// </summary>
        /// <param name="locationDTO"></param>
        /// <returns></returns>
        [HttpPost, Route("location")]
        public async Task<int> SaveLocation([FromBody] LocationDTO locationDTO)
        {
            return await this.adminMasterService.SaveLocation(locationDTO);
        }

        /// <summary>
        /// Delete Location Version based on LocationId
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        [HttpDelete, Route("location/{locationId}/version")]
        public async Task DiscardLocationVersion(int locationId)
        {
            await this.adminMasterService.DiscardLocationVersion(locationId);
        }
    }
}
