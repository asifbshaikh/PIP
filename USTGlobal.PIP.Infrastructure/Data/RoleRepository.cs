using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly PipContext pipContext;

        public RoleRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<RoleDTO> GetRole(int roleId)
        {
            return await this.pipContext.Role
                .Where(role => role.RoleId == roleId)
                .Select(role => new RoleDTO
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName
                }).SingleOrDefaultAsync();
        }
    }
}
