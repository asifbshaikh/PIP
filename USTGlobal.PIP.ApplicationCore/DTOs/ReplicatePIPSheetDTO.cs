namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReplicatePIPSheetDTO : BaseDTO
    {
        public int SourcePIPSheetId { get; set; }
        public int SourceProjectId { get; set; }
        public string SFProjectId { get; set; }
        public string ProjectNamePerSF { get; set; }
        public int AccountId { get; set; }
        public decimal paymentLag { get; set; }
        public bool IsDummy { get; set; }
        public int ReplicateType { get; set; }
        public int VersionNumber { get; set; }
    }
}
