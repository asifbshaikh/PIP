namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class RoleAccountProjectDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public int ProjectId { get; set; }
        public int PipSheetId { get; set; }
    }
}
