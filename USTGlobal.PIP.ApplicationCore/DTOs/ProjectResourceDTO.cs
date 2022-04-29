﻿namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectResourceDTO
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
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public decimal? TotalHoursPerResource { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Cost { get; set; }
        public decimal? RatePerHour { get; set; }
        public decimal? Yr1PerHour { get; set; }
        public decimal? Margin { get; set; }
        public decimal? CappedCost { get; set; }
        public decimal? TotalRevenue { get; set; }
        public decimal? CostHrsPerResource { get; set; }
        public decimal FTEPerResource { get; set; }
        public string Alias { get; set; }
        public int? NonBillableCategoryId { get; set; }
        public int ResourceServiceLineId { get; set; }
        public string ClientRole { get; set; }
        public decimal ResourceSeatCost { get; set; }
    }
}
