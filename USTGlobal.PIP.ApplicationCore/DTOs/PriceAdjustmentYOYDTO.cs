using System;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PriceAdjustmentYoyDTO : BaseDTO
    {
        public int PipSheetId { get; set; }
        public DateTime TriggerDate { get; set; }
        public DateTime EffectiveDate { get; set; }        
    }
}
