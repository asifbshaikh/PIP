using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ResourcePlanningDTO : BaseDTO
    {
        public int UId { get; set; }
        public int ProjectResourceId { get; set; }
        public int PipSheetId { get; set; }
        public int LocationId { get; set; }
        public int? MarkupId { get; set; }
        public int? MilestoneId { get; set; }
        public int ResourceGroupId { get; set; }
        public int ResourceId { get; set; }
        public bool UtilizationType { get; set; }
        public bool IsDeleted { get; set; }
        public decimal? TotalhoursPerResource { get; set; }
        public decimal? CostHrsPerResource { get; set; }
        public decimal FTEPerResource { get; set; }
        public string Alias { get; set; }
        public int? NonBillableCategoryId { get; set; }
        public int ResourceServiceLineId { get; set; }
        public string ClientRole { get; set; }
        public IList<ProjectResourcePeriodDTO> ProjectPeriod { get; set; }
    }
}
