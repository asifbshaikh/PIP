using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;
        public RoleService(IRoleRepository roleRepo)
        {
            this.roleRepository = roleRepo;
        }

        public async Task<RoleDTO> GetRole(int roleId)
        {
            return await this.roleRepository.GetRole(roleId);
        }
    }
}
