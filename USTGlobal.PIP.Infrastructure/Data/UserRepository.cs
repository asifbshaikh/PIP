using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly PipContext pipContext;

        public UserRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<UserRoleDTO> GetUserData(int userId)
        {
            return await this.pipContext.User
                    .Where(user => user.UserId == userId)
                    .Select(user => new UserRoleDTO
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = (this.pipContext.Role.Where(r => (this.pipContext.UserRole
                                                  .Where(userRole => userRole.UserId == userId)
                                                  .Select(ra => ra.RoleId)).Contains(r.RoleId))
                                                  .Select(r => new RoleDTO
                                                  {
                                                      RoleId = r.RoleId,
                                                      RoleName = r.RoleName
                                                  })).ToList()
                    }).SingleOrDefaultAsync();

        }

        public async Task<bool> VerifyUserData(string email)
        {
            return await this.pipContext.User.AnyAsync(x => ((x.Email == email) && (x.IsActive)));
        }

        public async Task<UserDataRoleDTO> GetUserData(string emailId)
        {
            UserDataRoleDTO userDataRoleDTO = new UserDataRoleDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetUserData")
             .WithSqlParam("@UserName", emailId)
              .ExecuteStoredProcAsync((userDataResultSet) =>
              {
                  userDataRoleDTO.UserDTO = userDataResultSet.ReadToList<UserDTO>();
                  userDataResultSet.NextResult();

                  userDataRoleDTO.RoleDTO = userDataResultSet.ReadToList<RoleDTO>().ToList();
                  userDataResultSet.NextResult();
              });
            return userDataRoleDTO;
        }

        public async Task<SharedDataDTO> GetSharedData(string emailId, int pipSheetId)
        {
            SharedDataDTO sharedDataDTO = new SharedDataDTO();
            UserRoleAccountDTO userRoleAccountDTO = new UserRoleAccountDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetSharedData")
              .WithSqlParam("@UserName", emailId)
              .WithSqlParam("@PipSheetId", pipSheetId)
               .ExecuteStoredProcAsync((sharedDataResultSet) =>
               {
                   var userDTO = sharedDataResultSet.ReadToList<UserDTO>().FirstOrDefault();
                   sharedDataResultSet.NextResult();
                   if (userDTO != null)
                   {
                       var userRoleMainDTO = sharedDataResultSet.ReadToList<UserRoleMainDTO>();
                       userRoleAccountDTO.UserId = userDTO.UserId;
                       userRoleAccountDTO.Email = userDTO.Email;
                       userRoleAccountDTO.FirstName = userDTO.FirstName;
                       userRoleAccountDTO.LastName = userDTO.LastName;
                       userRoleAccountDTO.UID = userDTO.UID;
                       if (userRoleMainDTO.Count > 0)
                       {
                           userRoleAccountDTO.RoleAndAccountDTO = (from s in userRoleMainDTO
                                                                   select new RoleAndAccountDTO()
                                                                   {
                                                                       RoleId = s.RoleId,
                                                                       RoleName = s.RoleName,
                                                                       AccountId = s.AccountId,
                                                                       AccountName = s.AccountName
                                                                   }).ToList();
                       }
                       sharedDataDTO.UserRoleAccountDTO = userRoleAccountDTO;
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.HasAccountLevelAccess = Convert.ToBoolean(sharedDataResultSet.ReadToValue<bool>());
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.HasSharePipAccess = Convert.ToBoolean(sharedDataResultSet.ReadToValue<bool>());
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.HasDummyPipAccess = Convert.ToBoolean(sharedDataResultSet.ReadToValue<bool>());
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.HasAccountLevelEditorAccess = Convert.ToBoolean(sharedDataResultSet.ReadToValue<bool>());
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.HasFinanceApproverAccess = Convert.ToBoolean(sharedDataResultSet.ReadToValue<bool>());
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.CurrencyId = sharedDataResultSet.ReadToValue<int>();
                       sharedDataResultSet.NextResult();

                       var servicePortfolioDTO = sharedDataResultSet.ReadToList<ServicePortfolioDTO>();
                       sharedDataDTO.ServicePortfolioDTO = servicePortfolioDTO;
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.ServiceLineDTO = sharedDataResultSet.ReadToList<ServiceLineDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.ProjectDeliveryTypeDTO = sharedDataResultSet.ReadToList<ProjectDeliveryTypeDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.ProjectBillingTypeDTO = sharedDataResultSet.ReadToList<ProjectBillingTypeDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.ContractingEntityDTO = sharedDataResultSet.ReadToList<ContractingEntityDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.LocationDTO = sharedDataResultSet.ReadToList<LocationDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.MarkupDTO = sharedDataResultSet.ReadToList<MarkupDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.ResourceDTO = sharedDataResultSet.ReadToList<ResourceDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.ResourceGroupDTO = sharedDataResultSet.ReadToList<ResourceGroupDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.HolidayDTO = sharedDataResultSet.ReadToList<HolidayDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.MilestoneDTO = sharedDataResultSet.ReadToList<MilestoneDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.MilestoneGroupDTO = sharedDataResultSet.ReadToList<MilestoneGroupDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.CurrencyDTO = sharedDataResultSet.ReadToList<CurrencyDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.CountryDTO = sharedDataResultSet.ReadToList<CountryDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.StandardCostRateDTO = sharedDataResultSet.ReadToList<StandardCostRateDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.CorpBillingRateDTO = sharedDataResultSet.ReadToList<CorpBillingRateDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.BasicAssetDTO = sharedDataResultSet.ReadToList<BasicAssetDTO>().ToList();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.DefaultLabelDTO = sharedDataResultSet.ReadToList<DefaultLabelDTO>().ToList();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.ProjectDeliveryBillingTypeDTO = sharedDataResultSet.ReadToList<ProjectDeliveryBillingTypeDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.BillingYearDTO = sharedDataResultSet.ReadToList<BillingYearDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.NonBillableCategoryDTO = sharedDataResultSet.ReadToList<NonBillableCategoryDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.AccountDTO = sharedDataResultSet.ReadToList<AccountDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.RoleDTO = sharedDataResultSet.ReadToList<RoleDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.ResourceServiceLineDTO = sharedDataResultSet.ReadToList<ResourceServiceLineDTO>();
                       sharedDataResultSet.NextResult();

                       sharedDataDTO.PipSheetWorkflowStatus = sharedDataResultSet.ReadToList<PipSheetWorkflowStatus>();
                   }
               });
            return sharedDataDTO;
        }

        public async Task<string> SaveUserData(UserDTO userDTO, string userName)
        {
            SqlParameter[] inputParams = new SqlParameter[6];

            inputParams[0] = new SqlParameter("@UserName", userName);
            inputParams[1] = new SqlParameter("@FirstName", userDTO.FirstName);
            inputParams[2] = new SqlParameter("@LastName", userDTO.LastName);
            inputParams[3] = new SqlParameter("@UserEmail", userDTO.Email);
            inputParams[4] = new SqlParameter("@UID", userDTO.UID);
            inputParams[5] = new SqlParameter("@OutputId", SqlDbType.Int);
            inputParams[5].Direction = ParameterDirection.Output;

            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveUserData @UserName, @FirstName," +
                   " @LastName, @UserEmail, @UID, @OutputId OUTPUT", inputParams);

            await pipContext.SaveChangesAsync();

            int outputParam = Convert.ToInt32(inputParams[5].Value);

            if (outputParam == 0)
            {
                return userDTO.Email;
            }
            else
            {
                return null;
            }


        }

        public async Task<List<UserListResultDTO>> UploadMultipleUserData(string userName, IList<UserListDTO> userListDTO)
        {
            List<UserListResultDTO> userListResultDTO = new List<UserListResultDTO>();

            await pipContext.LoadStoredProc("dbo.sp_SaveMultipleUserList")
            .WithSqlParam("@UserName", userName)
            .WithSqlParam("@InputUserList", new SqlParameter("@InputUserList", SqlDbType.Structured)
            {
                Value = IListToDataTableHelper.ToDataTables(userListDTO),
                TypeName = "dbo.UserList"
            })
            .ExecuteStoredProcAsync((userListResultSet) =>
            {
                userListResultDTO = userListResultSet.ReadToList<UserListResultDTO>().ToList();
            });

            return userListResultDTO;


        }
    }
}
