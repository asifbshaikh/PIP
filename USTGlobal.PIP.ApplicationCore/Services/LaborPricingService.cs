using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class LaborPricingService : ILaborPricingService
    {
        private readonly ILaborPricingRepository laborPricingRepository;
        private readonly IUserRepository userRepository;

        public LaborPricingService(ILaborPricingRepository laborPricingRepository, IUserRepository userRepository)
        {
            this.laborPricingRepository = laborPricingRepository;
            this.userRepository = userRepository;
        }

        public async Task<LaborPricingDTO> GetLaborPricingData(int pipSheetId)
        {
            return await this.laborPricingRepository.GetLaborPricingData(pipSheetId);
        }

        public async Task SaveLaborPricingData(string userName, LaborPricingDTO laborPricingDTO)
        {
            IList<ProjectPeriodTotalDTO> projectPeriodTotals = new List<ProjectPeriodTotalDTO>();
            if (laborPricingDTO.resourceLaborPricingDTOs.Count > 0)
            {
                laborPricingDTO = await CalculateResourceWiseSeatCost(laborPricingDTO, laborPricingDTO.resourceLaborPricingDTOs[0].PipSheetId);
            }
            IList<ProjectResourcePeriodDTO> projectPeriods = laborPricingDTO.resourceLaborPricingDTOs.SelectMany(x => x.projectResourcePeriodDTO).ToList();
            IList<ResourceLaborPricingSubDTO> resourceLaborPricingSubDTO = laborPricingDTO.resourceLaborPricingDTOs.Select(x => new ResourceLaborPricingSubDTO
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
                NonBillableCategoryId = (x.NonBillableCategoryId == -1 ? null : x.NonBillableCategoryId),
                ResourceSeatCost = x.ResourceSeatCost
            }).ToList();
            if (resourceLaborPricingSubDTO.Count > 0)
            {
                projectPeriodTotals = CalculatePeriodRevenueTotals(projectPeriods, resourceLaborPricingSubDTO[0].PipSheetId);
            }
            await this.laborPricingRepository.SaveLaborPricingData(userName, laborPricingDTO, projectPeriods, resourceLaborPricingSubDTO, projectPeriodTotals);
        }

        //This API will be called three times :  
        //1) ngOnInit on LaborPricing page, 
        //2) "Which" Dropdown change on LaborPricing Page, 
        //3) When MarginPercent is entered
        //4) When Cost is overriden
        public async Task<LaborPricingBackgroundCalculationDTO> CalculateBackgroundFields(int pipSheetId, bool isMarginSet, int which, decimal marginPercent, bool isInitLoad,
            decimal inflatedCappedCost, decimal totalInflation)
        {
            LaborPricingBackgroundCalculationDTO laborPricingBackgroundCalculationDTO = new LaborPricingBackgroundCalculationDTO();
            decimal G14;
            decimal[] ebidtaAndStdOverhead;
            LaborPricingBackgroundCalcParentDTO laborPricingBackgroundCalcParentDTO = await laborPricingRepository.CalculateBackgroundFields(pipSheetId);

            //isInitLoad flag added for ngOnInit in Angular Call (First API Call)
            if (laborPricingBackgroundCalcParentDTO != null && laborPricingBackgroundCalcParentDTO.LaborPricingEbidtaCalculationDTO.Count > 0)
            {
                if (isInitLoad)
                {
                    if (laborPricingBackgroundCalcParentDTO.MarginDTO == null)
                    {
                        //All background fields are 0 when first time user arrives on this page, so there will not be any entry in Margin table for this pipSheetId
                        laborPricingBackgroundCalculationDTO.G13 = 0;
                        laborPricingBackgroundCalculationDTO.G14 = 0;
                        laborPricingBackgroundCalculationDTO.G15 = 0;
                        laborPricingBackgroundCalculationDTO.G16 = 0;
                        return laborPricingBackgroundCalculationDTO;
                    }
                    else
                    {
                        //Execution will arrive here during PageLoad when an existing LaborPricing saved page is opened.
                        ebidtaAndStdOverhead = CalculateEbidtaSeatCostAndStdOverhead(laborPricingBackgroundCalcParentDTO.ResourcePlanningPipSheetDTO[0],
                                        laborPricingBackgroundCalcParentDTO.LaborPricingEbidtaCalculationDTO,
                                        laborPricingBackgroundCalcParentDTO.HolidayList);

                        // Irrespective if Fixed Bid, TotalAssesedRiskOverrun to be added in G14
                        if (laborPricingBackgroundCalcParentDTO.LaborPricingEbidtaCalculationDTO[0].Which == 1)
                        {
                            G14 = ebidtaAndStdOverhead[0] + ebidtaAndStdOverhead[1];
                        }
                        else
                        {
                            G14 = ebidtaAndStdOverhead[1];
                        }
                        G14 = G14 + laborPricingBackgroundCalcParentDTO.TotalAssesedRiskOverrun;

                        laborPricingBackgroundCalculationDTO.G13 = Convert.ToDecimal(laborPricingBackgroundCalcParentDTO.LaborPricingBackgroundCalculationDTO.G13)
                            * (laborPricingBackgroundCalcParentDTO.ProjectPeriodTotalDTO.CappedCost
                            / ((laborPricingBackgroundCalcParentDTO.ProjectPeriodTotalDTO.CappedCost - laborPricingBackgroundCalcParentDTO.ProjectPeriodTotalDTO.Inflation) == 0 ? 1 :
                            (laborPricingBackgroundCalcParentDTO.ProjectPeriodTotalDTO.CappedCost - laborPricingBackgroundCalcParentDTO.ProjectPeriodTotalDTO.Inflation)));
                        laborPricingBackgroundCalculationDTO.G14 = G14;
                        laborPricingBackgroundCalculationDTO.G15 = Convert.ToDecimal(laborPricingBackgroundCalcParentDTO.LaborPricingBackgroundCalculationDTO.G15);

                        // If "Set Target for" from DB is "Price", when G16 = 1, else (1 - Fees At Risk Percent)
                        if (laborPricingBackgroundCalcParentDTO.LaborPricingEbidtaCalculationDTO[0].Which == 3)
                        {
                            laborPricingBackgroundCalculationDTO.G16 = 1;
                        }
                        else
                        {
                            laborPricingBackgroundCalculationDTO.G16 = 1 - Convert.ToDecimal((laborPricingBackgroundCalcParentDTO.FeesAtRisk == 1 ? 0 : laborPricingBackgroundCalcParentDTO.FeesAtRisk));
                        }
                        return laborPricingBackgroundCalculationDTO;
                    }
                }
                else
                {
                    //Execution will enter this when second or third API call is received i.e. When "Which" dropdown is changed or MarginPercent is entered.
                    if (isMarginSet)
                    {
                        ebidtaAndStdOverhead = CalculateEbidtaSeatCostAndStdOverhead(laborPricingBackgroundCalcParentDTO.ResourcePlanningPipSheetDTO[0],
                                        laborPricingBackgroundCalcParentDTO.LaborPricingEbidtaCalculationDTO,
                                        laborPricingBackgroundCalcParentDTO.HolidayList);

                        // Irrespective if Fixed Bid, TotalAssesedRiskOverrun to be added in G14
                        if (which == 1)
                        {
                            G14 = ebidtaAndStdOverhead[0] + ebidtaAndStdOverhead[1];
                        }
                        else
                        {
                            G14 = ebidtaAndStdOverhead[1];
                        }
                        G14 = G14 + CalculateTotalAssessedRiskOverrun(laborPricingBackgroundCalcParentDTO, inflatedCappedCost);

                        laborPricingBackgroundCalculationDTO.G13 = Convert.ToDecimal(laborPricingBackgroundCalcParentDTO.LaborPricingBackgroundCalculationDTO.G13)
                            * (inflatedCappedCost / ((inflatedCappedCost - totalInflation) == 0 ? 1 : (inflatedCappedCost - totalInflation)));
                        laborPricingBackgroundCalculationDTO.G14 = G14;
                        laborPricingBackgroundCalculationDTO.G15 = 1 - (marginPercent / 100) -
                                (laborPricingBackgroundCalcParentDTO.PaymentLag > 30 ? ((laborPricingBackgroundCalcParentDTO.PaymentLag - 30) * (decimal)(0.025 / 100)) : 0);     //G15 re-calculated

                        // If "Set Target for" from screen is "Price", when G16 = 1, else (1 - Fees At Risk Percent)
                        if (which == 3)
                        {
                            laborPricingBackgroundCalculationDTO.G16 = 1;
                        }
                        else
                        {
                            laborPricingBackgroundCalculationDTO.G16 = 1 - Convert.ToDecimal((laborPricingBackgroundCalcParentDTO.FeesAtRisk == 1 ? 0 : laborPricingBackgroundCalcParentDTO.FeesAtRisk));
                        }
                        return laborPricingBackgroundCalculationDTO;
                    }
                    else
                    {
                        //All background fields are 0 when isMarginSet Flag = false
                        laborPricingBackgroundCalculationDTO.G13 = 0;
                        laborPricingBackgroundCalculationDTO.G14 = 0;
                        laborPricingBackgroundCalculationDTO.G15 = 0;
                        laborPricingBackgroundCalculationDTO.G16 = 0;
                        return laborPricingBackgroundCalculationDTO;
                    }
                }
            }
            else
            {
                //Whenever there is resource present in Resource Planning
                laborPricingBackgroundCalculationDTO.G13 = 0;
                laborPricingBackgroundCalculationDTO.G14 = 0;
                laborPricingBackgroundCalculationDTO.G15 = 0;
                laborPricingBackgroundCalculationDTO.G16 = 0;
                return laborPricingBackgroundCalculationDTO;
            }
        }

        public decimal[] CalculateEbidtaSeatCostAndStdOverhead(ResourcePlanningPipSheetDTO resourcePlanningPipSheetDTO,
            IList<LaborPricingEbidtaCalculationDTO> laborPricingEbidtaCalculationDTO, IList<HolidayDTO> holidayList)
        {
            decimal[] ebidtaAndStdOverhead = { 0, 0 };
            DateTime startDate = resourcePlanningPipSheetDTO.StartDate;
            DateTime endDate = resourcePlanningPipSheetDTO.EndDate;
            bool holidayOption = resourcePlanningPipSheetDTO.HolidayOption;

            DateTime startDateFirstDayOfMonth = DateHelper.GetFirstDayOfMonth(startDate);
            DateTime startDateLastDayOfMonth = DateHelper.GetLastDayOfMonth(startDate);
            DateTime endDateFirstDayOfMonth = DateHelper.GetFirstDayOfMonth(endDate);
            DateTime endDateLastDayOfMonth = DateHelper.GetLastDayOfMonth(endDate);

            int noOfPeriods = DateHelper.GetMonthsBetween(startDate, endDate);

            List<int> uniqueProjResId = (from l in laborPricingEbidtaCalculationDTO select l.ProjectResourceId).Distinct().ToList();
            List<LaborPricingEbidtaCalculationDTO> obj = null;

            IDictionary<int, IList<LaborPricingEbidtaCalculationDTO>> dict = new Dictionary<int, IList<LaborPricingEbidtaCalculationDTO>>();

            foreach (var resId in uniqueProjResId)
            {
                obj = new List<LaborPricingEbidtaCalculationDTO>();
                obj.AddRange(laborPricingEbidtaCalculationDTO.Where(singleItem => singleItem.ProjectResourceId == resId));
                dict.Add(resId, obj);
            }

            foreach (KeyValuePair<int, IList<LaborPricingEbidtaCalculationDTO>> entry in dict)
            {
                for (int i = 0; i < entry.Value.Count; i++)
                //foreach (var value in entry.Value)
                {
                    if (i != 0 && i != noOfPeriods - 1)
                    {
                        ebidtaAndStdOverhead[0] = ebidtaAndStdOverhead[0] + entry.Value[i].EbitdaSeatCost * entry.Value[i].FTEValue;
                        ebidtaAndStdOverhead[1] = ebidtaAndStdOverhead[1] + entry.Value[i].OverheadAmount * entry.Value[i].FTEValue;
                    }
                    else if (i == 0)                //First Period
                    {
                        if (holidayOption)
                        {
                            int daysWorked = 0;
                            if (startDate.Month == endDate.Month && startDate.Year == endDate.Year)
                            {
                                daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, endDate)
                                            - DateHelper.GetHolidaysCount(startDate, startDateLastDayOfMonth, entry.Value[i].LocationId, holidayList);
                            }
                            else
                            {
                                daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, startDateLastDayOfMonth)
                                            - DateHelper.GetHolidaysCount(startDate, startDateLastDayOfMonth, entry.Value[i].LocationId, holidayList);
                            }

                            int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(startDateFirstDayOfMonth, startDateLastDayOfMonth)
                                            - DateHelper.GetHolidaysCount(startDateFirstDayOfMonth, startDateLastDayOfMonth, entry.Value[i].LocationId, holidayList);

                            decimal ebidtaSeatCostPerDay = entry.Value[i].EbitdaSeatCost / noOfWorkingDays;
                            decimal stdOverheadPerDay = entry.Value[i].OverheadAmount / noOfWorkingDays;

                            ebidtaAndStdOverhead[0] = ebidtaAndStdOverhead[0] + ebidtaSeatCostPerDay * daysWorked * entry.Value[i].FTEValue;
                            ebidtaAndStdOverhead[1] = ebidtaAndStdOverhead[1] + stdOverheadPerDay * daysWorked * entry.Value[i].FTEValue;
                        }
                        else
                        {
                            int daysWorked = 0;
                            if (startDate.Month == endDate.Month && startDate.Year == endDate.Year)
                            {
                                daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, endDate);
                            }
                            else
                            {
                                daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, startDateLastDayOfMonth);
                            }
                            int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(startDateFirstDayOfMonth, startDateLastDayOfMonth);
                            decimal ebidtaSeatCostPerDay = entry.Value[i].EbitdaSeatCost / noOfWorkingDays;
                            decimal stdOverheadPerDay = entry.Value[i].OverheadAmount / noOfWorkingDays;
                            ebidtaAndStdOverhead[0] = ebidtaAndStdOverhead[0] + ebidtaSeatCostPerDay * daysWorked * entry.Value[i].FTEValue;
                            ebidtaAndStdOverhead[1] = ebidtaAndStdOverhead[1] + stdOverheadPerDay * daysWorked * entry.Value[i].FTEValue;
                        }
                    }
                    else                            //Last Period
                    {
                        if (holidayOption)
                        {
                            int daysWorked = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDate)
                                            - DateHelper.GetHolidaysCount(endDateFirstDayOfMonth, endDate, entry.Value[i].LocationId, holidayList);
                            int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDateLastDayOfMonth)
                                            - DateHelper.GetHolidaysCount(endDateFirstDayOfMonth, endDateLastDayOfMonth, entry.Value[i].LocationId, holidayList);

                            decimal ebidtaSeatCostPerDay = entry.Value[i].EbitdaSeatCost / noOfWorkingDays;
                            decimal stdOverheadPerDay = entry.Value[i].OverheadAmount / noOfWorkingDays;
                            ebidtaAndStdOverhead[0] = ebidtaAndStdOverhead[0] + ebidtaSeatCostPerDay * daysWorked * entry.Value[i].FTEValue;
                            ebidtaAndStdOverhead[1] = ebidtaAndStdOverhead[1] + stdOverheadPerDay * daysWorked * entry.Value[i].FTEValue;
                        }
                        else
                        {
                            int daysWorked = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDate);
                            int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDateLastDayOfMonth);

                            decimal ebidtaSeatCostPerDay = entry.Value[i].EbitdaSeatCost / noOfWorkingDays;
                            decimal stdOverheadPerDay = entry.Value[i].OverheadAmount / noOfWorkingDays;

                            ebidtaAndStdOverhead[0] = ebidtaAndStdOverhead[0] + ebidtaSeatCostPerDay * daysWorked * entry.Value[i].FTEValue;
                            ebidtaAndStdOverhead[1] = ebidtaAndStdOverhead[1] + stdOverheadPerDay * daysWorked * entry.Value[i].FTEValue;
                        }
                    }
                }
            }
            return ebidtaAndStdOverhead;
        }

        public IList<ProjectPeriodTotalDTO> CalculatePeriodRevenueTotals(IList<ProjectResourcePeriodDTO> projectPeriods, int pipSheetId)
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
                    monthlyRevenue += entry.Value[i].Revenue;
                    priceAdjustment += entry.Value[i].PriceAdjustment;
                }
                priceAdjustment = priceAdjustment == null ? 0 : priceAdjustment;
                ProjectPeriodTotalDTO ppt = new ProjectPeriodTotalDTO()
                {
                    BillingPeriodId = entry.Key,
                    PipSheetId = pipSheetId,
                    Revenue = Convert.ToDecimal(monthlyRevenue + priceAdjustment),
                    Inflation = (from inflation in entry.Value select inflation.Inflation).Sum(),
                    CappedCost = (from cappedCost in entry.Value select cappedCost.CappedCost ?? 0).Sum()

                };
                projectPeriodTotals.Add(ppt);
            }
            return projectPeriodTotals;
        }

        public decimal CalculateTotalAssessedRiskOverrun(LaborPricingBackgroundCalcParentDTO backgroundCalculation, decimal cappedCost)
        {
            decimal totalAssessedRiskOverrun = 0;
            decimal fixedBidPercent = 10;
            decimal? costSubTotal = backgroundCalculation.TotalDirectExpense + backgroundCalculation.TotalPartnerCost + cappedCost;
            if (backgroundCalculation.RiskManagementDTO != null && backgroundCalculation.RiskManagementDTO.RiskManagementId > 0)
            {
                // Add Fixed Bid Amount in Overrun
                if (backgroundCalculation.RiskManagementDTO.IsFixedBid)
                {
                    totalAssessedRiskOverrun = Math.Abs(Math.Round(((fixedBidPercent / 100) * costSubTotal ?? 0), 2, MidpointRounding.AwayFromZero));
                }

                if ((backgroundCalculation.RiskManagementDTO.IsContingencyPercent ?? false) == true)
                {
                    totalAssessedRiskOverrun += Math.Abs(Math.Round(((backgroundCalculation.RiskManagementDTO.CostContingencyPercent ?? 0) / 100) * (costSubTotal ?? 0), 2, MidpointRounding.AwayFromZero));
                }
                else
                {
                    totalAssessedRiskOverrun += Math.Abs((backgroundCalculation.RiskManagementDTO.CostContingencyRisk ?? 0));
                }
            }
            return totalAssessedRiskOverrun;
        }

        //   Save dependency implementations

        public SharedDataDTO sharedData { get; set; }

        public bool isDeliveryTypeRestricted
        {
            get
            {
                return this.laborPricingData.isDeliveryTypeRestricted;
            }
        }

        public bool isinflationApplicable { get; set; }

        public LaborPricingDTO laborPricingData { get; set; }

        public List<InflationDetails> InflationDetailsPerResource { get; set; }

        public decimal? inflationPerResource { get; set; }

        public decimal? inflatedCappedCost { get; set; }


        public async Task<LaborPricingDTO> CalculateLaborPricing(string emailId, int pipSheetId)
        {
            this.laborPricingData = await this.laborPricingRepository.GetLaborPricingData(pipSheetId);
            this.sharedData = await this.userRepository.GetSharedData(emailId, pipSheetId);

            if (this.laborPricingData.marginDTO == null)
            {
                this.laborPricingData.marginDTO = new MarginDTO()
                {
                    PipSheetId = pipSheetId,
                    IsMarginSet = false,
                    MarginId = 0,
                    MarginPercent = 0,
                    Which = 0,
                };
            }

            if (laborPricingData != null)
            {
                this.isinflationApplicable = this.CheckInflation();

                for (int index = 0; index < laborPricingData.resourceLaborPricingDTOs.Count; index++)
                {
                    await this.calculateRatePerHour(index, pipSheetId, emailId);
                    await this.calculateCostPerHr(index);
                }
                return this.laborPricingData;

            }

            return this.laborPricingData;
        }


        private async Task calculateRatePerHour(int resourceIndex, int pipSheetId, string emailId)
        {
            bool isResourceBillable = this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].UtilizationType;
            bool isMarginSet = this.laborPricingData.marginDTO == null ? false : this.laborPricingData.marginDTO.IsMarginSet;
            decimal? year1cost;
            decimal corpBillingRate = 0;
            LaborPricingBackgroundCalculationDTO backgroundCalculations = null;
            decimal staffHours = 0;

            if (!this.isDeliveryTypeRestricted && isResourceBillable)
            {
                decimal? computedRatePerHour = 0;
                decimal? cost = this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].Cost;
                decimal? rate = this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].Rate;

                if (isMarginSet)
                {
                    staffHours = this.calculateStaffHours(this.laborPricingData.resourceLaborPricingDTOs);
                    year1cost = (cost != null && cost >= 0) ? cost : this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].Yr1PerHour;


                    // BackgroundCalculations
                    backgroundCalculations = await this.CalculateBackgroundFields(pipSheetId, isMarginSet, this.laborPricingData.marginDTO.Which,
                    this.laborPricingData.marginDTO.MarginPercent ?? 0, true, 1, 1);  // change /  check this this 

                    computedRatePerHour = ((((year1cost * backgroundCalculations.G13) + backgroundCalculations.G14) / backgroundCalculations.G15)
                        / (staffHours == 0 ? 1 : staffHours)) / backgroundCalculations.G16;

                    this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].RatePerHour = computedRatePerHour;

                    for (int i = 0; i < this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO.Count; i++)
                    {
                        this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO[i].BillRate = computedRatePerHour;
                    }
                }
                else
                {
                    if (rate != null && rate >= 0)
                    {
                        // computedRatePerHour = rate;  // since rate is overridden no need to assign it to computed rate per hour.
                        for (int i = 0; i < this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO.Count; i++)
                        {
                            this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO[i].BillRate = rate;
                        }

                        // In case of Applied Rate overriden, when Set Margin = OFF, then Standard rate should always show master value (Bug 3673 Fix)
                        corpBillingRate = this.sharedData.CorpBillingRateDTO.FirstOrDefault(br =>
                        br.LocationId == this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].LocationId &&
                        br.ResourceId == this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].ResourceId).Rate ?? 0;
                        this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].RatePerHour = corpBillingRate;

                    }
                    else
                    {
                        corpBillingRate = this.sharedData.CorpBillingRateDTO.FirstOrDefault(br =>
                        br.LocationId == this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].LocationId &&
                        br.ResourceId == this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].ResourceId).Rate ?? 0;

                        //apply inflation
                        computedRatePerHour = this.isinflationApplicable ? this.applyInflationForRatePerHour(this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].LocationId, corpBillingRate) : corpBillingRate;
                        this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].RatePerHour = computedRatePerHour;

                        for (int i = 0; i < this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO.Count; i++)
                        {
                            this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO[i].BillRate = computedRatePerHour;
                        }
                    }
                }

                this.calculateMonthlyRevenue(this.laborPricingData.resourceLaborPricingDTOs[resourceIndex]);
                this.calculateTotalRevenue(this.laborPricingData.resourceLaborPricingDTOs[resourceIndex]);
            }
            else
            {
                this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].Rate = null;
                this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].RatePerHour = 0;
                this.calculateMonthlyRevenue(this.laborPricingData.resourceLaborPricingDTOs[resourceIndex]);
                this.calculateTotalRevenue(this.laborPricingData.resourceLaborPricingDTOs[resourceIndex]);
                this.setCellWiseBillRateZero(resourceIndex);
            }
        }


        private decimal calculateStaffHours(IList<ResourceLaborPricingDTO> resourceLaborPricingDTOs)
        {
            decimal? staffHours = 0;
            foreach (ResourceLaborPricingDTO item in resourceLaborPricingDTOs)
            {
                staffHours = staffHours + (item.TotalHoursPerResource ?? 0);
            }

            return staffHours ?? 0;
        }


        private void calculateMonthlyRevenue(ResourceLaborPricingDTO resource)
        {
            decimal revenue = 0;
            bool isMarginSet = this.laborPricingData.marginDTO == null ? false : this.laborPricingData.marginDTO.IsMarginSet;
            foreach (ProjectResourcePeriodDTO resourcePeriod in resource.projectResourcePeriodDTO)
            {
                if (!this.isDeliveryTypeRestricted)
                {
                    if (resource.TotalHoursPerResource != null && resource.TotalHoursPerResource != 0)
                    {
                        if (isMarginSet)
                        {
                            revenue = resourcePeriod.TotalHours * (resource.Rate ?? resource.RatePerHour) ?? 1;
                        }
                        else
                        {
                            revenue = resourcePeriod.TotalHours * Math.Round((resource.Rate ?? resource.RatePerHour) ?? 1, 2);
                        }

                    }
                    else
                    {
                        revenue = 0;
                    }

                }
                resourcePeriod.Revenue = revenue;
            }
        }

        private void calculateTotalRevenue(ResourceLaborPricingDTO resource)
        {
            if (this.isDeliveryTypeRestricted)
            {
                resource.TotalRevenue = 0;
                resource.Margin = 0;
            }
            else
            {
                decimal? totalRevenue = 0;
                foreach (ProjectResourcePeriodDTO resourcePeriod in resource.projectResourcePeriodDTO)
                {
                    totalRevenue = totalRevenue + resourcePeriod.Revenue + (resourcePeriod.PriceAdjustment ?? 0);
                }
                resource.TotalRevenue = (totalRevenue ?? 0);
                resource.Margin = this.calculateMarginPercent(resource.TotalRevenue, resource.CappedCost);
            }
        }

        private decimal? calculateMarginPercent(decimal? totalRevenue, decimal? cappedCost)
        {
            decimal? margin = 0;
            decimal? marginPercent = 0;
            if (totalRevenue == 0)
            {
                marginPercent = 0;
            }
            else
            {
                margin = (totalRevenue - cappedCost) / totalRevenue;
                marginPercent = margin * 100;
            }
            return (marginPercent ?? 0);
        }

        // calculate Cost per hour 


        private async Task calculateCostPerHr(int resourceIndex)
        {

            decimal? cost = this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].Cost;
            decimal? yearCostPerHour = 0;
            decimal? Percent = 0;
            decimal standardCost = 0;

            if (cost != null && cost >= 0) //  here yearcostperhour value will not be set as it is already overridden
            {
                yearCostPerHour = cost; // no need to do this

                // apply inflation :  though we are not consuming any value of this its essential while we calculate inflation based capped cost.
                this.applyInflationForYear1Cost(this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].LocationId, yearCostPerHour, resourceIndex);
            }
            else
            {
                standardCost = this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].StandardCostRate;
                Percent = this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].Percent;

                if (Percent != null && Percent > 0)
                {
                    yearCostPerHour = ((Percent / 100) * standardCost) + standardCost;
                }
                else
                {
                    yearCostPerHour = standardCost;
                }

                // apply inflation  : 
                if (this.isinflationApplicable)
                {
                    yearCostPerHour = this.applyInflationForYear1Cost(this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].LocationId, yearCostPerHour, resourceIndex);
                }
                else
                {
                    this.setCellWiseCostRate(resourceIndex, yearCostPerHour ?? 0);
                }

                this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].Yr1PerHour = Math.Round(yearCostPerHour ?? 0, 2, MidpointRounding.AwayFromZero);
            }

            this.calculateCappedCost(this.laborPricingData.resourceLaborPricingDTOs[resourceIndex], resourceIndex, yearCostPerHour ?? 0);
        }

        private void calculateCappedCost(ResourceLaborPricingDTO resource, int index, decimal nonRoundedYearCostPerHour)
        {
            decimal? cappedCost = 0;
            this.inflatedCappedCost = 0;
            this.inflationPerResource = 0;
            int currentYear = DateTime.Now.Year;
            int periodIndex = 0;

            if (this.InflationDetailsPerResource != null)
            {
                if (this.isinflationApplicable && this.InflationDetailsPerResource.Count > 0)
                {
                    foreach (ProjectPeriodDTO period in this.laborPricingData.projectPeriodDTO)
                    {
                        if (period.Year >= currentYear)
                        {
                            this.calculateInflationBasedCappedCost(period, resource, periodIndex, resource.Cost ?? resource.Yr1PerHour, index, nonRoundedYearCostPerHour); // Yr1PerHour can be extracted from resource paramater. but too sync with client side logic this is done
                            periodIndex++;
                        }
                        else
                        {
                            cappedCost = resource.CostHrsPerResource * (resource.Cost ?? Math.Round(resource.Yr1PerHour ?? 0, 2));
                            setCellWiseCappedCost(index, periodIndex, ((this.laborPricingData.resourceLaborPricingDTOs[index].projectResourcePeriodDTO[periodIndex].CostHours * resource.Yr1PerHour) ?? 0));
                            periodIndex++;
                        }
                    }
                    cappedCost = (this.inflatedCappedCost == 0) ? cappedCost : this.inflatedCappedCost;
                    resource.TotalInflation = this.inflationPerResource;
                }
                else
                {
                    cappedCost = resource.CostHrsPerResource * (resource.Cost ?? resource.Yr1PerHour);
                    for (int pIndex = 0; pIndex < this.laborPricingData.projectPeriodDTO.Count; pIndex++)
                    {
                        setCellWiseCappedCost(index, pIndex, ((this.laborPricingData.resourceLaborPricingDTOs[index].projectResourcePeriodDTO[pIndex].CostHours * (resource.Cost ?? resource.Yr1PerHour)) ?? 0));
                    }
                }
            }
            else
            {
                cappedCost = resource.CostHrsPerResource * resource.Yr1PerHour;
                for (int pIndex = 0; pIndex < this.laborPricingData.projectPeriodDTO.Count; pIndex++)
                {
                    setCellWiseCappedCost(index, pIndex, (this.laborPricingData.resourceLaborPricingDTOs[index].projectResourcePeriodDTO[pIndex].CostHours * resource.Yr1PerHour) ?? 0);
                }
            }

            resource.CappedCost = Math.Round(cappedCost ?? 0, 2);
            this.calculateTotalRevenue(resource);
        }

        private void calculateInflationBasedCappedCost(ProjectPeriodDTO period, ResourceLaborPricingDTO resource, int periodIndex, decimal? baseYear1Cost, int index, decimal nonRoundedYearCostPerHour)
        {
            decimal? perPeriodCappedCost = 0;
            decimal? costHoursPerPeriod = resource.projectResourcePeriodDTO[periodIndex].CostHours;
            decimal? year1Cost = this.InflationDetailsPerResource.First(specificYear => specificYear.Year == period.Year).Year1Cost;
            decimal? inflationPerPeriod = 0;

            perPeriodCappedCost = (year1Cost * costHoursPerPeriod);

            // calulate inflation based on year
            inflationPerPeriod = perPeriodCappedCost - (costHoursPerPeriod * nonRoundedYearCostPerHour);
            resource.projectResourcePeriodDTO[periodIndex].Inflation = inflationPerPeriod ?? 0;

            setCellWiseCappedCost(index, periodIndex, (perPeriodCappedCost ?? 0));

            // per resource  overrall inflation total
            this.inflationPerResource += inflationPerPeriod;

            this.inflatedCappedCost += perPeriodCappedCost;
        }


        // Inflation

        private bool CheckInflation()
        {
            bool isApplicable = false;
            int currentYear = DateTime.Now.Year;
            List<int> projectYears = this.getYearsInProject();
            int totalProjectYears = projectYears.Count;
            int endYear = projectYears[totalProjectYears - 1];

            if (projectYears.Count > 0 && currentYear < endYear)
            {
                isApplicable = true;
            }

            return isApplicable;
        }

        private List<int> getYearsInProject()
        {
            return this.laborPricingData.projectPeriodDTO.Select(t => t.Year).Distinct().ToList();
        }

        // rate Per hour

        private decimal? applyInflationForRatePerHour(int locationID, decimal? ratePerHour)
        {
            decimal? inflation;
            int currentYear = DateTime.Now.Year;
            decimal? inflationRate = this.getInflationRateOfLocation(locationID);
            decimal? updatedRatePerHour = 0;
            List<int> projectDuration = this.getYearsInProject();
            int totalYears = projectDuration.Count;

            if (currentYear >= projectDuration[0])
            {
                updatedRatePerHour = ratePerHour;
            }
            else
            {
                List<int> diffYears = this.computeDiffrentialYears(currentYear, projectDuration[0], projectDuration[totalYears - 1]);
                if (diffYears.Count > 0)
                {
                    for (int i = 0; i < diffYears.Count; i++)
                    {
                        inflation = 0;
                        inflation = (inflationRate / 100) * ratePerHour;
                        updatedRatePerHour = ratePerHour + inflation;
                        ratePerHour = updatedRatePerHour;
                    }
                }

            }
            return updatedRatePerHour;
        }

        private decimal? getInflationRateOfLocation(int locationID)
        {
            return this.sharedData.LocationDTO.FirstOrDefault(loc => loc.LocationId == locationID).InflationRate;
        }

        private List<int> computeDiffrentialYears(int currentYear, int projectStartYear, int projectEndYear)
        {
            int diff = projectStartYear - currentYear;
            List<int> differentialYears = new List<int>();

            if (diff > 0)
            {
                for (int i = 1; i <= diff; i++)
                {
                    differentialYears.Add(projectStartYear - i);
                }
            }
            else
            {
                if (currentYear < projectEndYear)
                {
                    // calculation should be done by current year  as a base
                    // Inflation should be applied
                    int CurEndYearDiff = projectEndYear - currentYear;
                    differentialYears.Add(projectEndYear);

                    for (int i = 1; i < CurEndYearDiff; i++)
                    {
                        differentialYears.Add(projectEndYear - i);
                    }
                }
            }
            return differentialYears;
        }


        //  year 1 cost inflation 

        private decimal? applyInflationForYear1Cost(int locationId, decimal? year1cost, int resourceIndex)
        {
            if (this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].TotalHoursPerResource != 0)
            {
                this.InflationDetailsPerResource = this.computeYearWiseInflationYear1cost(locationId, Math.Round(year1cost ?? 0, 2, MidpointRounding.AwayFromZero));

                this.setCellWiseCostRate(resourceIndex, (InflationDetailsPerResource.Count > 0 ? InflationDetailsPerResource[0].Year1Cost : year1cost) ?? 0);

                return InflationDetailsPerResource.Count > 0 ? InflationDetailsPerResource[0].Year1Cost : year1cost;
            }
            else
            {
                this.setCellWiseCostRate(resourceIndex, year1cost ?? 0);
                return year1cost;
            }
        }

        private List<InflationDetails> computeYearWiseInflationYear1cost(int locationId, decimal? currentYear1Cost)
        {
            int currentYear = DateTime.Now.Year;
            decimal? inflation = 0;
            List<int> projectDuration = this.getYearsInProject();
            decimal? inflationRate = this.getInflationRateOfLocation(locationId);
            decimal? updatedYear1Cost = 0;
            int totalYears = projectDuration.Count;
            List<InflationDetails> inflationDetails = new List<InflationDetails>();

            List<int> diffYears = new List<int>();

            if (currentYear != projectDuration[0])
            {
                diffYears = this.computeDiffrentialYears(currentYear, projectDuration[0], projectDuration[totalYears - 1]);

            }
            if (diffYears.Count > 0)
            {
                if (currentYear != projectDuration[0])
                {
                    // no change in current year 1 cost
                }
                else
                {
                    // derivating the previous years inflations
                    for (int i = 0; i < diffYears.Count; i++)
                    {
                        inflation = 0;
                        if (i == 0)
                        {
                            updatedYear1Cost = currentYear1Cost + inflation;
                        }
                        else
                        {
                            inflation = (inflationRate / 100) * currentYear1Cost;
                            updatedYear1Cost = currentYear1Cost + inflation;
                        }
                        currentYear1Cost = updatedYear1Cost;
                    }

                }
            }

            // computation based on project start year

            foreach (int particularYear in projectDuration)
            {
                if (particularYear >= currentYear)
                {
                    inflation = 0;
                    // first item project duration indicates -> project start year
                    if (currentYear == particularYear)
                    {
                        updatedYear1Cost = currentYear1Cost + inflation;

                    }
                    else
                    {
                        inflation = (inflationRate / 100) * currentYear1Cost;
                        updatedYear1Cost = currentYear1Cost + inflation;
                    }

                    inflationDetails.Add(new InflationDetails
                    {
                        Inflation = inflation,
                        Year = particularYear,
                        Year1Cost = updatedYear1Cost
                    });

                    currentYear1Cost = updatedYear1Cost;
                }
            }
            return inflationDetails;
        }

        private void setCellWiseCappedCost(int resourceIndex, int periodIndex, decimal cappedCost)
        {
            this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO[periodIndex].CappedCost = cappedCost;
        }

        private void setCellWiseCostRate(int resourceIndex, decimal year1Cost)
        {
            var laborForm = this.laborPricingData.resourceLaborPricingDTOs[resourceIndex];

            if (this.isinflationApplicable)
            {
                // Set Year Wise Inflated Cost Rates
                for (int index = 0; index < laborForm.projectResourcePeriodDTO.Count; index++)
                {
                    decimal periodCostRate = 0;
                    if (this.InflationDetailsPerResource.Count > 0
                        && (from inf in this.InflationDetailsPerResource where inf.Year == this.laborPricingData.projectPeriodDTO[index].Year select (inf.Year1Cost ?? 0)).Any())
                    {
                        periodCostRate = (from inf in this.InflationDetailsPerResource where inf.Year == this.laborPricingData.projectPeriodDTO[index].Year select (inf.Year1Cost ?? 0)).SingleOrDefault();
                    }
                    else
                    {
                        periodCostRate = (this.InflationDetailsPerResource.Count > 1 ? this.InflationDetailsPerResource[0].Year1Cost : year1Cost) ?? 0;
                    }

                    this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO[index].CostRate = periodCostRate;
                }
            }
            else
            {
                // Assign the same value Year1Cost to all the periods
                for (int index = 0; index < laborForm.projectResourcePeriodDTO.Count; index++)
                {
                    this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO[index].CostRate = year1Cost;
                }
            }
        }

        private void setCellWiseBillRateZero(int resourceIndex)
        {
            for (int i = 0; i < this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO.Count; i++)
            {
                this.laborPricingData.resourceLaborPricingDTOs[resourceIndex].projectResourcePeriodDTO[i].BillRate = 0;
            }
        }

        private async Task<LaborPricingDTO> CalculateResourceWiseSeatCost(LaborPricingDTO laborPricingDTO, int pipSheetId)
        {
            LaborPricingBackgroundCalcParentDTO laborPricing = await this.laborPricingRepository.GetResourceCostCalculationFields(pipSheetId);
            DateTime startDate = laborPricing.ResourcePlanningPipSheetDTO[0].StartDate;
            DateTime endDate = laborPricing.ResourcePlanningPipSheetDTO[0].EndDate;
            bool holidayOption = laborPricing.ResourcePlanningPipSheetDTO[0].HolidayOption;

            DateTime startDateFirstDayOfMonth = DateHelper.GetFirstDayOfMonth(startDate);
            DateTime startDateLastDayOfMonth = DateHelper.GetLastDayOfMonth(startDate);
            DateTime endDateFirstDayOfMonth = DateHelper.GetFirstDayOfMonth(endDate);
            DateTime endDateLastDayOfMonth = DateHelper.GetLastDayOfMonth(endDate);

            int noOfPeriods = DateHelper.GetMonthsBetween(startDate, endDate);

            for (int j = 0; j < laborPricingDTO.resourceLaborPricingDTOs.Count; j++)
            {
                decimal locationEbitdaSeatCost = (from l in laborPricing.LocationEbitdaSeatCost
                                                  where l.LocationId == laborPricingDTO.resourceLaborPricingDTOs[j].LocationId
                                                  select l.EbitdaSeatCost).FirstOrDefault();
                decimal resourceSeatCost = 0;

                for (int i = 0; i < laborPricingDTO.resourceLaborPricingDTOs[j].projectResourcePeriodDTO.Count; i++)
                {
                    if (i != 0 && i != noOfPeriods - 1)
                    {
                        resourceSeatCost += locationEbitdaSeatCost * laborPricingDTO.resourceLaborPricingDTOs[j].projectResourcePeriodDTO[i].FTEValue;
                    }
                    else if (i == 0)                //First Period
                    {
                        if (holidayOption)
                        {
                            int daysWorked = 0;
                            if (startDate.Month == endDate.Month && startDate.Year == endDate.Year)
                            {
                                daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, endDate)
                                            - DateHelper.GetHolidaysCount(startDate, startDateLastDayOfMonth, laborPricingDTO.resourceLaborPricingDTOs[j].LocationId, laborPricing.HolidayList);
                            }
                            else
                            {
                                daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, startDateLastDayOfMonth)
                                            - DateHelper.GetHolidaysCount(startDate, startDateLastDayOfMonth, laborPricingDTO.resourceLaborPricingDTOs[j].LocationId, laborPricing.HolidayList);
                            }

                            int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(startDateFirstDayOfMonth, startDateLastDayOfMonth)
                                            - DateHelper.GetHolidaysCount(startDateFirstDayOfMonth, startDateLastDayOfMonth, laborPricingDTO.resourceLaborPricingDTOs[j].LocationId, laborPricing.HolidayList);

                            decimal ebidtaSeatCostPerDay = locationEbitdaSeatCost / noOfWorkingDays;

                            resourceSeatCost += ebidtaSeatCostPerDay * daysWorked * laborPricingDTO.resourceLaborPricingDTOs[j].projectResourcePeriodDTO[i].FTEValue;
                        }
                        else
                        {
                            int daysWorked = 0;
                            if (startDate.Month == endDate.Month && startDate.Year == endDate.Year)
                            {
                                daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, endDate);
                            }
                            else
                            {
                                daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, startDateLastDayOfMonth);
                            }
                            int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(startDateFirstDayOfMonth, startDateLastDayOfMonth);
                            decimal ebidtaSeatCostPerDay = locationEbitdaSeatCost / noOfWorkingDays;
                            resourceSeatCost += ebidtaSeatCostPerDay * daysWorked * laborPricingDTO.resourceLaborPricingDTOs[j].projectResourcePeriodDTO[i].FTEValue;
                        }
                    }
                    else                            //Last Period
                    {
                        if (holidayOption)
                        {
                            int daysWorked = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDate)
                                            - DateHelper.GetHolidaysCount(endDateFirstDayOfMonth, endDate, laborPricingDTO.resourceLaborPricingDTOs[j].LocationId, laborPricing.HolidayList);
                            int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDateLastDayOfMonth)
                                            - DateHelper.GetHolidaysCount(endDateFirstDayOfMonth, endDateLastDayOfMonth, laborPricingDTO.resourceLaborPricingDTOs[j].LocationId, laborPricing.HolidayList);

                            decimal ebidtaSeatCostPerDay = locationEbitdaSeatCost / noOfWorkingDays;
                            resourceSeatCost += ebidtaSeatCostPerDay * daysWorked * laborPricingDTO.resourceLaborPricingDTOs[j].projectResourcePeriodDTO[i].FTEValue;
                        }
                        else
                        {
                            int daysWorked = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDate);
                            int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDateLastDayOfMonth);

                            decimal ebidtaSeatCostPerDay = locationEbitdaSeatCost / noOfWorkingDays;

                            resourceSeatCost += ebidtaSeatCostPerDay * daysWorked * laborPricingDTO.resourceLaborPricingDTOs[j].projectResourcePeriodDTO[i].FTEValue;
                        }
                    }
                }
                laborPricingDTO.resourceLaborPricingDTOs[j].ResourceSeatCost = resourceSeatCost;
            }
            return laborPricingDTO;
        }
    }
}

public class InflationDetails
{
    public int Year { get; set; }
    public decimal? Inflation { get; set; }

    public decimal? Year1Cost { get; set; }
}
