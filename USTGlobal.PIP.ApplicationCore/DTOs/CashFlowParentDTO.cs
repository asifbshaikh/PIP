using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class CashFlowParentDTO : ClientPriceDTO
    { 
        public List<CashFlowDTO> CashFlowDTO { get; set; }
    }
}
