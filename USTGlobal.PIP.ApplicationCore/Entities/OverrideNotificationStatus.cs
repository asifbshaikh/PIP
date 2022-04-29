namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class OverrideNotification
    {
        public int PipSheetId { get; set; }
        public bool? VacationAbsence { get; set; }
        public bool? RiskManagement { get; set; }
        public bool? ClientPrice { get; set; }
        public bool? EbitdaStdOverhead { get; set; }
    }
}
