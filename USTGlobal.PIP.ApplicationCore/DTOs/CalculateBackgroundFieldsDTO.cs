using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CalculateBackgroundFieldsDTO
    {
        public int PipSheetId { get; set; }
        public bool IsMarginSet { get; set; }
        public int Which { get; set; }
        public decimal MarginPercent { get; set; }
        public bool IsInitLoad { get; set; }
        public decimal InflatedCappedCost { get; set; }
        public decimal TotalInflation { get; set; }
    }
}
