using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IMasterRepository
    {
        Task<List<MasterListDTO>> GetMasterListNames();
        Task<ServicePortfolioDTO> GetServicePortfolioData(int servicePortfolioId);
        Task<List<ServicePortfolioDTO>> GetAllServicePortfolios();
        Task<List<ServiceLineDTO>> GetServiceLineByPortfolio(int servicePortfolioId);
        Task<List<ServiceLineDTO>> GetAllServiceLines();
        Task<List<ProjectBillingTypeDTO>> GetProjectBillingByDeliveryId(int projectDeliveryTypeId);
        Task<List<ProjectBillingTypeDTO>> GetAllProjectBillingData();
        Task<ProjectDeliveryTypeDTO> GetProjectDeliveryData(int projectDeliveryTypeId);
        Task<List<ProjectDeliveryTypeDTO>> GetAllProjectDeliveryData();
        Task<List<ContractingEntityDTO>> GetAllContractingEntities();
        Task<List<LocationDTO>> GetLocations();
        Task<List<LocationDTO>> GetLocations(int projectId, int pipSheetId);
        Task<string> GetLocationById(int locationId);
        Task<List<MilestoneGroupDTO>> GetMilestoneGroups();
        Task<List<MilestoneDTO>> GetMilestones();
        Task<List<MarkupDTO>> GetMarkup();
        Task<List<ResourceDTO>> GetResources();
        Task<List<ResourceGroupDTO>> GetResourceGroups();
        Task<List<HolidayDTO>> GetHolidays();
        Task<List<CurrencyDTO>> GetCurrencyData();
        Task<List<CountryDTO>> GetCountries();
    }
}