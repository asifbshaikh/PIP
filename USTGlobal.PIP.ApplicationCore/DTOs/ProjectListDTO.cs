namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectListDTO
    {
        public int ProjectId { get; set; }
        public string SFProjectId { get; set; }
        public string ProjectName { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string ServiceLine { get; set; }
        public string DeliveryType { get; set; }
        public string BillingType { get; set; }
        public int PipSheetId { get; set; }
        public bool IsDummy { get; set; }
        public int? CurrencyId { get; set; }
        public string PipSheetStatus { get; set; }
    }
}
