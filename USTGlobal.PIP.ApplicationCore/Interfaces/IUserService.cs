using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IUserService
    {
        Task<UserRoleDTO> GetUserData(string userName);       
        Task<bool> VerifyUserData(string email);
        Task<SharedDataDTO> GetSharedData(string userName, int pipSheetId);
        Task<string> SaveUserData(UserDTO userDTO, string userName);
        Task<List<UserListResultDTO>> UploadMultipleUserData(string userName, byte[] byteArray);
    }
}