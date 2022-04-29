namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class UserRoleMainDTO : UserDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
    }
}
