using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ResourceLaborPricingDTO : ResourceLaborPricingSubDTO
    {
        public int LocationId { get; set; }
        public string MilestoneName { get; set; }
        public string LocationName { get; set; }
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public decimal StandardCostRate { get; set; }
        public decimal Percent { get; set; }
        public IList<ProjectResourcePeriodDTO> projectResourcePeriodDTO { get; set; }
        public decimal? TotalHoursPerResource { get; set; }
        public decimal? CostHrsPerResource { get; set; }
        public decimal? TotalInflation { get; set; }
        public string GradeClientRole { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
}
