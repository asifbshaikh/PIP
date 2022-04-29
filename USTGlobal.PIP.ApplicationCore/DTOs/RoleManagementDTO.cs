namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class RoleManagementDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UID { get; set; }
        public int RoleId { get; set; }
        public int AccountId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsFinanceApprover { get; set; }
        public bool IsEditor { get; set; }
        public bool IsReviewer { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsAllAccountReadOnly { get; set; }
    }
}
