using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IUserRepository
    {
        Task<UserRoleDTO> GetUserData(int userId);
        Task<UserDataRoleDTO> GetUserData(string emailId);
        Task<bool> VerifyUserData(string email);
        Task<SharedDataDTO> GetSharedData(string emailId, int pipSheetId);
        Task<string> SaveUserData(UserDTO userDTO, string userName);
        Task<List<UserListResultDTO>> UploadMultipleUserData(string userName, IList<UserListDTO> userListDTO);
    }
}
