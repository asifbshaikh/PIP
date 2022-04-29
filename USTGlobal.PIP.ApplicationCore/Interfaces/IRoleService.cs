using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IRoleService
    {
        Task<RoleDTO> GetRole(int roleId);
    }
}
