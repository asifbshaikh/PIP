using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Entities;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class PartnerCostAndRevenueService : IPartnerCostAndRevenueService
    {
        private readonly IPartnerCostAndRevenueRepository partnerCostAndRevenueRepository;

        public PartnerCostAndRevenueService(IPartnerCostAndRevenueRepository partnerCostAndRevenueRepository)
        {
            this.partnerCostAndRevenueRepository = partnerCostAndRevenueRepository;
        }

        public async Task<PartnerCostAndRevenueDTO> GetPartnerCostAndRevenue(int pipSheetId)
        {
            PartnerCostAndRevenueParentDTO partnerCostAndRevenueParentDTO = await this.partnerCostAndRevenueRepository.GetPartnerCostAndRevenue(pipSheetId);
            PartnerCostAndRevenueDTO partnerCostAndRevenue = new PartnerCostAndRevenueDTO();
            List<PartnerCostParentDTO> partnerCostDTOList = new List<PartnerCostParentDTO>();
            List<PartnerRevenueParentDTO> partnerRevenueDTOList = new List<PartnerRevenueParentDTO>();

            foreach (PartnerCostDTO partnerCostDTO in partnerCostAndRevenueParentDTO.PartnerCostDTO)
            {
                PartnerCostParentDTO partnerCost = new PartnerCostParentDTO();
                partnerCost.PartnerCostPeriodDetail = new List<PartnerCostPeriodDetailDTO>();

                partnerCost.PartnerCostId = partnerCostDTO.PartnerCostId;
                partnerCost.PipSheetId = partnerCostDTO.PipSheetId;
                partnerCost.MilestoneId = partnerCostDTO.MilestoneId;
                partnerCost.Description = partnerCostDTO.Description;
                partnerCost.PaidAmount = partnerCostDTO.PaidAmount;
                partnerCost.SetMargin = partnerCostDTO.SetMargin;
                partnerCost.MarginPercent = partnerCostDTO.MarginPercent;

                partnerCost.PartnerCostPeriodDetail = partnerCostAndRevenueParentDTO.PartnerCostPeriodDetailDTO.Where(partnerCostPeriodDetailDTO => partnerCostPeriodDetailDTO.PartnerCostId ==
                      partnerCostDTO.PartnerCostId).Select(partnerCostPeriodDetailDTO => partnerCostPeriodDetailDTO).ToList();
                partnerCost.CreatedBy = partnerCostDTO.CreatedBy;
                partnerCost.UpdatedBy = partnerCostDTO.UpdatedBy;
                partnerCostDTOList.Add(partnerCost);
            }

            foreach (PartnerRevenueParentDTO partnerRevenueDTO in partnerCostAndRevenueParentDTO.PartnerRevenueDTO)
            {
                PartnerRevenueParentDTO partnerRevenue = new PartnerRevenueParentDTO();
                partnerRevenue.PartnerRevenuePeriodDetail = new List<PartnerRevenuePeriodDetailDTO>();

                partnerRevenue.PartnerRevenueId = partnerRevenueDTO.PartnerRevenueId;
                partnerRevenue.PartnerCostUId = partnerRevenueDTO.PartnerCostUId;
                partnerRevenue.PipSheetId = partnerRevenueDTO.PipSheetId;
                partnerRevenue.MilestoneId = partnerRevenueDTO.MilestoneId;
                partnerRevenue.Description = partnerRevenueDTO.Description;
                partnerRevenue.RevenueAmount = partnerRevenueDTO.RevenueAmount;
                partnerRevenue.SetMargin = partnerRevenueDTO.SetMargin;
                partnerRevenue.MarginPercent = partnerRevenueDTO.MarginPercent;

                partnerRevenue.PartnerRevenuePeriodDetail = partnerCostAndRevenueParentDTO.PartnerRevenuePeriodDetailDTO.Where(partnerRevenuePeriodDetailDTO => partnerRevenuePeriodDetailDTO.PartnerRevenueId ==
                      partnerRevenueDTO.PartnerRevenueId).Select(partnerCostPeriodDetailDTO => partnerCostPeriodDetailDTO).ToList();
                partnerRevenue.CreatedBy = partnerRevenueDTO.CreatedBy;
                partnerRevenue.UpdatedBy = partnerRevenueDTO.UpdatedBy;
                partnerRevenueDTOList.Add(partnerRevenue);
            }
            partnerCostAndRevenue.ProjectMilestone = partnerCostAndRevenueParentDTO.ProjectMilestoneDTO;
            partnerCostAndRevenue.PartnerCost = partnerCostDTOList;
            partnerCostAndRevenue.PartnerRevenue = partnerRevenueDTOList;
            partnerCostAndRevenue.projectPeriod = partnerCostAndRevenueParentDTO.projectPeriodDTO;

            return partnerCostAndRevenue;
        }

        public async Task SavePartnerCostAndRevenueData(string userName, PartnerCostAndRevenueDTO partnerCostAndRevenueDTO)
        {
            CalculateMonthWisePartnerRevenueForCopiedRows(partnerCostAndRevenueDTO);

            IList<PartnerCostDTO> partnerCostDTO = partnerCostAndRevenueDTO.PartnerCost.Select(partnerCost => new PartnerCostDTO
            {
                UId = partnerCost.UId,
                PartnerCostId = partnerCost.PartnerCostId,
                PipSheetId = partnerCost.PipSheetId,
                MilestoneId = partnerCost.MilestoneId == -1 ? null : partnerCost.MilestoneId,
                Description = partnerCost.Description,
                PaidAmount = partnerCost.PaidAmount,
                SetMargin = partnerCost.SetMargin,
                MarginPercent = partnerCost.MarginPercent,
                IsDeleted = partnerCost.IsDeleted,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            }).ToList();

            IList<PartnerRevenueDTO> partnerRevenueDTO = partnerCostAndRevenueDTO.PartnerRevenue.Select(partnerRevenue => new PartnerRevenueDTO
            {
                UId = partnerRevenue.UId,
                PartnerRevenueId = partnerRevenue.PartnerRevenueId,
                PartnerCostUId = partnerRevenue.PartnerCostUId,
                PipSheetId = partnerRevenue.PipSheetId,
                MilestoneId = partnerRevenue.MilestoneId == -1 ? null : partnerRevenue.MilestoneId,
                Description = partnerRevenue.Description,
                RevenueAmount = partnerRevenue.RevenueAmount,
                SetMargin = partnerRevenue.SetMargin,
                MarginPercent = partnerRevenue.MarginPercent,
                IsDeleted = partnerRevenue.IsDeleted,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            }).ToList();

            IList<PartnerCostPeriodDetailDTO> partnerCostPeriodDTO = partnerCostAndRevenueDTO.PartnerCost
                .SelectMany(partnerCostPeriodDetail => partnerCostPeriodDetail.PartnerCostPeriodDetail).ToList();

            IList<PartnerRevenuePeriodDetailDTO> partnerRevenuePeriodDTO = partnerCostAndRevenueDTO.PartnerRevenue
               .SelectMany(partnerRevenuePeriodDetail => partnerRevenuePeriodDetail.PartnerRevenuePeriodDetail).ToList();

            IList<PartnerCostPeriodTotalDTO> partnerCostPeriodTotalDTO = CalculatePartnerCostPeriodTotals(partnerCostPeriodDTO, partnerCostAndRevenueDTO.PartnerCost[0].PipSheetId);
            IList<PartnerRevenuePeriodTotalDTO> partnerRevenuePeriodTotalDTO = CalculatePartnerRevenuePeriodTotals(partnerRevenuePeriodDTO, partnerCostAndRevenueDTO.PartnerRevenue[0].PipSheetId);

            await this.partnerCostAndRevenueRepository.SavePartnerCostAndRevenueData(userName, partnerCostDTO, partnerCostPeriodDTO, partnerRevenueDTO, partnerRevenuePeriodDTO, partnerCostPeriodTotalDTO,
                partnerRevenuePeriodTotalDTO);
        }

        /// <summary>
        /// Function to calculate partner cost period total column wise
        /// </summary>
        /// <param name="partnerCostPeriod"></param>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        public IList<PartnerCostPeriodTotalDTO> CalculatePartnerCostPeriodTotals(IList<PartnerCostPeriodDetailDTO> partnerCostPeriod, int pipSheetId)
        {
            IList<PartnerCostPeriodTotalDTO> partnerCostPeriodTotals = new List<PartnerCostPeriodTotalDTO>();
            List<int> uniqueBillingPeriodId = (from pcp in partnerCostPeriod select pcp.BillingPeriodId).Distinct().ToList();
            List<PartnerCostPeriodDetailDTO> obj = null;

            IDictionary<int, IList<PartnerCostPeriodDetailDTO>> dict = new Dictionary<int, IList<PartnerCostPeriodDetailDTO>>();

            foreach (var bpId in uniqueBillingPeriodId)
            {
                obj = new List<PartnerCostPeriodDetailDTO>();
                obj.AddRange(partnerCostPeriod.Where(singleItem => singleItem.BillingPeriodId == bpId));
                dict.Add(bpId, obj);
            }

            foreach (KeyValuePair<int, IList<PartnerCostPeriodDetailDTO>> entry in dict)
            {
                decimal? cost = 0;
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    cost += entry.Value[i].Cost == null ? 0 : entry.Value[i].Cost;
                }

                PartnerCostPeriodTotalDTO ppt = new PartnerCostPeriodTotalDTO()
                {
                    BillingPeriodId = entry.Key,
                    PipSheetId = pipSheetId,
                    PartnerCost = cost
                };
                partnerCostPeriodTotals.Add(ppt);
            }
            return partnerCostPeriodTotals;
        }

        /// <summary>
        /// Function to calculate partner revenue period total column wise
        /// </summary>
        /// <param name="partnerRevenuePeriod"></param>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        public IList<PartnerRevenuePeriodTotalDTO> CalculatePartnerRevenuePeriodTotals(IList<PartnerRevenuePeriodDetailDTO> partnerRevenuePeriod, int pipSheetId)
        {
            IList<PartnerRevenuePeriodTotalDTO> partnerRevenuePeriodTotals = new List<PartnerRevenuePeriodTotalDTO>();
            List<int> uniqueBillingPeriodId = (from pcp in partnerRevenuePeriod select pcp.BillingPeriodId).Distinct().ToList();
            List<PartnerRevenuePeriodDetailDTO> obj = null;

            IDictionary<int, IList<PartnerRevenuePeriodDetailDTO>> dict = new Dictionary<int, IList<PartnerRevenuePeriodDetailDTO>>();

            foreach (var bpId in uniqueBillingPeriodId)
            {
                obj = new List<PartnerRevenuePeriodDetailDTO>();
                obj.AddRange(partnerRevenuePeriod.Where(singleItem => singleItem.BillingPeriodId == bpId));
                dict.Add(bpId, obj);
            }

            foreach (KeyValuePair<int, IList<PartnerRevenuePeriodDetailDTO>> entry in dict)
            {
                decimal? revenue = 0;
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    revenue += entry.Value[i].Revenue == null ? 0 : entry.Value[i].Revenue;
                }

                PartnerRevenuePeriodTotalDTO ppt = new PartnerRevenuePeriodTotalDTO()
                {
                    BillingPeriodId = entry.Key,
                    PipSheetId = pipSheetId,
                    PartnerRevenue = revenue
                };
                partnerRevenuePeriodTotals.Add(ppt);
            }
            return partnerRevenuePeriodTotals;
        }

        public PartnerCostAndRevenueDTO ReAssignUIds(PartnerCostAndRevenueDTO partnerCostAndRevenue)
        {
            // Re-Assigning UIds to Partner Cost object
            for (int i = 0; i < partnerCostAndRevenue.PartnerCost.Count; i++)
            {
                partnerCostAndRevenue.PartnerCost[i].UId = i + 1;
                foreach (var p in partnerCostAndRevenue.PartnerCost[i].PartnerCostPeriodDetail)
                {
                    p.UId = i + 1;
                }
            }

            // Re-Assigning UIds to Partner Revenue object
            for (int i = 0; i < partnerCostAndRevenue.PartnerRevenue.Count; i++)
            {
                partnerCostAndRevenue.PartnerRevenue[i].UId = i + 1;
                foreach (var p in partnerCostAndRevenue.PartnerRevenue[i].PartnerRevenuePeriodDetail)
                {
                    p.UId = i + 1;
                }
            }
            return partnerCostAndRevenue;
        }

        private void CalculateMonthWisePartnerRevenueForCopiedRows(PartnerCostAndRevenueDTO partnerCostAndRevenueDTO)
        {
            var filteredPartnerCostRows = partnerCostAndRevenueDTO.PartnerCost.Where(row => row.SetMargin == true).ToList();

            if (filteredPartnerCostRows.Count > 0)
            {
                foreach (PartnerCostParentDTO item in filteredPartnerCostRows)
                {
                    decimal marginPercent = item.MarginPercent ?? 0;
                    if (marginPercent > 0)
                    {
                        marginPercent = marginPercent / 100;
                        for (int i = 0; i < item.PartnerCostPeriodDetail.Count; i++)
                        {
                            decimal? cost = item.PartnerCostPeriodDetail[i].Cost;
                            decimal denominator = (1 - marginPercent);
                            if (cost != null && cost > 0)
                            {
                                if (denominator != 0)
                                {
                                    PartnerRevenueParentDTO revenueRow = partnerCostAndRevenueDTO.PartnerRevenue.Find(row => row.PartnerCostUId == item.UId);
                                    revenueRow.PartnerRevenuePeriodDetail[i].Revenue = (cost / (1 - marginPercent));
                                }                     

                            }
                        }
                    }
                }
            }


        }
    }
}
