using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPartnerCostAndRevenueWorkflowService
    {
        Task ProcessPartnerCostAndRevenueSaving(string userName, PartnerCostAndRevenueDTO partnerCostAndRevenueDTO);
    }
}
