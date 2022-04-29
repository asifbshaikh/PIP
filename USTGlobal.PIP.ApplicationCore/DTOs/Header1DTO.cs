using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class Header1DTO
    {
        public string SFAccountName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProjectName { get; set; }
        public int VersionNumber { get; set; }
        public string SFProjectId { get; set; }
        public string Currency { get; set; }
        public decimal? TotalClientPrice { get; set; }
        public int TotalVersionsPresent { get; set; }
    }
}
