using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class AssetDTO : BaseDTO
    {
        public int ProjectAssetId { get; set; }
        public int PIPSheetId { get; set; }
        public int? BasicAssetId { get; set; }
        public string Description { get; set; }
        public decimal? CostToProject { get; set; }
        public int? Count { get; set; }
        public decimal? Amount { get; set; }
    }
}
