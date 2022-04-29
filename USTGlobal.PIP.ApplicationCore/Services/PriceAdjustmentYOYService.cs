using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using System.Linq;
using USTGlobal.PIP.ApplicationCore.Helpers;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class PriceAdjustmentYoyService : IPriceAdjustmentYoyService
    {
        
        private readonly IPriceAdjustmentYoyRepository priceAdjustmentYoyRepository;

        public PriceAdjustmentYoyService(IPriceAdjustmentYoyRepository priceAdjustmentYoyRepository)
        {
            this.priceAdjustmentYoyRepository = priceAdjustmentYoyRepository;
        }
        public async Task<PriceAdjustmentDTO> GetPriceAdjustmentYoy(int pipSheetId)
        {
            return await this.priceAdjustmentYoyRepository.GetPriceAdjustmentYoy(pipSheetId);
        }

        public async Task SavePriceAdjustmentYoy(string userName, PriceAdjustmentDTO priceAdjustmentDTO)
        {
            IList<PriceAdjustmentResourceDTO> priceAdjustmentResourceDTO = await ReCalculateLaborPricing(priceAdjustmentDTO);
            IList<ProjectResourcePeriodDTO> projectPeriods = priceAdjustmentResourceDTO.SelectMany(x => x.projectResourcePeriodDTO).ToList();
            IList<ResourceLaborPricingSubDTO> resourceLaborPricingSubDTO = priceAdjustmentResourceDTO.Select(x => new ResourceLaborPricingSubDTO
            {
                ProjectResourceId = x.ProjectResourceId,
                PipSheetId = x.PipSheetId,
                UtilizationType = x.UtilizationType,
                CappedCost = x.CappedCost,
                Cost = x.Cost,
                Margin = x.Margin,
                Rate = x.Rate,
                RatePerHour = x.RatePerHour,
                TotalRevenue = x.TotalRevenue,
                Yr1PerHour = x.Yr1PerHour,
            }).ToList();

            IList<ProjectPeriodTotalDTO> projectPeriodTotals = CalculatePeriodRevenueTotals(projectPeriods, priceAdjustmentDTO.PriceAdjustmentYoyDTO.PipSheetId);
            await this.priceAdjustmentYoyRepository.SavePriceAdjustmentYoy(userName, priceAdjustmentDTO, projectPeriods, resourceLaborPricingSubDTO, projectPeriodTotals);
        }

        private async Task<IList<PriceAdjustmentResourceDTO>> ReCalculateLaborPricing(PriceAdjustmentDTO priceAdjustmentDTO)
        {
            //From Database
            IList<PriceAdjustmentResourceDTO> laborPricingData = await
                priceAdjustmentYoyRepository.GetLaborPricingDataForPriceAdjustment(priceAdjustmentDTO.PriceAdjustmentYoyDTO.PipSheetId);

            //From Price Adjustment Screen
            ProjectDurationDTO projectDurationDTO = priceAdjustmentDTO.ProjectDurationDTO;
            PriceAdjustmentYoyDTO priceAdjustmentYoyDTO = priceAdjustmentDTO.PriceAdjustmentYoyDTO;
            IList<PriceAdjustmentResourceDTO> calculatedLaborPricingData = new List<PriceAdjustmentResourceDTO>();
            int noOfPeriods = DateHelper.GetMonthsBetween(projectDurationDTO.StartDate, projectDurationDTO.EndDate);

            //Iterating over a list of resources
            if (laborPricingData != null)
            {
                for (int res = 0; res < laborPricingData.Count; res++)
                {
                    calculatedLaborPricingData.Add(laborPricingData[res]);
                    var periodDetails = laborPricingData[res].projectResourcePeriodDTO;
                    DateTime effectiveDate = priceAdjustmentYoyDTO.EffectiveDate;
                    decimal colaPercentPerLocation = GetColaPercent(laborPricingData[res].LocationId, priceAdjustmentDTO.ColaDTO) / 100;
                    if (laborPricingData[res].UtilizationType)
                    {
                        //Iterating over number of periods
                        for (int i = 0; i < noOfPeriods; i++)
                        {
                            if (periodDetails[i].FTEValue != 0)
                            {
                                decimal? revenuePerFTE = 0;
                                decimal? billRate = 0;
                                DateTime tmpStartDate = projectDurationDTO.StartDate.AddMonths(i);
                                revenuePerFTE = periodDetails[i].Revenue / periodDetails[i].FTEValue;
                                billRate = periodDetails[i].BillRate;

                                // Iterating with tmpStartDate until it is greater than Price Adjustment Effective Date 
                                while (tmpStartDate >= effectiveDate)
                                {
                                    billRate += colaPercentPerLocation * billRate;
                                    revenuePerFTE += colaPercentPerLocation * revenuePerFTE;
                                    tmpStartDate = tmpStartDate.AddYears(-1);
                                }
                                calculatedLaborPricingData[res].projectResourcePeriodDTO[i].PriceAdjustment = (revenuePerFTE * periodDetails[i].FTEValue) - calculatedLaborPricingData[res].projectResourcePeriodDTO[i].Revenue;
                                calculatedLaborPricingData[res].projectResourcePeriodDTO[i].BillRate = billRate;
                            }
                        }
                        calculatedLaborPricingData[res].TotalRevenue = (from resource in calculatedLaborPricingData[res].projectResourcePeriodDTO select (resource.Revenue + resource.PriceAdjustment) ?? 0).Sum();
                        if (calculatedLaborPricingData[res].TotalRevenue != 0)
                        {
                            calculatedLaborPricingData[res].Margin = ((calculatedLaborPricingData[res].TotalRevenue - calculatedLaborPricingData[res].CappedCost) / calculatedLaborPricingData[res].TotalRevenue) * 100;
                        }

                    }
                }
            }
            else
            {
                calculatedLaborPricingData = null;
            }

            return calculatedLaborPricingData;
        }

        private decimal GetColaPercent(int locationId, IList<ColaDTO> colaDTO)
        {
            return Convert.ToDecimal((from cola in colaDTO where cola.LocationId == locationId select cola.COLAPercent).FirstOrDefault());
        }

        private IList<ProjectPeriodTotalDTO> CalculatePeriodRevenueTotals(IList<ProjectResourcePeriodDTO> projectPeriods, int pipSheetId)
        {
            IList<ProjectPeriodTotalDTO> projectPeriodTotals = new List<ProjectPeriodTotalDTO>();
            List<int> uniqueBillingPeriodId = (from pp in projectPeriods select pp.BillingPeriodId).Distinct().ToList();
            List<ProjectResourcePeriodDTO> obj = null;

            IDictionary<int, IList<ProjectResourcePeriodDTO>> dict = new Dictionary<int, IList<ProjectResourcePeriodDTO>>();

            foreach (var bpId in uniqueBillingPeriodId)
            {
                obj = new List<ProjectResourcePeriodDTO>();
                obj.AddRange(projectPeriods.Where(singleItem => singleItem.BillingPeriodId == bpId));
                dict.Add(bpId, obj);
            }

            foreach (KeyValuePair<int, IList<ProjectResourcePeriodDTO>> entry in dict)
            {
                decimal? monthlyRevenue = 0;
                decimal? priceAdjustment = 0;
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    monthlyRevenue += entry.Value[i].Revenue ?? 0;
                    priceAdjustment += entry.Value[i].PriceAdjustment ?? 0;
                }
                ProjectPeriodTotalDTO ppt = new ProjectPeriodTotalDTO()
                {
                    BillingPeriodId = entry.Key,
                    PipSheetId = pipSheetId,
                    Revenue = Convert.ToDecimal(monthlyRevenue + priceAdjustment)
                };
                projectPeriodTotals.Add(ppt);
            }
            return projectPeriodTotals;
        }
    }
}
