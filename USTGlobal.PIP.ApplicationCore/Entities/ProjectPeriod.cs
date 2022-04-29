namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ProjectPeriod : BaseEntity
    {
        public int ProjectPeriodId { get; set; }
        public int PipSheetId { get; set; }
        public int BillingPeriodId { get; set; }
        public int ProjectPeriodYear { get; set; }
        public int ProjectPeriodMonth { get; set; }
    }
}
