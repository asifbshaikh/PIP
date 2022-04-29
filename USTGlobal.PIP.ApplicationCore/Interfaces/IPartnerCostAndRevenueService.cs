using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPartnerCostAndRevenueService
    {
        Task<PartnerCostAndRevenueDTO> GetPartnerCostAndRevenue(int pipSheetId);
        Task SavePartnerCostAndRevenueData(string userName, PartnerCostAndRevenueDTO partnerCostAndRevenueDTO);
        PartnerCostAndRevenueDTO ReAssignUIds(PartnerCostAndRevenueDTO partnerCostAndRevenue);
    }
}
