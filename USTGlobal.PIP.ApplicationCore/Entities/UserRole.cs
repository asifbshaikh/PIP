namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class UserRole : BaseEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int? AccountId { get; set; }
    }
}
