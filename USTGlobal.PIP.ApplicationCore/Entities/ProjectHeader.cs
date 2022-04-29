using System;

namespace USTGlobal.PIP.ApplicationCore.Entities
{
    public class ProjectHeader : BaseEntity
    {
        public int PipSheetId { get; set; }
        public int? ContractingEntityID { get; set; }
        public string DeliveryOwner { get; set; }
        public int? ServicePortfolioID { get; set; }
        public int? ServiceLineID { get; set; }
        public int? ProjectDeliveryTypeID { get; set; }
        public int? ProjectBillingTypeID { get; set; }
        public int SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
        public ContractingEntity ContractingEntity { get; set; }
        public ServicePortfolio ServicePortfolio { get; set; }
        public ServiceLine ServiceLine { get; set; }
        public ProjectDeliveryType ProjectDeliveryType { get; set; }
        public ProjectBillingType ProjectBillingType { get; set; }        
    }
}
