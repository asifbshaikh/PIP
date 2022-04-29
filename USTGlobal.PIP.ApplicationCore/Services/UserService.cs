using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IUploadExcelService uploadExcelService;

        public UserService(IUserRepository userRepo, IUploadExcelService uploadExcelService)
        {
            this.userRepository = userRepo;
            this.uploadExcelService = uploadExcelService;
        }

        public async Task<UserRoleDTO> GetUserData(string userName)
        {
            UserDataRoleDTO userDataRoleDTO = await this.userRepository.GetUserData(userName);
            UserRoleDTO userRoleDTO = null;
            if (userDataRoleDTO.UserDTO.Count != 0)
            {
                userRoleDTO = (new UserRoleDTO
                {
                    UserId = userDataRoleDTO.UserDTO[0].UserId,
                    Email = userDataRoleDTO.UserDTO[0].Email,
                    FirstName = userDataRoleDTO.UserDTO[0].FirstName,
                    LastName = userDataRoleDTO.UserDTO[0].LastName,
                    UID = userDataRoleDTO.UserDTO[0].UID,
                    IsActive = userDataRoleDTO.UserDTO[0].IsActive,
                    Role = userDataRoleDTO.RoleDTO
                });
            }
            return userRoleDTO;
        }

        public async Task<bool> VerifyUserData(string email)
        {
            return await this.userRepository.VerifyUserData(email);
        }

        public async Task<SharedDataDTO> GetSharedData(string userName, int pipSheetId)
        {
            return await this.userRepository.GetSharedData(userName, pipSheetId);
        }

        public async Task<string> SaveUserData(UserDTO userDTO, string userName)
        {
            return await this.userRepository.SaveUserData(userDTO, userName);
        }

        public async Task<List<UserListResultDTO>> UploadMultipleUserData(string userName, byte[] byteArray)
        {
            var userDTOList = await this.uploadExcelService.ReadExcelToList(byteArray);
            if (userDTOList.Count != 0)
            {
                IList<UserListDTO> userListDTO = new List<UserListDTO>();
                foreach (UserListDTO userList in userDTOList)
                {
                    UserListDTO user = new UserListDTO();
                    user.FirstName = userList.FirstName;
                    user.LastName = userList.LastName;
                    user.Email = userList.UID == null ? null : userList.UID + "@ust-global.com";
                    user.IsActive = true;
                    user.UID = userList.UID;
                    user.UserEmail = userList.Email;
                    userListDTO.Add(user);
                }
                return await this.userRepository.UploadMultipleUserData(userName, userListDTO);
            }
            else
            {
                List<UserListResultDTO> userListResults = new List<UserListResultDTO>();
                UserListResultDTO userListResultDTO = new UserListResultDTO();
                userListResultDTO.Status = "Failed";
                userListResultDTO.Message = "No Content";
                userListResults.Add(userListResultDTO);
                return userListResults;
            }
            
        }
    }
}
