using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// MasterController
    /// </summary>
    [Route("api/master")]
    public class MasterController : BaseController
    {
        private readonly IMasterService masterService;

        /// <summary>
        /// MasterController constructor
        /// </summary>
        /// <param name="masterService"></param>
        public MasterController(IMasterService masterService)
        {
            this.masterService = masterService;
        }

        /// <summary>
        /// Get MasterList
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("listNames")]
        public async Task<List<MasterListDTO>> GetMasterListNames()
        {
            return await this.masterService.GetMasterListNames();
        }

        /// <summary>
        /// Get Masters
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<MasterDTO>> GetMasters()
        {
            return await this.masterService.GetMasters();
        }

        /// <summary>
        /// Get All ServicePortfolios data
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("servicePortfolios")]
        public async Task<List<ServicePortfolioDTO>> GetAllServicePortfolios()
        {
            return await this.masterService.GetAllServicePortfolios();
        }

        /// <summary>
        /// Get ServiceLine based on ServicePortfolioId
        /// </summary>
        /// <param name="servicePortfolioId"></param>
        /// <returns></returns>
        [HttpGet, Route("portfolio/{servicePortfolioId}/serviceLines")]
        public async Task<List<ServiceLineDTO>> GetServiceLineByPortfolio(int servicePortfolioId)
        {
            return await this.masterService.GetServiceLineByPortfolio(servicePortfolioId);
        }

        /// <summary>
        /// Get All ServiceLines data
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("serviceLines")]
        public async Task<List<ServiceLineDTO>> GetAllServiceLines()
        {
            return await this.masterService.GetAllServiceLines();
        }

        /// <summary>
        /// Get ProjectBillingType based on ProjectDeliveryTypeId
        /// </summary>
        /// <param name="projectDeliveryId"></param>
        /// <returns></returns>
        [HttpGet, Route("projectDelivery/{projectDeliveryId}/billing")]
        public async Task<List<ProjectBillingTypeDTO>> GetProjectBillingByDeliveryId(int projectDeliveryId)
        {
            return await masterService.GetProjectBillingByDeliveryId(projectDeliveryId);
        }

        /// <summary>
        /// Get All ProjectDeliveryType data
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("projectDelivery")]
        public async Task<List<ProjectDeliveryTypeDTO>> GetAllProjectDeliveryData()
        {
            return await this.masterService.GetAllProjectDeliveryData();
        }

        /// <summary>
        /// Get All ProjectBillingType data
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("projectBilling")]
        public async Task<List<ProjectBillingTypeDTO>> GetAllProjectBillingData()
        {
            return await this.masterService.GetAllProjectBillingData();
        }

        /// <summary>
        /// Get All ContractingEntities data
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("contractingEntities")]
        public async Task<List<ContractingEntityDTO>> GetAllContractingEntities()
        {
            return await this.masterService.GetAllContractingEntities();
        }

        /// <summary>
        /// Get Locations based on ProjectId and PipSheetId
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("project/{projectId}/pipSheet/{pipSheetId}/locations")]
        public async Task<List<LocationDTO>> GetLocations(int projectId, int pipSheetId)
        {
            return await this.masterService.GetLocations(projectId, pipSheetId);
        }

        /// <summary>
        /// Get Milestones
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("milestones")]
        public async Task<List<MilestoneDTO>> GetMilestones()
        {
            return await this.masterService.GetMilestones();
        }

        /// <summary>
        /// Get MilestoneGroups
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("milestoneGroups")]
        public async Task<List<MilestoneGroupDTO>> GetMilestoneGroups()
        {
            return await this.masterService.GetMilestoneGroups();
        }

        /// <summary>
        /// Get All SkillSets
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("markup")]
        public async Task<List<MarkupDTO>> GetAllMarkup()
        {
            return await this.masterService.GetMarkup();
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("resources")]
        public async Task<List<ResourceDTO>> GetAllResources()
        {
            return await this.masterService.GetResources();
        }

        /// <summary>
        /// Get All RoleGroups
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("resourceGroups")]
        public async Task<List<ResourceGroupDTO>> GetAllResourceGroups()
        {
            return await this.masterService.GetResourceGroups();
        }

        /// <summary>
        /// Get Currency Conversion data
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("currencies")]
        public async Task<List<CurrencyDTO>> GetCurrencyData()
        {
            return await this.masterService.GetCurrencyData();
        }


        /// <summary>
        /// Get Countires Master data
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("countries")]
        public async Task<List<CountryDTO>> GetCountries()
        {
            return await this.masterService.GetCountries();
        }
    }
}
