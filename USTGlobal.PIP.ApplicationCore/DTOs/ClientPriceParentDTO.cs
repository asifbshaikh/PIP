using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ClientPriceParentDTO : ClientPriceDTO
    {
        public List<ClientPricePeriodDTO> ClientPricePeriodDTO { get; set; }
    }
}
