namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectHeaderCurrencyDTO
    {
        public ProjectHeaderDTO ProjectHeader { get; set; }
        public CurrencyDTO Currency { get; set; }
        public int? TotalVersionsPresent { get; set; }
    }
}
