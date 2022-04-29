using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class RiskManagementService : IRiskManagementService
    {
        private readonly IRiskManagementRepository riskManagementRepository;
        private readonly ISharedRepository sharedRepository;

        public RiskManagementService(IRiskManagementRepository riskManagementRepository,
           ISharedRepository sharedRepository)
        {
            this.riskManagementRepository = riskManagementRepository;
            this.sharedRepository = sharedRepository;
        }
        public async Task<RiskManagementCalcDTO> GetRiskManagement(int pipSheetId)
        {
            RiskManagementParentCalcDTO riskManagementParentCalcDTO = await this.riskManagementRepository.GetRiskManagement(pipSheetId);
            RiskManagementCalcDTO riskManagementCalcDTO = new RiskManagementCalcDTO();
            riskManagementCalcDTO.riskManagement = new RiskManagementParentDTO();
            riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail = new List<RiskManagementPeriodDetailDTO>();
            riskManagementCalcDTO.CalculatedValue = riskManagementParentCalcDTO.CalculatedValue == null ? new CalculatedValueDTO() : riskManagementParentCalcDTO.CalculatedValue;
            riskManagementCalcDTO.ProjectDeliveryTypeId = riskManagementParentCalcDTO.ProjectDeliveryTypeId;

            if (riskManagementParentCalcDTO.RiskManagement != null)
            {
                riskManagementCalcDTO.riskManagement.RiskManagementId = riskManagementParentCalcDTO.RiskManagement.RiskManagementId;
                riskManagementCalcDTO.riskManagement.PipSheetId = riskManagementParentCalcDTO.RiskManagement.PipSheetId;
                riskManagementCalcDTO.riskManagement.IsContingencyPercent = riskManagementParentCalcDTO.RiskManagement.IsContingencyPercent;
                riskManagementCalcDTO.riskManagement.CostContingencyRisk = riskManagementParentCalcDTO.RiskManagement.CostContingencyRisk;
                riskManagementCalcDTO.riskManagement.FeesAtRisk = riskManagementParentCalcDTO.RiskManagement.FeesAtRisk;
                riskManagementCalcDTO.riskManagement.IsFixedBid = riskManagementParentCalcDTO.RiskManagement.IsFixedBid;
                riskManagementCalcDTO.riskManagement.FixBidRiskAmount = riskManagementParentCalcDTO.RiskManagement.FixBidRiskAmount;
                riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun = riskManagementParentCalcDTO.RiskManagement.TotalAssesedRiskOverrun;
                riskManagementCalcDTO.riskManagement.IsOverride = riskManagementParentCalcDTO.RiskManagement.IsOverride;
                riskManagementCalcDTO.riskManagement.CostContingencyPercent = riskManagementParentCalcDTO.RiskManagement.CostContingencyPercent;
                riskManagementCalcDTO.riskManagement.RiskCostSubTotal = riskManagementParentCalcDTO.RiskManagement.RiskCostSubTotal;
                riskManagementCalcDTO.riskManagement.CreatedBy = riskManagementParentCalcDTO.RiskManagement.CreatedBy;
                riskManagementCalcDTO.riskManagement.UpdatedBy = riskManagementParentCalcDTO.RiskManagement.UpdatedBy;
                riskManagementCalcDTO.riskManagement.ProjectDeliveryTypeID = riskManagementParentCalcDTO.RiskManagement.ProjectDeliveryTypeID;
                riskManagementCalcDTO.riskManagement.IsMarginSet = riskManagementParentCalcDTO.RiskManagement.IsMarginSet;
                riskManagementCalcDTO.riskManagement.IsOverrideUpdated = riskManagementParentCalcDTO.RiskManagement.IsOverrideUpdated;

                riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail = riskManagementParentCalcDTO.RiskManagementPeriodDetail;
            }
            riskManagementCalcDTO.projectPeriod = riskManagementParentCalcDTO.projectPeriod;

            return riskManagementCalcDTO;
        }

        public async Task SaveRiskManagement(string userName, RiskManagementCalcDTO riskManagementCalcDTO)
        {
            RiskManagementDTO riskManagementDTO = new RiskManagementDTO();
            IList<RiskManagementPeriodDetailDTO> riskManagementPeriodDetailDTO = new List<RiskManagementPeriodDetailDTO>();

            CalculatedValueDTO calculatedValueDTO = riskManagementCalcDTO.CalculatedValue;
            riskManagementDTO.RiskManagementId = riskManagementCalcDTO.riskManagement.RiskManagementId;
            riskManagementDTO.PipSheetId = riskManagementCalcDTO.riskManagement.PipSheetId;
            riskManagementDTO.IsContingencyPercent = riskManagementCalcDTO.riskManagement.IsContingencyPercent;
            riskManagementDTO.CostContingencyRisk = riskManagementCalcDTO.riskManagement.CostContingencyRisk;
            riskManagementDTO.FeesAtRisk = riskManagementCalcDTO.riskManagement.FeesAtRisk;
            riskManagementDTO.IsFixedBid = riskManagementCalcDTO.riskManagement.IsFixedBid;
            riskManagementDTO.FixBidRiskAmount = riskManagementCalcDTO.riskManagement.FixBidRiskAmount;
            riskManagementDTO.TotalAssesedRiskOverrun = riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun;
            riskManagementDTO.IsOverride = riskManagementCalcDTO.riskManagement.IsOverride;
            riskManagementDTO.CostContingencyPercent = riskManagementCalcDTO.riskManagement.CostContingencyPercent;
            riskManagementDTO.CreatedOn = DateTime.Now;
            riskManagementDTO.UpdatedOn = DateTime.Now;
            riskManagementDTO.ProjectDeliveryTypeID = riskManagementCalcDTO.riskManagement.ProjectDeliveryTypeID;
            riskManagementDTO.IsOverrideUpdated = riskManagementCalcDTO.riskManagement.IsOverrideUpdated;

            foreach (RiskManagementPeriodDetailDTO riskManagementPeriodDTO in riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail)
            {
                RiskManagementPeriodDetailDTO riskManagementPeriod = new RiskManagementPeriodDetailDTO();
                riskManagementPeriod.RiskManagementId = riskManagementPeriodDTO.RiskManagementId;
                riskManagementPeriod.BillingPeriodId = riskManagementPeriodDTO.BillingPeriodId;
                riskManagementPeriod.RiskAmount = riskManagementPeriodDTO.RiskAmount;
                riskManagementPeriod.CreatedOn = DateTime.Now;
                riskManagementPeriod.UpdatedOn = DateTime.Now;
                riskManagementPeriodDetailDTO.Add(riskManagementPeriod);
            }
            await this.riskManagementRepository.SaveRiskManagement(userName, calculatedValueDTO, riskManagementDTO, riskManagementPeriodDetailDTO);
            //await this.fixBidAndMarginRepository.CalculateAndSaveFixBidData(riskManagementCalcDTO.riskManagement.PipSheetId, userName);
        }

        public async Task<RiskManagementCalcDTO> CalculateRiskManagementData(int pipSheetId, string userName)
        {
            decimal? fixedBidPercent = 10;
            RiskManagementCalcDTO riskManagementCalcDTO = await GetRiskManagement(pipSheetId);
            decimal? costSubTotal = riskManagementCalcDTO.riskManagement.RiskCostSubTotal; //M155
            if (riskManagementCalcDTO.riskManagement != null && riskManagementCalcDTO.riskManagement.RiskManagementId > 0)
            {
                bool isContingencyPercent = riskManagementCalcDTO.riskManagement.IsContingencyPercent ?? false;
                if (isContingencyPercent)
                {
                    //if (isContingencyPercent)
                    //{
                    //    riskManagementCalcDTO.riskManagement.CostContingencyPercent = Math.Ceiling(Math.Abs((riskManagementCalcDTO.riskManagement.CostContingencyRisk * 100 ?? 0) / riskManagementCalcDTO.riskManagement.RiskCostSubTotal ?? 0));
                    //}

                    if (riskManagementCalcDTO.riskManagement.ProjectDeliveryTypeID != 5)
                    {
                        riskManagementCalcDTO.riskManagement.IsFixedBid = false;
                        riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun = Math.Abs(((riskManagementCalcDTO.riskManagement.CostContingencyPercent / 100) * riskManagementCalcDTO.riskManagement.RiskCostSubTotal) ?? 0);
                        riskManagementCalcDTO.riskManagement.CostContingencyRisk = Math.Abs(riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun ?? 0);
                        riskManagementCalcDTO.riskManagement.FixBidRiskAmount = 0;
                    }
                    else
                    {
                        riskManagementCalcDTO.riskManagement.IsFixedBid = true;
                        riskManagementCalcDTO.riskManagement.FixBidRiskAmount = Math.Abs(((fixedBidPercent / 100) * costSubTotal) ?? 0); //H156
                        riskManagementCalcDTO.riskManagement.CostContingencyRisk = Math.Abs(((riskManagementCalcDTO.riskManagement.CostContingencyPercent / 100) * costSubTotal) ?? 0); //H157
                        riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun = Math.Round((riskManagementCalcDTO.riskManagement.FixBidRiskAmount ?? 0) + (riskManagementCalcDTO.riskManagement.CostContingencyRisk ?? 0), 2);

                    }
                }
                else
                {
                    if (riskManagementCalcDTO.riskManagement.ProjectDeliveryTypeID != 5) // Fixed Bid (Milestone based)
                    {
                        riskManagementCalcDTO.riskManagement.IsFixedBid = false;
                        riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun = Math.Abs(riskManagementCalcDTO.riskManagement.CostContingencyRisk ?? 0);
                        riskManagementCalcDTO.riskManagement.FixBidRiskAmount = 0;
                    }
                    else
                    {
                        riskManagementCalcDTO.riskManagement.IsFixedBid = true;
                        decimal? h156 = Math.Abs(((fixedBidPercent / 100) * costSubTotal) ?? 0);
                        riskManagementCalcDTO.riskManagement.FixBidRiskAmount = h156;
                        riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun = Math.Abs((h156 ?? 0) + (riskManagementCalcDTO.riskManagement.CostContingencyRisk ?? 0));
                    }
                }

                //Calculate Period-wise Risk
                decimal? riskAmount = 0;
                foreach (var riskManagementPeriod in riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail)
                {
                    riskAmount += riskManagementPeriod.RiskAmount ?? 0;
                }
                if (riskManagementCalcDTO.riskManagement.IsOverride)
                {
                    if (Math.Abs(Math.Round(riskAmount ?? 0, 2) - Math.Round(riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun ?? 0, 2)) > 1)
                    {
                        riskManagementCalcDTO.riskManagement.IsOverrideUpdated = true;
                    }
                    else
                    {
                        riskManagementCalcDTO.riskManagement.IsOverrideUpdated = false;
                    }
                }
                else
                {
                    riskManagementCalcDTO.riskManagement.IsOverrideUpdated = false;
                    for (int i = 0; i < riskManagementCalcDTO.projectPeriod.Count; i++)
                    {
                        if ((riskManagementCalcDTO.CalculatedValue.TotalCappedCost ?? 0) != 0)
                        {
                            riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail[i].RiskAmount = Math.Round((riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun *
                                (riskManagementCalcDTO.projectPeriod[i].CappedCost / (riskManagementCalcDTO.CalculatedValue.TotalCappedCost == 0 ? 1 : riskManagementCalcDTO.CalculatedValue.TotalCappedCost))) ?? 0, 2);
                        }
                        else
                        {
                            riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail[i].RiskAmount = Math.Round(((riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun ?? 0) / riskManagementCalcDTO.projectPeriod.Count), 2);
                        }
                    }
                }

                // await SaveRiskManagement(userName, riskManagementCalcDTO);
            }
            else
            {
                // Execution will enter here from Resource Planning First Time Save Click
                riskManagementCalcDTO.riskManagement.PipSheetId = pipSheetId;
                riskManagementCalcDTO.riskManagement.CostContingencyPercent = null;
                riskManagementCalcDTO.riskManagement.CostContingencyRisk = null;
                riskManagementCalcDTO.riskManagement.IsContingencyPercent = false;
                riskManagementCalcDTO.riskManagement.FeesAtRisk = null;
                riskManagementCalcDTO.riskManagement.IsOverride = false;
                riskManagementCalcDTO.riskManagement.IsMarginSet = false;
                riskManagementCalcDTO.riskManagement.ProjectDeliveryTypeID = riskManagementCalcDTO.ProjectDeliveryTypeId;


                if (riskManagementCalcDTO.ProjectDeliveryTypeId != 5)
                {
                    // Other than Fixed Bid 
                    riskManagementCalcDTO.riskManagement.IsFixedBid = false;
                    riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun = null;
                    riskManagementCalcDTO.riskManagement.FixBidRiskAmount = 0;
                    riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail = new List<RiskManagementPeriodDetailDTO>();

                    //Calculate Period-wise Risk
                    for (int i = 0; i < riskManagementCalcDTO.projectPeriod.Count; i++)
                    {
                        RiskManagementPeriodDetailDTO riskManagementPeriod = new RiskManagementPeriodDetailDTO()
                        {
                            BillingPeriodId = riskManagementCalcDTO.projectPeriod[i].BillingPeriodId,
                            RiskAmount = 0,
                            CreatedOn = new DateTime(),
                            UpdatedOn = new DateTime()
                        };
                        riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail.Add(riskManagementPeriod);
                    }
                }
                else
                {
                    // Fixed Bid
                    riskManagementCalcDTO.riskManagement.IsFixedBid = true;
                    riskManagementCalcDTO.riskManagement.FixBidRiskAmount = Math.Abs(((fixedBidPercent / 100) * riskManagementCalcDTO.riskManagement.RiskCostSubTotal) ?? 0);
                    riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun = riskManagementCalcDTO.riskManagement.FixBidRiskAmount;

                    //Calculate Period-wise Fixed Bid Risk Amount
                    for (int i = 0; i < riskManagementCalcDTO.projectPeriod.Count; i++)
                    {
                        RiskManagementPeriodDetailDTO riskManagementPeriod = new RiskManagementPeriodDetailDTO()
                        {
                            BillingPeriodId = riskManagementCalcDTO.projectPeriod[i].BillingPeriodId,
                            RiskAmount = Math.Round((riskManagementCalcDTO.riskManagement.TotalAssesedRiskOverrun * (riskManagementCalcDTO.projectPeriod[i].CappedCost /
                            (riskManagementCalcDTO.CalculatedValue.TotalCappedCost == 0 ? 1 : riskManagementCalcDTO.CalculatedValue.TotalCappedCost))) ?? 0, 2),
                            CreatedOn = new DateTime(),
                            UpdatedOn = new DateTime()
                        };
                        riskManagementCalcDTO.riskManagement.RiskManagementPeriodDetail.Add(riskManagementPeriod);
                    }
                }
            }

            return riskManagementCalcDTO;
        }
    }
}
