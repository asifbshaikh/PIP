using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PriceAdjustmentResourceDTO
    {
        public int ProjectResourceId { get; set; }
        public int PipSheetId { get; set; }
        public int LocationId { get; set; }
        public bool UtilizationType { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Cost { get; set; }
        public decimal? RatePerHour { get; set; }
        public decimal? Yr1PerHour { get; set; }
        public decimal? Margin { get; set; }
        public decimal? CappedCost { get; set; }
        public decimal? TotalRevenue { get; set; }
        public IList<ProjectResourcePeriodDTO> projectResourcePeriodDTO { get; set; }
    }
}
