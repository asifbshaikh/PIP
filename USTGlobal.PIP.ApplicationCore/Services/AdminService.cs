using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository adminRepository;
        private readonly IEmailNotificationService emailNotificationService;

        public bool isFromAdminScreen { get; set; }

        public AdminService(IAdminRepository adminRepository, IEmailNotificationService emailNotificationService)
        {
            this.adminRepository = adminRepository;
            this.emailNotificationService = emailNotificationService;
        }
        public async Task DeleteUserRole(int userId, int accountId, string userName)
        {
            await this.adminRepository.DeleteUserRole(userId, accountId);
            OperationType OT = isFromAdminScreen ? OperationType.AdminDelete : OperationType.FinanceApproverDelete;
            await this.InitiateEmailProcess(userId, OT, userName, null, null, accountId);
        }
        public async Task<List<AdminDTO>> GetAdmins(int accountId)
        {
            return await this.adminRepository.GetAdmins(accountId);
        }
        public async Task<List<UserDTO>> GetUsers()
        {
            return await this.adminRepository.GetUsers();
        }
        public async Task SaveAdminRole(AdminRoleDTO adminRoleDTO, string userName)
        {
            await this.adminRepository.SaveAdminRole(adminRoleDTO, userName);
            OperationType OT = isFromAdminScreen ? OperationType.AdminAdd : OperationType.FinanceApproverAdd;
            await this.InitiateEmailProcess(adminRoleDTO, OT, userName);
        }
        public async Task SaveUserRoles(RoleManagementDTO currentRoles, string userName)
        {
            RoleManagementDTO previousRoles = null;

            //  getting previous role before making latest updates and check condition 
            List<RoleManagementDTO> usersAndRoles = await this.adminRepository.getUsersAndRoles(currentRoles.AccountId);
            if (usersAndRoles != null && usersAndRoles.Count > 0)
            {
                previousRoles = usersAndRoles.Find(u => u.UserId == currentRoles.UserId);
            }

            await this.adminRepository.SaveUserRoles(currentRoles, userName);

            if (previousRoles.IsEditor != currentRoles.IsEditor || previousRoles.IsReviewer != currentRoles.IsReviewer
                || previousRoles.IsReadOnly != currentRoles.IsReadOnly)
            {
                await this.InitiateEmailProcess(currentRoles, OperationType.RoleAndPermission, userName, currentRoles, previousRoles);
            }
        }
        public async Task<List<RoleManagementDTO>> getUsersAndRoles(int accountId)
        {
            return await this.adminRepository.getUsersAndRoles(accountId);
        }
        public async Task SaveSharedPipRole(SharedPipRoleDTO sharedPipRole, string userName)
        {
            await this.adminRepository.SaveSharedPipRole(sharedPipRole, userName);
        }
        public async Task<List<UserRoleReadOnly>> GetReadOnlyUserList()
        {
            return await this.adminRepository.GetReadOnlyUserList();
        }
        public async Task AssignReadOnlyRoleForAllAccounts(int userId, string userName)
        {
            await this.adminRepository.AssignReadOnlyRoleForAllAccounts(userId, userName);
            await this.InitiateEmailProcess(userId, OperationType.AllReadOnly, userName);
        }
        public async Task DeleteReadOnlyRoleForAllAccounts(int userId, string userName)
        {
            await this.adminRepository.DeleteReadOnlyRoleForAllAccounts(userId);
            await this.InitiateEmailProcess(userId, OperationType.DeleteAllReadOnly, userName);
        }

        public async Task<List<RoleManagementDTO>> getAllUsersAndAssociatedRoles()
        {
            return await this.adminRepository.getAllUsersAndAssociatedRoles();
        }


        private async Task InitiateEmailProcess(dynamic operationDTO, OperationType operationType, string senderEmaild, RoleManagementDTO currentRole = null, RoleManagementDTO previousRole = null, int AccountId = 0)
        {
            EmailDTO emailDto = await emailNotificationService.PreComposeEmail(operationDTO, operationType, senderEmaild, AccountId);
            var processedEmailData = await emailNotificationService.ProcessEmail(emailDto);

            if (operationType == OperationType.RoleAndPermission)
            {
                processedEmailData.TemplateData.WasAdmin = previousRole.IsAdmin;
                processedEmailData.TemplateData.OldAccessName += previousRole.IsAdmin ? "Admin " : "";
                processedEmailData.TemplateData.WasEditor = previousRole.IsEditor;
                processedEmailData.TemplateData.OldAccessName += previousRole.IsEditor ? "Editor " : "";
                processedEmailData.TemplateData.WasReviewer = previousRole.IsReviewer;
                processedEmailData.TemplateData.OldAccessName += previousRole.IsReviewer ? "Reviewer " : "";
                processedEmailData.TemplateData.WasReadOnly = previousRole.IsReadOnly;
                processedEmailData.TemplateData.OldAccessName += previousRole.IsReadOnly ? "ReadOnly " : "";
                processedEmailData.TemplateData.WasFinanceApprover = previousRole.IsFinanceApprover;
                processedEmailData.TemplateData.OldAccessName += previousRole.IsFinanceApprover ? "Finance-Approver" : "";
                processedEmailData.TemplateData.IsAdmin = currentRole.IsAdmin;
                processedEmailData.TemplateData.NewAccessName += currentRole.IsAdmin ? "Admin " : "";
                processedEmailData.TemplateData.IsEditor = currentRole.IsEditor;
                processedEmailData.TemplateData.NewAccessName += currentRole.IsEditor ? "Editor " : "";
                processedEmailData.TemplateData.IsReviewer = currentRole.IsReviewer;
                processedEmailData.TemplateData.NewAccessName += currentRole.IsReviewer ? "Reviewer " : "";
                processedEmailData.TemplateData.IsReadOnly = currentRole.IsReadOnly;
                processedEmailData.TemplateData.NewAccessName += currentRole.IsReadOnly ? "ReadOnly " : "";
                processedEmailData.TemplateData.IsFinanceApprover = currentRole.IsFinanceApprover;
                processedEmailData.TemplateData.NewAccessName += currentRole.IsFinanceApprover ? "Finance-Approver" : "";

                //Receiver credentials setting
                processedEmailData.TemplateData.ReceiverFirstName = currentRole.FirstName;
                processedEmailData.TemplateData.ReceiverLastName = currentRole.LastName;

                processedEmailData.From = processedEmailData.TemplateData.SenderEmailId;
                processedEmailData.TemplateData.OldAccessName = processedEmailData.TemplateData.OldAccessName == "" ? "None" : processedEmailData.TemplateData.OldAccessName;
                processedEmailData.TemplateData.NewAccessName = processedEmailData.TemplateData.NewAccessName == "" ? "None" : processedEmailData.TemplateData.NewAccessName;
            }

            emailDto = await emailNotificationService.PostComposeEmail(processedEmailData, null, operationType);

            //  send email 
            await emailNotificationService.SendEmail(emailDto);
        }
    }
}
