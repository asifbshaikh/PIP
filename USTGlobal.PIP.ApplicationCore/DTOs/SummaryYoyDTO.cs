using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SummaryYoyDTO
    {
        public int DescriptionId { get; set; }
        public decimal Total { get; set; }
        public IList<SummaryYoyYearDTO> SummaryYoyPeriodList { get; set; }
    }
}
