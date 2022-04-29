using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class OtherPriceAdjustmentService : IOtherPriceAdjustmentService
    {
        private readonly IOtherPriceAdjustmentRepository otherPriceAdjustmentRepository;

        public OtherPriceAdjustmentService(IOtherPriceAdjustmentRepository otherPriceAdjustmentRepository)
        {
            this.otherPriceAdjustmentRepository = otherPriceAdjustmentRepository;
        }

        public async Task<OtherPriceAdjustmentMainDTO> GetOtherPriceAdjustment(int pipSheetId)
        {
            OtherPriceAdjustmentSubDTO otherPriceAdjustmentSubDTO = await this.otherPriceAdjustmentRepository.GetOtherPriceAdjustment(pipSheetId);
            OtherPriceAdjustmentMainDTO otherPriceAdjustmentMainDTO = new OtherPriceAdjustmentMainDTO();
            List<OtherPriceAdjustmentParentDTO> otherPriceAdjustmentParent = new List<OtherPriceAdjustmentParentDTO>();

            // Create Object for Fee Before Adjustment
            otherPriceAdjustmentParent.Add(GetFeeBeforeAdjustmentRow(otherPriceAdjustmentSubDTO.FeeBeforeAdjustment,
                otherPriceAdjustmentSubDTO.FeeBeforeAdjustmentPeriod, pipSheetId));

            foreach (OtherPriceAdjustmentDTO otherPriceAdjustmentDTO in otherPriceAdjustmentSubDTO.OtherPriceAdjustment)
            {
                otherPriceAdjustmentParent.Add(CreateOtherPriceAdjustmentObject(otherPriceAdjustmentDTO, otherPriceAdjustmentSubDTO));
            }

            otherPriceAdjustmentMainDTO.ProjectMilestone = otherPriceAdjustmentSubDTO.ProjectMilestone;
            otherPriceAdjustmentMainDTO.OtherPriceAdjustmentParent = otherPriceAdjustmentParent;
            otherPriceAdjustmentMainDTO.ProjectPeriod = otherPriceAdjustmentSubDTO.ProjectPeriod;
            otherPriceAdjustmentMainDTO.IsMonthlyFeeAdjustment = otherPriceAdjustmentSubDTO.IsMonthlyFeeAdjustment;

            return otherPriceAdjustmentMainDTO;
        }

        private OtherPriceAdjustmentParentDTO CreateOtherPriceAdjustmentObject(OtherPriceAdjustmentDTO otherPriceAdjustmentDTO
            , OtherPriceAdjustmentSubDTO otherPriceAdjustmentSubDTO)
        {
            OtherPriceAdjustmentParentDTO otherPriceAdjustment = new OtherPriceAdjustmentParentDTO();
            otherPriceAdjustment.OtherPriceAdjustmentPeriodDetail = new List<OtherPriceAdjustmentPeriodDetailDTO>();

            otherPriceAdjustment.OtherPriceAdjustmentId = otherPriceAdjustmentDTO.OtherPriceAdjustmentId;
            otherPriceAdjustment.PipSheetId = otherPriceAdjustmentDTO.PipSheetId;
            otherPriceAdjustment.MilestoneId = otherPriceAdjustmentDTO.MilestoneId;
            otherPriceAdjustment.RowType = Constants.OPAEditableFields;
            otherPriceAdjustment.Description = otherPriceAdjustmentDTO.Description;
            otherPriceAdjustment.TotalRevenue = otherPriceAdjustmentDTO.TotalRevenue;
            otherPriceAdjustment.OtherPriceAdjustmentPeriodDetail = otherPriceAdjustmentSubDTO.OtherPriceAdjustmentPeriodDetail
                .Where(otherPriceAdjustmentPeriodDetailDTO => otherPriceAdjustmentPeriodDetailDTO.OtherPriceAdjustmentId == otherPriceAdjustmentDTO.OtherPriceAdjustmentId)
                .Select(otherPriceAdjustmentPeriodDetailDTO => otherPriceAdjustmentPeriodDetailDTO).ToList();
            otherPriceAdjustment.CreatedBy = otherPriceAdjustmentDTO.CreatedBy;
            otherPriceAdjustment.UpdatedBy = otherPriceAdjustmentDTO.UpdatedBy;
            otherPriceAdjustment.CreatedOn = otherPriceAdjustmentDTO.CreatedOn;
            otherPriceAdjustment.UpdatedOn = otherPriceAdjustmentDTO.UpdatedOn;

            return otherPriceAdjustment;
        }

        private OtherPriceAdjustmentParentDTO GetFeeBeforeAdjustmentRow(FixBidCalcDTO feeBeforeAdjustmentDTO
            , IList<FixBidCalcPeriodDTO> feeBeforeAdjustmentPeriod, int pipSheetId)
        {
            OtherPriceAdjustmentParentDTO feeBeforeAdjustment = new OtherPriceAdjustmentParentDTO();
            feeBeforeAdjustment.OtherPriceAdjustmentPeriodDetail = new List<OtherPriceAdjustmentPeriodDetailDTO>();

            feeBeforeAdjustment.OtherPriceAdjustmentId = 0;
            feeBeforeAdjustment.PipSheetId = pipSheetId;
            feeBeforeAdjustment.MilestoneId = null;
            feeBeforeAdjustment.RowType = Constants.OPAFeeBeforeAdjustment;
            feeBeforeAdjustment.Description = "Fee Before Adjustment";
            feeBeforeAdjustment.TotalRevenue = feeBeforeAdjustmentDTO.TotalCost;
            feeBeforeAdjustment.OtherPriceAdjustmentPeriodDetail = (from period in feeBeforeAdjustmentPeriod
                                                                    select new OtherPriceAdjustmentPeriodDetailDTO
                                                                    {
                                                                        BillingPeriodId = period.BillingPeriodId,
                                                                        Revenue = period.Cost
                                                                    }).ToList();
            feeBeforeAdjustment.CreatedBy = 0;
            feeBeforeAdjustment.UpdatedBy = 0;
            feeBeforeAdjustment.CreatedOn = new DateTime();
            feeBeforeAdjustment.UpdatedOn = new DateTime();

            return feeBeforeAdjustment;
        }

        public async Task SaveOtherPriceAdjustmentData(string userName, OtherPriceAdjustmentMainDTO otherPriceAdjustmentMainDTO)
        {
            otherPriceAdjustmentMainDTO = ReAssignUIds(otherPriceAdjustmentMainDTO);

            IList<OtherPriceAdjustmentDTO> otherPriceAdjustmentDTO = otherPriceAdjustmentMainDTO.OtherPriceAdjustmentParent
                .Where(opa => opa.RowType == Constants.OPAEditableFields)
                .Select(otherPriceAdjustment => new OtherPriceAdjustmentDTO
                {
                    UId = otherPriceAdjustment.UId,
                    OtherPriceAdjustmentId = otherPriceAdjustment.OtherPriceAdjustmentId,
                    PipSheetId = otherPriceAdjustment.PipSheetId,
                    MilestoneId = otherPriceAdjustment.MilestoneId == -1 ? null : otherPriceAdjustment.MilestoneId,
                    Description = otherPriceAdjustment.Description,
                    TotalRevenue = otherPriceAdjustment.TotalRevenue,
                    IsDeleted = otherPriceAdjustment.IsDeleted,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                }).ToList();

            IList<OtherPriceAdjustmentPeriodDetailDTO> otherPriceAdjustmentPeriodDetailDTO = otherPriceAdjustmentMainDTO.OtherPriceAdjustmentParent
                .SelectMany(otherPriceAdjustmentPeriodDetail => otherPriceAdjustmentPeriodDetail.OtherPriceAdjustmentPeriodDetail).ToList();

            // Calculate Adjustment Entry, Fee After Adjustment and Adjusted Revenue
            IList<OtherPriceAdjustmentPeriodTotalDTO> otherPriceAdjustmentPeriodTotalDTO = CalculateAllFields(otherPriceAdjustmentMainDTO);

            await this.otherPriceAdjustmentRepository.SaveOtherPriceAdjustmentData(userName, otherPriceAdjustmentDTO, otherPriceAdjustmentPeriodDetailDTO,
                otherPriceAdjustmentPeriodTotalDTO, otherPriceAdjustmentMainDTO.IsMonthlyFeeAdjustment);
        }

        /// <summary>
        /// Calculate OtherPriceAdjustment Period Totals
        /// </summary>
        /// <param name="otherPriceAdjustmentPeriodDetailDTO"></param>
        /// <param name="pipSheetId"></param>
        /// <returns></returns>
        private IList<OtherPriceAdjustmentPeriodTotalDTO> CalculateOtherPriceAdjustmentPeriodTotals(IList<OtherPriceAdjustmentPeriodDetailDTO> otherPriceAdjustmentPeriodDetail,
            int pipSheetId)
        {
            IList<OtherPriceAdjustmentPeriodTotalDTO> otherPriceAdjustmentPeriodTotal = new List<OtherPriceAdjustmentPeriodTotalDTO>();
            List<int> uniqueBillingPeriodId = (from opapd in otherPriceAdjustmentPeriodDetail select opapd.BillingPeriodId).Distinct().ToList();
            List<OtherPriceAdjustmentPeriodDetailDTO> obj = null;

            IDictionary<int, IList<OtherPriceAdjustmentPeriodDetailDTO>> dict = new Dictionary<int, IList<OtherPriceAdjustmentPeriodDetailDTO>>();

            foreach (var bpId in uniqueBillingPeriodId)
            {
                obj = new List<OtherPriceAdjustmentPeriodDetailDTO>();
                obj.AddRange(otherPriceAdjustmentPeriodDetail.Where(singleItem => singleItem.BillingPeriodId == bpId));
                dict.Add(bpId, obj);
            }

            foreach (KeyValuePair<int, IList<OtherPriceAdjustmentPeriodDetailDTO>> entry in dict)
            {
                decimal? revenue = 0;
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    revenue += entry.Value[i].Revenue == null ? 0 : entry.Value[i].Revenue;
                }

                OtherPriceAdjustmentPeriodTotalDTO ppt = new OtherPriceAdjustmentPeriodTotalDTO()
                {
                    BillingPeriodId = entry.Key,
                    PipSheetId = pipSheetId,
                    AdjustedRevenue = revenue
                };
                otherPriceAdjustmentPeriodTotal.Add(ppt);
            }
            return otherPriceAdjustmentPeriodTotal;
        }

        public OtherPriceAdjustmentMainDTO ReAssignUIds(OtherPriceAdjustmentMainDTO otherPriceAdjustment)
        {
            // Re-Assigning UIds to Other Price Adjustment object
            for (int i = 0; i < otherPriceAdjustment.OtherPriceAdjustmentParent.Count; i++)
            {
                otherPriceAdjustment.OtherPriceAdjustmentParent[i].UId = i + 1;
                foreach (var opa in otherPriceAdjustment.OtherPriceAdjustmentParent[i].OtherPriceAdjustmentPeriodDetail)
                {
                    opa.UId = i + 1;
                }
            }
            return otherPriceAdjustment;
        }

        private IList<OtherPriceAdjustmentPeriodTotalDTO> CalculateAllFields(OtherPriceAdjustmentMainDTO otherPriceAdjustment)
        {
            int pipSheetId = otherPriceAdjustment.OtherPriceAdjustmentParent[0].PipSheetId;
            IList<OtherPriceAdjustmentPeriodTotalDTO> opaPeriodTotal = new List<OtherPriceAdjustmentPeriodTotalDTO>();
            OtherPriceAdjustmentParentDTO setMonthlyFee = new OtherPriceAdjustmentParentDTO();
            OtherPriceAdjustmentParentDTO feeBeforeAdjustment = otherPriceAdjustment.OtherPriceAdjustmentParent.Where(x => x.RowType == Constants.OPAFeeBeforeAdjustment).SingleOrDefault();
            if (otherPriceAdjustment.IsMonthlyFeeAdjustment)
            {
                setMonthlyFee = otherPriceAdjustment.OtherPriceAdjustmentParent.Where(x => x.RowType == Constants.OPAEditableFields).SingleOrDefault();
            }
            IList<OtherPriceAdjustmentParentDTO> editableFields = otherPriceAdjustment.OtherPriceAdjustmentParent
                 .Where(x => x.RowType == Constants.OPAEditableFields && x.IsDeleted == false).ToList();

            IList<OtherPriceAdjustmentPeriodDetailDTO> editableFieldsPeriods = editableFields
                .SelectMany(otherPriceAdjustmentPeriodDetail => otherPriceAdjustmentPeriodDetail.OtherPriceAdjustmentPeriodDetail).ToList();

            IList<OtherPriceAdjustmentPeriodTotalDTO> editableFieldsPeriodTotals = CalculateOtherPriceAdjustmentPeriodTotals(editableFieldsPeriods, pipSheetId);

            for (int i = 0; i < otherPriceAdjustment.OtherPriceAdjustmentParent[0].OtherPriceAdjustmentPeriodDetail.Count; i++)
            {
                OtherPriceAdjustmentPeriodTotalDTO periodTotal = new OtherPriceAdjustmentPeriodTotalDTO();
                periodTotal.BillingPeriodId = otherPriceAdjustment.OtherPriceAdjustmentParent[0].OtherPriceAdjustmentPeriodDetail[i].BillingPeriodId;
                periodTotal.PipSheetId = otherPriceAdjustment.OtherPriceAdjustmentParent[0].PipSheetId;
                if (otherPriceAdjustment.IsMonthlyFeeAdjustment)
                {
                    periodTotal.PriceAdjustmentEntry = (setMonthlyFee.OtherPriceAdjustmentPeriodDetail[i].Revenue ?? 0) - (feeBeforeAdjustment.OtherPriceAdjustmentPeriodDetail[i].Revenue ?? 0);
                    periodTotal.FeeAfterAdjustment = (setMonthlyFee.OtherPriceAdjustmentPeriodDetail[i].Revenue ?? 0);
                    periodTotal.AdjustedRevenue = periodTotal.PriceAdjustmentEntry;
                }
                else
                {
                    decimal? adjustedRevenue = editableFieldsPeriodTotals.Count > 1 ? editableFieldsPeriodTotals[i].AdjustedRevenue : 0;
                    periodTotal.PriceAdjustmentEntry = 0;
                    periodTotal.AdjustedRevenue = adjustedRevenue;
                    periodTotal.FeeAfterAdjustment = adjustedRevenue + (feeBeforeAdjustment.OtherPriceAdjustmentPeriodDetail[i].Revenue ?? 0);
                }
                opaPeriodTotal.Add(periodTotal);
            }

            return opaPeriodTotal;
        }
    }
}
