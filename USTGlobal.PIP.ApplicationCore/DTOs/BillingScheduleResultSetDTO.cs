using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class BillingScheduleResultSetDTO
    {
        public BillingScheduleCalculatedValueDTO billingScheduleCalculatedValueDTO { get; set; }
        public List<BillingScheduleProjectResourceDTO> billingScheduleProjectResourceDTO { get; set; }
        public List<ClientPriceDTO> clientPriceDTO { get; set; }
        public List<ClientPricePeriodDTO> clientPricePeriodDTO { get; set; }
        public IList<ProjectPeriodDTO> projectPeriodDTO { get; set; }
    }
}
