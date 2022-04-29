using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ServiceLineRevenueDTO
    {
        public int ServiceLineId { get; set; }
        public string ServiceLineName { get; set; }
        public decimal ServiceLineRevenue { get; set; }
        public string GroupName { get; set; }
    }
}
