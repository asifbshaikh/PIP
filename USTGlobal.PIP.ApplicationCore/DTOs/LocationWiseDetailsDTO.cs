using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class LocationWiseDetailsDTO
    {
        public int DescriptionId { get; set; }
        public decimal Total { get; set; }
        public List<SummaryLocationDTO> SummaryLocationDTO { get; set; }
    }
}
