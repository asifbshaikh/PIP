using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class MasterRepository : IMasterRepository
    {
        private readonly PipContext pipContext;
        public MasterRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<MasterListDTO>> GetMasterListNames()
        {
            return await this.pipContext.MasterList
                .Select(master => new MasterListDTO
                {
                    MasterId = master.MasterId,
                    MasterName = master.Name
                })
                .ToListAsync();
        }

        public async Task<ServicePortfolioDTO> GetServicePortfolioData(int servicePortfolioId)
        {
            return await this.pipContext.ServicePortfolio
                    .Where(servicePortfolio => servicePortfolio.ServicePortfolioId == servicePortfolioId)
                    .ProjectToType<ServicePortfolioDTO>()
                    .SingleOrDefaultAsync();
        }

        public async Task<List<ServicePortfolioDTO>> GetAllServicePortfolios()
        {
            return await this.pipContext.ServicePortfolio
                .ProjectToType<ServicePortfolioDTO>()
                .ToListAsync();
        }

        public async Task<List<ServiceLineDTO>> GetServiceLineByPortfolio(int servicePortfolioId)
        {
            return await this.pipContext.ServiceLine
                    .Where(serviceLine => serviceLine.ServicePortfolioId == servicePortfolioId)
                    .ProjectToType<ServiceLineDTO>()
                    .ToListAsync();
        }

        public async Task<List<ServiceLineDTO>> GetAllServiceLines()
        {
            return await this.pipContext.ServiceLine
                .ProjectToType<ServiceLineDTO>()
                .ToListAsync();
        }

        public async Task<List<ProjectBillingTypeDTO>> GetProjectBillingByDeliveryId(int projectDeliveryTypeId)
        {
            return await this.pipContext.ProjectBillingType
                    .Join(pipContext.DeliveryBilling, PB => PB.ProjectBillingTypeId, DB => DB.ProjectBillingTypeId, (PB, DB) => new { DB, PB })
                    .Join(pipContext.ProjectDeliveryType, PD => PD.DB.ProjectDeliveryTypeId, PDT => PDT.ProjectDeliveryTypeId, (PD, PDT) => new { PD, PDT })
                    .Where(projectBillingType => projectBillingType.PD.DB.ProjectDeliveryTypeId == projectDeliveryTypeId)
                    .Select(projectBillingType => new ProjectBillingTypeDTO
                    {
                        ProjectBillingTypeId = projectBillingType.PD.PB.ProjectBillingTypeId,
                        BillingTypeName = projectBillingType.PD.PB.BillingTypeName,
                        MasterVersionId = projectBillingType.PD.PB.MasterVersionId,
                        CreatedBy = projectBillingType.PD.PB.CreatedBy,
                        CreatedOn = projectBillingType.PD.PB.CreatedOn,
                        UpdatedBy = projectBillingType.PD.PB.UpdatedBy,
                        UpdatedOn = projectBillingType.PD.PB.UpdatedOn
                    }).ToListAsync();
        }

        public async Task<List<ProjectBillingTypeDTO>> GetAllProjectBillingData()
        {
            return await this.pipContext.ProjectBillingType
                .ProjectToType<ProjectBillingTypeDTO>()
                .ToListAsync();
        }

        public async Task<ProjectDeliveryTypeDTO> GetProjectDeliveryData(int projectDeliveryTypeId)
        {
            return await this.pipContext.ProjectDeliveryType
                    .Where(projectDeliveryType => projectDeliveryType.ProjectDeliveryTypeId == projectDeliveryTypeId)
                    .ProjectToType<ProjectDeliveryTypeDTO>()
                    .SingleOrDefaultAsync();
        }

        public async Task<List<ProjectDeliveryTypeDTO>> GetAllProjectDeliveryData()
        {
            return await this.pipContext.ProjectDeliveryType
                .ProjectToType<ProjectDeliveryTypeDTO>()
                .ToListAsync();
        }

        public async Task<List<ContractingEntityDTO>> GetAllContractingEntities()
        {
            return await this.pipContext.ContractingEntity
                .ProjectToType<ContractingEntityDTO>()
                .ToListAsync();
        }

        public async Task<List<LocationDTO>> GetLocations()
        {
            return await this.pipContext.Location
                .ProjectToType<LocationDTO>()
                .ToListAsync();
        }

        public async Task<List<LocationDTO>> GetLocations(int projectId, int pipSheetId)
        {
            var locationList = await this.pipContext.Location
                .Where(x => x.IsActive == true && x.EndDate == null)
                .OrderByDescending(x => x.IsFrequent).ThenBy(y => y.LocationName)
                .ProjectToType<LocationDTO>()
                .ToListAsync();
                
               
            int? sfBillingType = await GetSFBillingType(projectId, pipSheetId);
            if (sfBillingType == 3 || sfBillingType == 8)
            {
                foreach (var location in locationList)
                {
                    location.HoursPerDay = 0;
                }
            }
            else
            {
                foreach (var location in locationList)
                {
                    location.HoursPerMonth = 0;
                }
            }
            return locationList;
        }

        public async Task<int?> GetSFBillingType(int projectId, int pipSheetId)
        {
            ProjectHeaderDTO projectHeaderDTO = null;

            if (projectId != 0)
            {
                projectHeaderDTO = await (from project in this.pipContext.Project
                                          join pipSheet in this.pipContext.PipSheet on project.ProjectId equals pipSheet.ProjectId
                                          join projectHeader in this.pipContext.ProjectHeader on pipSheet.PipSheetId equals projectHeader.PipSheetId
                                          where project.ProjectId == projectId && pipSheet.PipSheetId == pipSheetId
                                          select new ProjectHeaderDTO
                                          {
                                              ProjectBillingTypeId = projectHeader.ProjectBillingType.ProjectBillingTypeId,
                                          }).SingleOrDefaultAsync();
            }
            else
            {
                return 0;
            }
            return projectHeaderDTO.ProjectBillingTypeId;
        }

        public async Task<string> GetLocationById(int locationId)
        {
            return await this.pipContext.Location
                    .Where(x => x.LocationId == locationId)
                    .Select(x => x.LocationName)
                    .SingleOrDefaultAsync();
        }

        public async Task<List<MilestoneGroupDTO>> GetMilestoneGroups()
        {
            return await this.pipContext.MilestoneGroup
                .ProjectToType<MilestoneGroupDTO>()
                .ToListAsync();
        }

        public async Task<List<MilestoneDTO>> GetMilestones()
        {
            return await
               (from milestone in this.pipContext.Milestone
                join milestoneGroup in this.pipContext.MilestoneGroup on milestone.MilestoneGroupId equals milestoneGroup.MilestoneGroupId
                select new MilestoneDTO
                {
                    MilestoneId = milestone.MilestoneId,
                    MasterVersionId = milestone.MasterVersionId,
                    MilestoneGroupId = milestone.MilestoneGroupId,
                    MilestoneGroupName = milestoneGroup.GroupName,
                    MilestoneName = milestone.Name
                }).ToListAsync();
        }

        public async Task<List<MarkupDTO>> GetMarkup()
        {
            return await this.pipContext.Markup
                .ProjectToType<MarkupDTO>()
                .ToListAsync();
        }

        public async Task<List<ResourceDTO>> GetResources()
        {
            return await this.pipContext.Resource
                    .ProjectToType<ResourceDTO>()
                    .ToListAsync();
        }

        public async Task<List<ResourceGroupDTO>> GetResourceGroups()
        {
            return await this.pipContext.ResourceGroup
                    .ProjectToType<ResourceGroupDTO>()
                    .ToListAsync();
        }

        public async Task<List<HolidayDTO>> GetHolidays()
        {
            return await this.pipContext.Holiday
                    .ProjectToType<HolidayDTO>()
                    .ToListAsync();
        }

        public async Task<List<CurrencyDTO>> GetCurrencyData()
        {
            return await this.pipContext.Currency
                .ProjectToType<CurrencyDTO>()
                .ToListAsync();
        }

        public async Task<List<CountryDTO>> GetCountries()
        {
            return await this.pipContext.Country
                .ProjectToType<CountryDTO>()
                .ToListAsync();
        }

        public async Task<List<StandardCostRateDTO>> GetStandardCostRate()
        {
            return await this.pipContext.StandardCostRate
                .ProjectToType<StandardCostRateDTO>()
                .ToListAsync();
        }

        public async Task<List<CorpBillingRateDTO>> GetCorpBillingRate()
        {
            return await this.pipContext.CorpBillingRate
                .ProjectToType<CorpBillingRateDTO>()
                .ToListAsync();
        }
    }
}
