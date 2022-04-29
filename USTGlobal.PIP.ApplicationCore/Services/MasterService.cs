using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class MasterService : IMasterService
    {
        private readonly IMasterRepository masterRepository;
        private readonly IHolidayRepository holidayRepository;

        public MasterService(IMasterRepository masterRepo, IHolidayRepository holidayRepo)
        {
            this.masterRepository = masterRepo;
            this.holidayRepository = holidayRepo;
        }

        public async Task<List<MasterListDTO>> GetMasterListNames()
        {
            return await this.masterRepository.GetMasterListNames();
        }

        public async Task<List<MasterDTO>> GetMasters()
        {
            List<MasterDTO> masterDTOs = new List<MasterDTO>();
            List<MasterListDTO> masterListDTOs = await this.masterRepository.GetMasterListNames();

            foreach (var item in masterListDTOs)
            {
                if (item.MasterName == "Holiday")
                {
                    List<HolidayDTO> holidayDTOs = await this.holidayRepository.GetHolidays();
                    SetMasterDTO(item, masterDTOs, holidayDTOs);
                }
                else if (item.MasterName == "MilestoneGroup")
                {
                    List<MilestoneGroupDTO> milestoneGroupDTOs = await this.masterRepository.GetMilestoneGroups();
                    SetMasterDTO(item, masterDTOs, milestoneGroupDTOs);
                }
                else if (item.MasterName == "Milestone")
                {
                    List<MilestoneDTO> milestoneDTOs = await this.masterRepository.GetMilestones();
                    SetMasterDTO(item, masterDTOs, milestoneDTOs);
                }
            }

            return masterDTOs;
        }

        private void SetMasterDTO(MasterListDTO item, List<MasterDTO> masterDTOs, IList dto)
        {
                MasterDTO masterDTO = new MasterDTO();
                masterDTO.MasterId = item.MasterId;
                masterDTO.MasterName = item.MasterName;
                masterDTO.Data = dto;
                masterDTOs.Add(masterDTO);
        }

        public async Task<ServicePortfolioDTO> GetServicePortfolioData(int servicePortfolioId)
        {
            return await this.masterRepository.GetServicePortfolioData(servicePortfolioId);
        }

        public async Task<List<ServicePortfolioDTO>> GetAllServicePortfolios()
        {
            return await this.masterRepository.GetAllServicePortfolios();
        }
        public async Task<List<ServiceLineDTO>> GetServiceLineByPortfolio(int servicePortfolioId)
        {
            return await this.masterRepository.GetServiceLineByPortfolio(servicePortfolioId);
        }
        public async Task<List<ServiceLineDTO>> GetAllServiceLines()
        {
            return await this.masterRepository.GetAllServiceLines();
        }

        public async Task<List<ProjectBillingTypeDTO>> GetProjectBillingByDeliveryId(int projectDeliveryTypeId)
        {
            return await this.masterRepository.GetProjectBillingByDeliveryId(projectDeliveryTypeId);
        }
        public async Task<List<ProjectBillingTypeDTO>> GetAllProjectBillingData()
        {
            return await this.masterRepository.GetAllProjectBillingData();
        }

        public async Task<ProjectDeliveryTypeDTO> GetProjectDeliveryData(int projectDeliveryTypeId)
        {
            return await this.masterRepository.GetProjectDeliveryData(projectDeliveryTypeId);
        }
        public async Task<List<ProjectDeliveryTypeDTO>> GetAllProjectDeliveryData()
        {
            return await this.masterRepository.GetAllProjectDeliveryData();
        }
        public async Task<List<ContractingEntityDTO>> GetAllContractingEntities()
        {
            return await this.masterRepository.GetAllContractingEntities();
        }

        public async Task<List<LocationDTO>> GetLocations(int projectId, int pipSheetId)
        {
            return await this.masterRepository.GetLocations(projectId, pipSheetId);
        }
        public async Task<List<MilestoneGroupDTO>> GetMilestoneGroups()
        {
            return await this.masterRepository.GetMilestoneGroups();
        }
        public async Task<List<MilestoneDTO>> GetMilestones()
        {
            return await this.masterRepository.GetMilestones();
        }
        public async Task<List<MarkupDTO>> GetMarkup()
        {
            return await this.masterRepository.GetMarkup();
        }
        public async Task<List<ResourceDTO>> GetResources()
        {
            return await this.masterRepository.GetResources();
        }
        public async Task<List<ResourceGroupDTO>> GetResourceGroups()
        {
            return await this.masterRepository.GetResourceGroups();
        }
        public async Task<List<CurrencyDTO>> GetCurrencyData()
        {
            return await this.masterRepository.GetCurrencyData();
        }

        public async Task<List<CountryDTO>> GetCountries()
        {
            return await this.masterRepository.GetCountries();
        }
    }
}