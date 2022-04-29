using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ServiceLineEbitdaDTO
    {
        public int ServiceLineId { get; set; }
        public string ServiceLineName { get; set; }
        public decimal ServiceLineEbitdaPercent { get; set; }
        public string GroupName { get; set; }
    }
}
