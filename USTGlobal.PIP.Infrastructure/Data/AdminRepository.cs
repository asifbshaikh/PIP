using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly PipContext pipContext;
        public AdminRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task DeleteUserRole(int userId, int accountId)
        {
            await this.pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_DeleteUserRole {0}, {1}",
                userId,
                accountId);

            await this.pipContext.SaveChangesAsync();
        }

        public async Task<List<AdminDTO>> GetAdmins(int accountId)
        {
            List<AdminDTO> adminDTOList = new List<AdminDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetAdminData")
               .WithSqlParam("@AccountId", accountId)
               .ExecuteStoredProcAsync((result) =>
               {
                   adminDTOList = result.ReadToList<AdminDTO>().ToList();
               });
            return adminDTOList;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            return await this.pipContext.User
                .Select(user => new UserDTO
                {
                    UserId = user.UserId,
                    UID = user.UID,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }).ToListAsync();
        }

        public async Task SaveAdminRole(AdminRoleDTO userRoleDTO, string userName)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveAdminRole {0}, {1}, {2}, {3}",
                userName,
                userRoleDTO.UserId,
                userRoleDTO.RoleId,
                userRoleDTO.AccountId);

            await pipContext.SaveChangesAsync();
        }

        public async Task SaveUserRoles(RoleManagementDTO roleDTO, string userName)
        {

            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveUserRoles {0}, {1}, {2}, {3}, {4}, {5}",
              userName,
              roleDTO.UserId,
              roleDTO.AccountId,
              roleDTO.IsEditor,
              roleDTO.IsReviewer,
              roleDTO.IsReadOnly);

            await pipContext.SaveChangesAsync();

        }

        public async Task<List<RoleManagementDTO>> getUsersAndRoles(int accountID)
        {
            List<RoleManagementDTO> userRoles = new List<RoleManagementDTO>();

            await pipContext.LoadStoredProc("dbo.sp_GetUsersAndRoles")
                .WithSqlParam("@AccountId", accountID)
                .ExecuteStoredProcAsync((resultingSet) =>
                {
                    List<RoleManagementDTO> users = resultingSet.ReadToList<RoleManagementDTO>().ToList();
                    resultingSet.NextResult();

                    List<RoleManagementDTO> roles = resultingSet.ReadToList<RoleManagementDTO>().ToList();
                    resultingSet.NextResult();


                    if (roles != null && roles.Count > 0) // given account has atleast one user assocciated with it
                    {
                        var nonAssociatedUsers = users.Select(u => u.UserId).Except(roles.Select(r => r.UserId)).ToList();
                        var associatedUsers = users.Select(u => u.UserId).Intersect(roles.Select(r => r.UserId)).ToList();

                        // Adding all Associated users 

                        if (associatedUsers.Count > 0)
                        {
                            associatedUsers.Sort();

                            foreach (int userId in associatedUsers)
                            {
                                var isEditor = roles.Find(role => role.RoleId == 3 && role.UserId == userId && role.AccountId == accountID);
                                var isReviewer = roles.Find(role => role.RoleId == 4 && role.UserId == userId && role.AccountId == accountID);
                                var isReadOnly = roles.Find(role => role.RoleId == 5 && role.UserId == userId && role.AccountId == accountID);

                                var user = users.Find(u => u.UserId == userId);

                                // binding the properties 
                                user.AccountId = accountID;
                                user.IsEditor = (isEditor != null) ? true : false;
                                user.IsReviewer = (isReviewer != null) ? true : false;
                                user.IsReadOnly = (isReadOnly != null) ? true : false;


                                userRoles.Add(user);
                            }
                        }

                        // Adding all Non Associated users  (*** 2 lists can be clubbed into one list)


                        if (nonAssociatedUsers != null && nonAssociatedUsers.Count > 0)
                        {
                            nonAssociatedUsers.Sort();

                            foreach (int userId in nonAssociatedUsers)
                            {
                                var user = users.Find(u => u.UserId == userId);
                                user.IsEditor = false;
                                user.IsReviewer = false;
                                user.IsReadOnly = false;

                                userRoles.Add(user);

                            }
                        }
                    }
                    else
                    {
                        userRoles = users;
                    }
                });


            return userRoles;

        }

        public async Task SaveSharedPipRole(SharedPipRoleDTO sharedPipRole, string userName)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveSharedPipRoles {0}, {1}, {2}, {3}, {4}",
              userName,
              sharedPipRole.PipSheetId,
              sharedPipRole.UserId,
              sharedPipRole.IsEditor,
              sharedPipRole.IsReadOnly);

            await pipContext.SaveChangesAsync();
        }

        public async Task<List<UserRoleReadOnly>> GetReadOnlyUserList()
        {
            return await (from urro in this.pipContext.UserRoleReadOnly
                          join u in this.pipContext.User on urro.UserId equals u.UserId
                          join r in this.pipContext.Role on urro.RoleId equals r.RoleId
                          select new UserRoleReadOnly()
                          {
                              UserId = u.UserId,
                              UID = u.UID,
                              Name = u.FirstName + " " + u.LastName,
                              RoleName = r.RoleName
                          }).Distinct().ToListAsync();
        }

        public async Task AssignReadOnlyRoleForAllAccounts(int userId, string userName)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_AssignReadOnlyRoleForAllAccounts {0}, {1}", userId, userName);
            await pipContext.SaveChangesAsync();
        }
        public async Task DeleteReadOnlyRoleForAllAccounts(int userId)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_DeleteReadOnlyRoleForAllAccounts {0}", userId);
            await pipContext.SaveChangesAsync();
        }

        public async Task<List<RoleManagementDTO>> getAllUsersAndAssociatedRoles()
        {
            List<RoleManagementDTO> userRoles = new List<RoleManagementDTO>();

            await pipContext.LoadStoredProc("sp_GetAllUsersAndAssociatedRoles")
               .ExecuteStoredProcAsync((resultingSet) =>
               {
                   List<RoleManagementDTO> users = resultingSet.ReadToList<RoleManagementDTO>().ToList();
                   resultingSet.NextResult();

                   List<RoleManagementDTO> roles = resultingSet.ReadToList<RoleManagementDTO>().ToList();
                   resultingSet.NextResult();

                   List<RoleManagementDTO> AllAccounts = resultingSet.ReadToList<RoleManagementDTO>().ToList();
                   resultingSet.NextResult();


                   //  extract accounts  in use
                   var accounts = roles.Select(x => x.AccountId).Distinct();

                   foreach (int accountId in accounts)
                   {
                       if (accountId != 0)
                       {
                           // extract users against the account
                           var accountUsers = roles.Where(U => U.AccountId == accountId).Select(x => x.UserId).Distinct().ToList();

                           accountUsers.Sort();

                           foreach (var accountUser in accountUsers)
                           {
                               //   get users
                               var foundUser = users.Find(u => u.UserId == accountUser);

                               var user = new RoleManagementDTO();

                               //This logic can be extracted in a common method.
                               user.FirstName = foundUser.FirstName;
                               user.LastName = foundUser.LastName;
                               user.UID = foundUser.UID;
                               user.UserId = foundUser.UserId;
                               user.AccountId = accountId;
                               user.IsAdmin = roles.Exists(role => role.RoleId == 1 && role.UserId == user.UserId && role.AccountId == 0);
                               user.IsFinanceApprover = roles.Exists(role => role.RoleId == 2 && role.UserId == user.UserId && role.AccountId == accountId);
                               user.IsEditor = roles.Exists(role => role.RoleId == 3 && role.UserId == user.UserId && role.AccountId == accountId);
                               user.IsReviewer = roles.Exists(role => role.RoleId == 4 && role.UserId == user.UserId && role.AccountId == accountId);
                               user.IsReadOnly = roles.Exists(role => role.RoleId == 5 && role.UserId == user.UserId && role.AccountId == accountId);
                               user.IsAllAccountReadOnly = AllAccounts.Exists(role => role.UserId == user.UserId);

                               userRoles.Add(user);
                           }

                       }


                   }


               });

            return userRoles;
        }
    }
}
