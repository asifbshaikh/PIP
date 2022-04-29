using System;
namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ProjectHeaderDTO : BaseDTO
    {
        public int ProjectId { get; set; }
        public int PIPSheetId { get; set; }
        public string SfProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? AccountId { get; set; }
        public int? ContractingEntityId { get; set; }
        public bool? BeatTax { get; set; }
        public string DeliveryOwner { get; set; }
        public int? ServicePortfolioId { get; set; }
        public int? ServiceLineId { get; set; }
        public int? ProjectDeliveryTypeId { get; set; }
        public int? ProjectBillingTypeId { get; set; }
        public int? VersionNumber { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
        public int CurrencyId { get; set; }
        public string LastUpdatedBy { get; set; }
        public string PipsheetCreatedBy { get; set; }
        public string ApproverComments { get; set; }
        public int? ApproverStatusId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedOn { get; set; }
        public string ResendComments { get; set; }
        public string ResendBy { get; set; }
        public DateTime ResendOn { get; set; }
        public bool IsDummy { get; set; }
        public bool IsFromReplicate { get; set; }

    }
}
