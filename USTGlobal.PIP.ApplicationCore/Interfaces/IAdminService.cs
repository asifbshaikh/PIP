using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminDTO>> GetAdmins(int accountId);
        Task<List<UserDTO>> GetUsers();
        Task DeleteUserRole(int userId, int accountId, string userName);
        Task SaveAdminRole(AdminRoleDTO adminRoleDTO, string userName);
        Task SaveUserRoles(RoleManagementDTO adminRoleDTO, string userName);
        Task<List<RoleManagementDTO>> getUsersAndRoles(int accountId);
        Task<List<RoleManagementDTO>> getAllUsersAndAssociatedRoles();
        Task SaveSharedPipRole(SharedPipRoleDTO sharedPipRole, string userName);
        Task AssignReadOnlyRoleForAllAccounts(int userId, string userName);
        Task<List<UserRoleReadOnly>> GetReadOnlyUserList();
        Task DeleteReadOnlyRoleForAllAccounts(int userId, string userName);
        bool isFromAdminScreen { get; set; }
    }
}
