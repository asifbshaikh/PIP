using System.Collections.Generic;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class SharedDataDTO
    {
        //public UserDTO UserDTO { get; set; }
        //public IList<RoleDTO> RoleDTO { get; set; }
        public int? CurrencyId { get; set; }
        public UserRoleAccountDTO UserRoleAccountDTO { get; set; }
        public IList<ServicePortfolioDTO> ServicePortfolioDTO { get; set; }
        public IList<ServiceLineDTO> ServiceLineDTO { get; set; }
        public IList<ProjectDeliveryTypeDTO> ProjectDeliveryTypeDTO { get; set; }
        public IList<ProjectBillingTypeDTO> ProjectBillingTypeDTO { get; set; }
        public IList<ContractingEntityDTO> ContractingEntityDTO { get; set; }
        public IList<LocationDTO> LocationDTO { get; set; }
        public IList<MarkupDTO> MarkupDTO { get; set; }
        public IList<ResourceDTO> ResourceDTO { get; set; }
        public IList<ResourceGroupDTO> ResourceGroupDTO { get; set; }
        public IList<HolidayDTO> HolidayDTO { get; set; }
        public IList<MilestoneDTO> MilestoneDTO { get; set; }
        public IList<MilestoneGroupDTO> MilestoneGroupDTO { get; set; }
        public IList<CurrencyDTO> CurrencyDTO { get; set; }
        public IList<CountryDTO> CountryDTO { get; set; }
        public IList<StandardCostRateDTO> StandardCostRateDTO { get; set; }
        public IList<CorpBillingRateDTO> CorpBillingRateDTO { get; set; }
        public IList<BasicAssetDTO> BasicAssetDTO { get; set; }
        public IList<DefaultLabelDTO> DefaultLabelDTO { get; set; }
        public IList<ProjectDeliveryBillingTypeDTO> ProjectDeliveryBillingTypeDTO { get; set; }
        public IList<BillingYearDTO> BillingYearDTO { get; set; }
        public IList<NonBillableCategoryDTO> NonBillableCategoryDTO { get; set; }
        public IList<AccountDTO> AccountDTO { get; set; }
        public IList<RoleDTO> RoleDTO { get; set; }
        public IList<ResourceServiceLineDTO> ResourceServiceLineDTO { get; set; }
        public IList<PipSheetWorkflowStatus> PipSheetWorkflowStatus { get; set; }
        public bool HasAccountLevelAccess { get; set; }
        public bool HasSharePipAccess { get; set; }
        public bool HasDummyPipAccess { get; set; }
        public bool HasAccountLevelEditorAccess { get; set; }
        public bool HasFinanceApproverAccess { get; set; }
    }
}
