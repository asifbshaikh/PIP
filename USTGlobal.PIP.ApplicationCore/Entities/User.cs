namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public partial class User : BaseEntity
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string UID { get; set; }
        public string UserEmail { get; set; }
    }
}
