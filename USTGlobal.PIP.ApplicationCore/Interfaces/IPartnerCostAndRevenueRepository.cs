using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;


namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPartnerCostAndRevenueRepository
    {
        Task<PartnerCostAndRevenueParentDTO> GetPartnerCostAndRevenue(int pipSheetId);
        Task SavePartnerCostAndRevenueData(string userName, IList<PartnerCostDTO> partnerCostDTO, IList<PartnerCostPeriodDetailDTO> partnerCostPeriodDetailDTO, IList<PartnerRevenueDTO> partnerRevenueDTO, IList<PartnerRevenuePeriodDetailDTO> partnerRevenuePeriodDetailDTO, IList<PartnerCostPeriodTotalDTO> partnerCostPeriodTotalDTO, IList<PartnerRevenuePeriodTotalDTO> partnerRevenuePeriodTotalDTO);
    }
}
