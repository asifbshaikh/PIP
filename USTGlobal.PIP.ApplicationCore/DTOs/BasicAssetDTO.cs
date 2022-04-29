using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class BasicAssetDTO
    {
        public int BasicAssetId { get; set; }
        public string Description { get; set; }
        public decimal CostToProject { get; set; }        
        public int MasterVersionId { get; set; }
    }
}
