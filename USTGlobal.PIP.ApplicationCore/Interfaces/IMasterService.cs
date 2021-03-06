using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IMasterService
    {
        Task<List<MasterListDTO>> GetMasterListNames();
        Task<List<MasterDTO>> GetMasters();
        Task<ServicePortfolioDTO> GetServicePortfolioData(int servicePortfolioId);
        Task<List<ServicePortfolioDTO>> GetAllServicePortfolios();
        Task<List<ServiceLineDTO>> GetServiceLineByPortfolio(int servicePortfolioId);
        Task<List<ServiceLineDTO>> GetAllServiceLines();
        Task<List<ProjectBillingTypeDTO>> GetProjectBillingByDeliveryId(int projectDeliveryTypeId);
        Task<List<ProjectBillingTypeDTO>> GetAllProjectBillingData();
        Task<ProjectDeliveryTypeDTO> GetProjectDeliveryData(int projectDeliveryTypeId);
        Task<List<ProjectDeliveryTypeDTO>> GetAllProjectDeliveryData();
        Task<List<ContractingEntityDTO>> GetAllContractingEntities();
        Task<List<LocationDTO>> GetLocations(int projectId, int pipSheetId);
        Task<List<MilestoneGroupDTO>> GetMilestoneGroups();
        Task<List<MilestoneDTO>> GetMilestones();
        Task<List<MarkupDTO>> GetMarkup();
        Task<List<ResourceDTO>> GetResources();
        Task<List<ResourceGroupDTO>> GetResourceGroups();
        Task<List<CurrencyDTO>> GetCurrencyData();
        Task<List<CountryDTO>> GetCountries();
    }
}