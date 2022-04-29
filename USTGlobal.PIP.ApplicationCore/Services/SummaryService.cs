using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly ISummaryRepository summaryRepository;
        public SummaryService(ISummaryRepository summaryRepository)
        {
            this.summaryRepository = summaryRepository;
        }

        public async Task<List<ProjectSummaryMainDTO>> GetProjectSummary(int pipSheetId)
        {
            ProjectSummaryMainDTO projectSummaryLocalDTO = new ProjectSummaryMainDTO();
            ProjectSummaryMainDTO projectSummaryLocalToUSDDTO = new ProjectSummaryMainDTO();
            List<ProjectSummaryMainDTO> projectSummaryDTOList = new List<ProjectSummaryMainDTO>();
            ProjectSummaryFinalDTO projectSummaryFinalDTO = await this.summaryRepository.GetProjectSummary(pipSheetId);
            if (projectSummaryFinalDTO.ProjectSummaryDTO != null)
            {
                projectSummaryLocalDTO.RiskOrMarginCalculation = projectSummaryFinalDTO.ProjectSummaryDTO.RiskOrMarginCalculation;
                projectSummaryLocalDTO.TotalExpenseReimbursement = projectSummaryFinalDTO.ProjectSummaryDTO.TotalExpenseReimbursement;
                projectSummaryLocalDTO.TotalLaborRevenue = projectSummaryFinalDTO.ProjectSummaryDTO.TotalLaborRevenue;
                projectSummaryLocalDTO.TotalOtherRevenue = projectSummaryFinalDTO.ProjectSummaryDTO.TotalOtherRevenue;
                projectSummaryLocalDTO.TotalPartnerCost = projectSummaryFinalDTO.ProjectSummaryDTO.TotalPartnerCost;
                projectSummaryLocalDTO.TotalPartnerRevenue = projectSummaryFinalDTO.ProjectSummaryDTO.TotalPartnerRevenue;
                projectSummaryLocalDTO.TotalQualifyingDiscounts = projectSummaryFinalDTO.ProjectSummaryDTO.TotalQualifyingDiscounts;
                projectSummaryLocalDTO.VacationAbsence = projectSummaryFinalDTO.ProjectSummaryDTO.VacationAbsence;
                projectSummaryLocalDTO.NetPartnerRevenue = projectSummaryLocalDTO.TotalPartnerRevenue - projectSummaryLocalDTO.TotalPartnerCost;

                projectSummaryDTOList.Add(projectSummaryLocalDTO);

                if (projectSummaryFinalDTO.CurrencyConversion.Symbol != "USD")
                {
                    projectSummaryLocalToUSDDTO.RiskOrMarginCalculation = null;
                    projectSummaryLocalToUSDDTO.TotalExpenseReimbursement = projectSummaryFinalDTO.ProjectSummaryDTO.TotalExpenseReimbursement * projectSummaryFinalDTO.CurrencyConversion.LocalToUSD;
                    projectSummaryLocalToUSDDTO.TotalLaborRevenue = projectSummaryFinalDTO.ProjectSummaryDTO.TotalLaborRevenue * projectSummaryFinalDTO.CurrencyConversion.LocalToUSD;
                    projectSummaryLocalToUSDDTO.TotalOtherRevenue = projectSummaryFinalDTO.ProjectSummaryDTO.TotalOtherRevenue * projectSummaryFinalDTO.CurrencyConversion.LocalToUSD;
                    projectSummaryLocalToUSDDTO.TotalPartnerCost = projectSummaryFinalDTO.ProjectSummaryDTO.TotalPartnerCost * projectSummaryFinalDTO.CurrencyConversion.LocalToUSD;
                    projectSummaryLocalToUSDDTO.TotalPartnerRevenue = projectSummaryFinalDTO.ProjectSummaryDTO.TotalPartnerRevenue * projectSummaryFinalDTO.CurrencyConversion.LocalToUSD;
                    projectSummaryLocalToUSDDTO.TotalQualifyingDiscounts = projectSummaryFinalDTO.ProjectSummaryDTO.TotalQualifyingDiscounts * projectSummaryFinalDTO.CurrencyConversion.LocalToUSD;
                    projectSummaryLocalToUSDDTO.VacationAbsence = null;
                    projectSummaryLocalToUSDDTO.NetPartnerRevenue = projectSummaryLocalToUSDDTO.TotalPartnerRevenue - projectSummaryLocalToUSDDTO.TotalPartnerCost;

                    projectSummaryDTOList.Add(projectSummaryLocalToUSDDTO);
                }
            }
            else
            {
                projectSummaryDTOList.Add(projectSummaryLocalDTO);
                if (projectSummaryFinalDTO.CurrencyConversion.Symbol != "USD")
                {
                    projectSummaryDTOList.Add(projectSummaryLocalToUSDDTO);
                }
            }

            return projectSummaryDTOList;
        }

        public async Task<List<GrossProfitMainDTO>> GetGrossProfit(int pipSheetId)
        {
            GrossProfitMainDTO grossProfitLocalDDTO = new GrossProfitMainDTO();
            GrossProfitMainDTO grossProfitLocalToUSDDTO = new GrossProfitMainDTO();
            List<GrossProfitMainDTO> grossProfitDTOList = new List<GrossProfitMainDTO>();
            GrossProfitFinalDTO grossProfitFinalDTO = await this.summaryRepository.GetGrossProfit(pipSheetId);
            if (grossProfitFinalDTO.GrossProfitDTO != null)
            {
                grossProfitLocalDDTO.TotalClientPrice = grossProfitFinalDTO.GrossProfitDTO.TotalClientPrice;
                grossProfitLocalDDTO.FeesAtRisk = grossProfitFinalDTO.GrossProfitDTO.FeesAtRisk;
                grossProfitLocalDDTO.EstimatedGrossProfit = grossProfitFinalDTO.GrossProfitDTO.NetEstimatedRevenue - grossProfitFinalDTO.GrossProfitDTO.TotalProjectCost;
                grossProfitLocalDDTO.NetEstimatedRevenue = grossProfitFinalDTO.GrossProfitDTO.NetEstimatedRevenue;
                grossProfitLocalDDTO.TotalProjectCost = grossProfitFinalDTO.GrossProfitDTO.TotalProjectCost;
                grossProfitLocalDDTO.TotalSeatCost = grossProfitFinalDTO.GrossProfitDTO.TotalSeatCost;
                grossProfitLocalDDTO.TotalTargetMargin = grossProfitFinalDTO.GrossProfitDTO.TotalTargetMargin;
                grossProfitLocalDDTO.TotalRevenue = grossProfitFinalDTO.GrossProfitDTO.TotalRevenue;

                //Conditions to calculate project gpm percent
                grossProfitLocalDDTO.ProjectGPMPercent = CalculateGPPercent(grossProfitFinalDTO.GrossProfitDTO.TotalProjectCost, grossProfitFinalDTO.GrossProfitDTO.NetEstimatedRevenue);

                //Condition to calculate project EBITDA percent
                if (grossProfitFinalDTO.GrossProfitDTO.NetEstimatedRevenue <= 0)
                    grossProfitLocalDDTO.ProjectEBITDAPercent = 0;
                else
                    grossProfitLocalDDTO.ProjectEBITDAPercent = Math.Round(((grossProfitLocalDDTO.EstimatedGrossProfit - grossProfitLocalDDTO.TotalSeatCost) / grossProfitLocalDDTO.NetEstimatedRevenue) * 100, 4);

                if (grossProfitLocalDDTO.TotalTargetMargin == 0 && grossProfitLocalDDTO.TotalRevenue == 0)
                {
                    grossProfitLocalDDTO.TargetEBITDAPercent = 0;
                }
                else
                {
                    grossProfitLocalDDTO.TargetEBITDAPercent = Math.Round(((grossProfitLocalDDTO.TotalTargetMargin / grossProfitLocalDDTO.TotalRevenue) * 100), 4);
                }

                grossProfitLocalDDTO.Variance = grossProfitLocalDDTO.ProjectEBITDAPercent - grossProfitLocalDDTO.TargetEBITDAPercent;

                grossProfitDTOList.Add(grossProfitLocalDDTO);
                if (grossProfitFinalDTO.CurrencyConversion.Symbol != "USD")
                {
                    grossProfitLocalToUSDDTO.TotalClientPrice = grossProfitFinalDTO.GrossProfitDTO.TotalClientPrice * grossProfitFinalDTO.CurrencyConversion.LocalToUSD;
                    grossProfitLocalToUSDDTO.FeesAtRisk = grossProfitFinalDTO.GrossProfitDTO.FeesAtRisk * grossProfitFinalDTO.CurrencyConversion.LocalToUSD;
                    grossProfitLocalToUSDDTO.EstimatedGrossProfit = (grossProfitFinalDTO.GrossProfitDTO.NetEstimatedRevenue - grossProfitFinalDTO.GrossProfitDTO.TotalProjectCost) * grossProfitFinalDTO.CurrencyConversion.LocalToUSD;
                    grossProfitLocalToUSDDTO.NetEstimatedRevenue = grossProfitFinalDTO.GrossProfitDTO.NetEstimatedRevenue * grossProfitFinalDTO.CurrencyConversion.LocalToUSD;
                    grossProfitLocalToUSDDTO.TotalProjectCost = grossProfitFinalDTO.GrossProfitDTO.TotalProjectCost * grossProfitFinalDTO.CurrencyConversion.LocalToUSD;
                    grossProfitLocalToUSDDTO.TotalSeatCost = grossProfitFinalDTO.GrossProfitDTO.TotalSeatCost * grossProfitFinalDTO.CurrencyConversion.LocalToUSD;
                    grossProfitLocalToUSDDTO.TotalTargetMargin = grossProfitFinalDTO.GrossProfitDTO.TotalTargetMargin * grossProfitFinalDTO.CurrencyConversion.LocalToUSD;
                    grossProfitLocalToUSDDTO.TotalRevenue = grossProfitFinalDTO.GrossProfitDTO.TotalRevenue * grossProfitFinalDTO.CurrencyConversion.LocalToUSD;
                    //Conditions to calculate project gpm percent
                    grossProfitLocalToUSDDTO.ProjectGPMPercent = null;
                    grossProfitLocalToUSDDTO.ProjectEBITDAPercent = null;
                    grossProfitLocalToUSDDTO.TargetEBITDAPercent = null;
                    grossProfitLocalToUSDDTO.Variance = null;
                    grossProfitDTOList.Add(grossProfitLocalToUSDDTO);
                }

            }
            else
            {
                grossProfitDTOList.Add(grossProfitLocalDDTO);
                if (grossProfitFinalDTO.CurrencyConversion.Symbol != "USD")
                {
                    grossProfitDTOList.Add(grossProfitLocalToUSDDTO);
                }
            }

            return grossProfitDTOList;
        }

        public async Task<List<InvestmentViewResultSetDTO>> GetInvestmentView(int pipSheetId)
        {
            List<InvestmentViewResultSetDTO> investmentViewResultSetList = new List<InvestmentViewResultSetDTO>();
            InvestmentViewResultSetDTO investmentViewResultSetDTO = await this.summaryRepository.GetInvestmentView(pipSheetId);
            InvestmentViewDTO investmentViewDTO = new InvestmentViewDTO();
            InvestmentViewResultSetDTO investmentViewResultSetLocalToUSDDTO = new InvestmentViewResultSetDTO();


            if (investmentViewResultSetDTO.investmentView != null)
            {
                investmentViewResultSetList.Add(investmentViewResultSetDTO);
                if (investmentViewResultSetDTO.CurrencyConversion.Symbol != "USD")
                {
                    investmentViewResultSetLocalToUSDDTO.corporateTarget = investmentViewResultSetDTO.corporateTarget;
                    if (investmentViewResultSetDTO.investmentView != null)
                        if (investmentViewResultSetDTO.investmentView != null)
                        {
                            investmentViewDTO.PipSheetId = investmentViewResultSetDTO.investmentView.PipSheetId;
                            investmentViewDTO.CorporateTarget = investmentViewResultSetDTO.investmentView.CorporateTarget;
                            investmentViewDTO.TotalClientPrice = investmentViewResultSetDTO.investmentView.TotalClientPrice * investmentViewResultSetDTO.CurrencyConversion.LocalToUSD;
                            investmentViewDTO.TotalProjectCost = investmentViewResultSetDTO.investmentView.TotalProjectCost * investmentViewResultSetDTO.CurrencyConversion.LocalToUSD;
                        }
                    investmentViewResultSetLocalToUSDDTO.investmentView = investmentViewDTO;
                    investmentViewResultSetList.Add(investmentViewResultSetLocalToUSDDTO);
                }
            }
            else
            {
                investmentViewDTO.PipSheetId = pipSheetId;
                investmentViewResultSetLocalToUSDDTO.investmentView = investmentViewDTO;
                investmentViewResultSetLocalToUSDDTO.CurrencyConversion = investmentViewResultSetDTO.CurrencyConversion;
                investmentViewResultSetLocalToUSDDTO.corporateTarget = investmentViewResultSetDTO.corporateTarget;
                investmentViewResultSetList.Add(investmentViewResultSetLocalToUSDDTO);
            }
            return investmentViewResultSetList;
        }

        public async Task SaveInvestmentView(string userName, InvestmentViewDTO investmentViewDTO)
        {
            await this.summaryRepository.SaveInvestmentView(userName, investmentViewDTO);
        }
        public async Task<EffortSummaryDTO> GetEffortSummary(int pipSheetId)
        {
            return await this.summaryRepository.GetEffortSummary(pipSheetId);
        }
        public async Task<BillingScheduleDetailDTO> GetBillingScheduleDetail(int pipSheetId)
        {
            BillingScheduleResultSetDTO billingScheduleResultSetDTO = await this.summaryRepository.GetBillingScheduleDetail(pipSheetId);

            BillingScheduleDetailDTO billingScheduleDetail = new BillingScheduleDetailDTO();
            List<ClientPriceParentDTO> listClientPriceParentDTO = new List<ClientPriceParentDTO>();
            List<CashFlowParentDTO> listCashFlowClientPriceParentDTO = new List<CashFlowParentDTO>();
            decimal totalOfTotalHrs = 0;

            if (billingScheduleResultSetDTO.billingScheduleCalculatedValueDTO != null)
            {
                billingScheduleDetail.PipSheetId = billingScheduleResultSetDTO.billingScheduleCalculatedValueDTO.PipSheetId;

                billingScheduleResultSetDTO.billingScheduleProjectResourceDTO.ForEach(projectResource =>
                {
                    if (projectResource.TotalRevenue > 0.5m)
                    {
                        totalOfTotalHrs = totalOfTotalHrs + projectResource.TotalHoursPerResource;
                    }
                });

                if (totalOfTotalHrs != 0)
                {
                    billingScheduleDetail.BlendedLaborCostPerHr = billingScheduleResultSetDTO.billingScheduleCalculatedValueDTO.TotalCappedCost / totalOfTotalHrs;
                    billingScheduleDetail.BlendedBillRate = billingScheduleResultSetDTO.billingScheduleCalculatedValueDTO.LaborRevenue / totalOfTotalHrs;
                }

                if (billingScheduleResultSetDTO.clientPriceDTO.Count < 1)
                {
                    List<ProjectPeriodDTO> lstProjectPeriodDTO = new List<ProjectPeriodDTO>();
                    billingScheduleDetail.projectPeriodDTO = lstProjectPeriodDTO;

                    List<ClientPricePeriodDTO> listClientPricePeriodDto = null;
                    for (int i = 0; i <= 4; i++)
                    {
                        ClientPriceParentDTO clientPriceParentDTO = new ClientPriceParentDTO();
                        clientPriceParentDTO.ClientPriceId = i;
                        clientPriceParentDTO.PipSheetId = billingScheduleDetail.PipSheetId;
                        clientPriceParentDTO.DescriptionId = i;
                        clientPriceParentDTO.TotalPrice = 0;

                        listClientPricePeriodDto = new List<ClientPricePeriodDTO>();
                        CashFlowParentDTO cashFlowClientPriceParentDTO = new CashFlowParentDTO();
                        cashFlowClientPriceParentDTO.CashFlowDTO = new List<CashFlowDTO>();
                        cashFlowClientPriceParentDTO.ClientPriceId = i;
                        cashFlowClientPriceParentDTO.PipSheetId = billingScheduleDetail.PipSheetId;
                        cashFlowClientPriceParentDTO.DescriptionId = i;
                        cashFlowClientPriceParentDTO.TotalPrice = 0;
                        cashFlowClientPriceParentDTO.CreatedBy = 0;
                        cashFlowClientPriceParentDTO.DescriptionId = i;
                        cashFlowClientPriceParentDTO.UId =i;
                       
                        listCashFlowClientPriceParentDTO.Add(cashFlowClientPriceParentDTO);
                        
                        clientPriceParentDTO.ClientPricePeriodDTO = listClientPricePeriodDto;
                        listClientPriceParentDTO.Add(clientPriceParentDTO);
                    }
                }
                else
                {
                    billingScheduleResultSetDTO.clientPriceDTO.ForEach(clientPrice =>
                    {
                        ClientPriceParentDTO clientPriceParentDTO = new ClientPriceParentDTO();
                        clientPriceParentDTO.ClientPriceId = clientPrice.ClientPriceId;
                        clientPriceParentDTO.PipSheetId = clientPrice.PipSheetId;
                        clientPriceParentDTO.DescriptionId = clientPrice.DescriptionId;
                        clientPriceParentDTO.TotalPrice = clientPrice.TotalPrice;
                        clientPriceParentDTO.ClientPricePeriodDTO = billingScheduleResultSetDTO.clientPricePeriodDTO.Where(clientPricePeriodDto =>
                        clientPricePeriodDto.ClientPriceId == clientPrice.ClientPriceId)
                               .Select(clientPricePeriodDto => clientPricePeriodDto).ToList();


                        var groupby = billingScheduleResultSetDTO.clientPricePeriodDTO.Where(clientPricePeriodDto =>
                                clientPricePeriodDto.ClientPriceId == clientPrice.ClientPriceId)
                            .Select(clientPricePeriodDto => clientPricePeriodDto).GroupBy(a => new { a.ClientPriceId, a.Year }).ToList();

                        CashFlowParentDTO cashFlowClientPriceParentDTO = new CashFlowParentDTO();
                        cashFlowClientPriceParentDTO.CashFlowDTO = new List<CashFlowDTO>();

                        foreach (var group in groupby)
                        {
                            // Display key and its values.
                            List<ClientPricePeriodDTO> cclientPricePeriodDTO = new List<ClientPricePeriodDTO>();
                            foreach (var clientPricePeriodDTO in group)
                            {
                                cclientPricePeriodDTO.Add(clientPricePeriodDTO);
                            }
                            cashFlowClientPriceParentDTO.ClientPriceId = clientPrice.ClientPriceId;
                            cashFlowClientPriceParentDTO.PipSheetId = clientPrice.PipSheetId;
                            cashFlowClientPriceParentDTO.DescriptionId = clientPrice.DescriptionId;
                            cashFlowClientPriceParentDTO.TotalPrice = clientPrice.TotalPrice;
                            cashFlowClientPriceParentDTO.CreatedBy = clientPrice.CreatedBy;
                            cashFlowClientPriceParentDTO.CreatedOn = clientPrice.CreatedOn;
                            cashFlowClientPriceParentDTO.DescriptionId = clientPrice.DescriptionId;
                            cashFlowClientPriceParentDTO.IsOverrideUpdated = clientPrice.IsOverrideUpdated;
                            cashFlowClientPriceParentDTO.UId = clientPrice.UId;
                            cashFlowClientPriceParentDTO.CashFlowDTO.Add(new CashFlowDTO()
                            {
                                ClientPriceId = group.Key.ClientPriceId,
                                Year = group.Key.Year,
                                SumOfYearPrice = group.Sum(a => a.Price),
                                ClientPricePeriodDTO = cclientPricePeriodDTO
                            });

                        }
                        listCashFlowClientPriceParentDTO.Add(cashFlowClientPriceParentDTO);                        
                        listClientPriceParentDTO.Add(clientPriceParentDTO);

                    });
                    List<ProjectPeriodDTO> lstProjectPeriodDTO = new List<ProjectPeriodDTO>();
                    int periodId = 0;
                    foreach (var projectPeriodDTO in billingScheduleResultSetDTO.projectPeriodDTO)
                    {
                        if (periodId == 0)
                        {
                            lstProjectPeriodDTO.Add(new ProjectPeriodDTO() { BillingPeriodId = 0, CappedCost = 0, Inflation = 0, Month = 0, PipSheetId = projectPeriodDTO.PipSheetId, Year = projectPeriodDTO.Year });
                            lstProjectPeriodDTO.Add(projectPeriodDTO);
                        }
                        else if (periodId != 0 && projectPeriodDTO.Month == 1)
                        {

                            lstProjectPeriodDTO.Add(new ProjectPeriodDTO() { BillingPeriodId = 0, CappedCost = 0, Inflation = 0, Month = 0, PipSheetId = projectPeriodDTO.PipSheetId, Year = projectPeriodDTO.Year });

                            lstProjectPeriodDTO.Add(projectPeriodDTO);
                        }
                        else
                        {
                            lstProjectPeriodDTO.Add(projectPeriodDTO);
                        }
                        periodId++;
                    }
                   
                    billingScheduleDetail.projectPeriodDTO = lstProjectPeriodDTO;

                }

                billingScheduleDetail.cashFlowParentDTO = listCashFlowClientPriceParentDTO;
            }
            else
            {
                ClientPriceParentDTO clientPriceParentDTO = new ClientPriceParentDTO();
                clientPriceParentDTO.ClientPricePeriodDTO = new List<ClientPricePeriodDTO>();
                listClientPriceParentDTO.Add(clientPriceParentDTO);
                billingScheduleDetail.cashFlowParentDTO = new List<CashFlowParentDTO>() { new CashFlowParentDTO() };
                billingScheduleDetail.projectPeriodDTO = new List<ProjectPeriodDTO>();
            }
            return billingScheduleDetail;
        }
        
        public async Task<PLForecastParentDTO> GetPLForecastData(int pipSheetId)
        {
            PLForecastDBResultDTO pLForecastDBResultDTO = await this.summaryRepository.GetPLForecastData(pipSheetId);
            PLForecastParentDTO pLForecastParentDTO = new PLForecastParentDTO();
            
            IList<PLForecastDTO> pLForecastDTO = new List<PLForecastDTO>();

            List<ProjectPeriodDTO> lstProjectPeriodDTO = new List<ProjectPeriodDTO>();
            int projectPeriodId = 0;
            foreach (var projectPeriodDTO in pLForecastDBResultDTO.ProjectPeriodDTO)
            {
                if (projectPeriodId == 0)
                {
                    lstProjectPeriodDTO.Add(new ProjectPeriodDTO() { BillingPeriodId = 0, CappedCost = 0, Inflation = 0, Month = 0, PipSheetId = projectPeriodDTO.PipSheetId, Year = projectPeriodDTO.Year });
                    lstProjectPeriodDTO.Add(projectPeriodDTO);
                }
                else if (projectPeriodId != 0 && projectPeriodDTO.Month == 1)
                {

                    lstProjectPeriodDTO.Add(new ProjectPeriodDTO() { BillingPeriodId = 0, CappedCost = 0, Inflation = 0, Month = 0, PipSheetId = projectPeriodDTO.PipSheetId, Year = projectPeriodDTO.Year });

                    lstProjectPeriodDTO.Add(projectPeriodDTO);
                }
                else
                {
                    lstProjectPeriodDTO.Add(projectPeriodDTO);
                }
                projectPeriodId++;
            }
            pLForecastDBResultDTO.ProjectPeriodDTO = lstProjectPeriodDTO;
            pLForecastDTO.Add(CalculatePriceToClient(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateStaffLaborRevenue(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateVacationAbsence(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculatePartnerRevenue(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateReimbursement(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateSalesDiscount(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateMarginRevenue(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateOtherPriceAdjustment(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateFeesAtRisk(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateNetEstimatedRevenue(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateCostOfStaffing(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateInflationOnly(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateExpensesAndOverhead(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateContingencyCost(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculatePartnerCost(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateCapitalCharge(pLForecastDBResultDTO));
            pLForecastDTO.Insert(10, CalculateProjectCost(pLForecastDBResultDTO, pLForecastDTO));       // DescriptionId = 11
            pLForecastDTO.Add(CalculateGPAmount(pLForecastDBResultDTO, pLForecastDTO));
            pLForecastDTO.Add(CalculateGPMEstimated(pLForecastDBResultDTO, pLForecastDTO));
            pLForecastDTO.Add(CalculateEbitdaSeatCost(pLForecastDBResultDTO));
            pLForecastDTO.Add(CalculateAdjustedEbitda(pLForecastDBResultDTO, pLForecastDTO));
            pLForecastDTO.Add(CalculateAdjustedEbitdaPercent(pLForecastDBResultDTO, pLForecastDTO));
            pLForecastDTO.Add(CalculateHeadcountTotal(pLForecastDBResultDTO));

            pLForecastParentDTO.PLForecastDTO = pLForecastDTO;

            pLForecastParentDTO.ProjectPeriodDTO = lstProjectPeriodDTO;
            return pLForecastParentDTO;
        }

        private PLForecastDTO CalculatePriceToClient(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 1,
                RowSectionId = 1,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalClientPrice)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;
                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.ClientPrice)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Price = Convert.ToDecimal(period.ClientPrice),
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.ClientPrice)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Price = Convert.ToDecimal(period.ClientPrice),
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.ClientPrice),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }

            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateStaffLaborRevenue(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 2,
                RowSectionId = 1,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.LaborRevenue)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {

                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Revenue)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Revenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Revenue)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Revenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Revenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;


                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateVacationAbsence(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 3,
                RowSectionId = 1,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalLostRevenue)) * (-1), //Lost Revenue is always Negative. Hence, multiplied by -1,
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.LostRevenue)) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.LostRevenue) * (-1), //Lost Revenue is always Negative. Hence, multiplied by -1
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.LostRevenue)) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.LostRevenue) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.LostRevenue) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculatePartnerRevenue(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 4,
                RowSectionId = 1,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalPartnerRevenue)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;


                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.PartnerRevenue)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.PartnerRevenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.PartnerRevenue)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.PartnerRevenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.PartnerRevenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }
            return pLForecastDTO;
        }

        private PLForecastDTO CalculateReimbursement(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 5,
                RowSectionId = 1,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalReimbursement)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;


                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Reimbursement)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Reimbursement),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Reimbursement)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Reimbursement),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Reimbursement),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateSalesDiscount(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 6,
                RowSectionId = 1,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalSalesDiscount)) * (-1), //Sales Discount is always Negative. Hence, multiplied by -1
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.SalesDiscount)) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.SalesDiscount) * (-1), //Sales Discount is always Negative. Hence, multiplied by -1
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.SalesDiscount)) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.SalesDiscount) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.SalesDiscount) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }

            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateMarginRevenue(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 7,
                RowSectionId = 1,
                Total = Convert.ToDecimal((pLForecastResult.FixBidCalcDTO == null ? 0 : pLForecastResult.FixBidCalcDTO.TotalCost)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };



            if (pLForecastResult.FixBidCalcPeriodDTO != null && pLForecastResult.FixBidCalcPeriodDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (FixBidCalcPeriodDTO period in pLForecastResult.FixBidCalcPeriodDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.FixBidCalcPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Cost)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Cost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.FixBidCalcPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Cost)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Cost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Cost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }

            }
            else
            {
                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                    {
                        BillingPeriodId = period.BillingPeriodId,
                        Price = 0,
                        DescriptionId = pLForecastDTO.DescriptionId
                    };
                    pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateOtherPriceAdjustment(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 8,
                RowSectionId = 1,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalOtherPriceAdjustment)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.OtherPriceAdjustment)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.OtherPriceAdjustment),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.OtherPriceAdjustment)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.OtherPriceAdjustment),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.OtherPriceAdjustment),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }

            }

            return pLForecastDTO;
        }



        private PLForecastDTO CalculateFeesAtRisk(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 9,
                RowSectionId = 2,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalFeesAtRisk)) * (-1), //Fees At Risk is always Negative. Hence, multiplied by -1
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.FeesAtRisk)) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.FeesAtRisk) * (-1), //Fees At Risk is always Negative. Hence, multiplied by -1
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.FeesAtRisk)) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.FeesAtRisk) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.FeesAtRisk) * (-1),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateNetEstimatedRevenue(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 10,
                RowSectionId = 3,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalNetEstimatedRevenue)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.NetEstimatedRevenue)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.NetEstimatedRevenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.NetEstimatedRevenue)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.NetEstimatedRevenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.NetEstimatedRevenue),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }



        private PLForecastDTO CalculateCostOfStaffing(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 12,
                RowSectionId = 4,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalCappedCost)) -
                (pLForecastResult.ProjectPeriodTotalDTO.Count == 0 ? 0 : (from client in pLForecastResult.ProjectPeriodTotalDTO select client.Inflation).Sum()),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.CappedCost) - pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Inflation),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = period.CappedCost - period.Inflation,
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.CappedCost) - pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Inflation),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = period.CappedCost - period.Inflation,
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = period.CappedCost - period.Inflation,
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateInflationOnly(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 13,
                RowSectionId = 4,
                Total = (pLForecastResult.ProjectPeriodTotalDTO.Count == 0 ? 0 : (from plForecast in pLForecastResult.ProjectPeriodTotalDTO select plForecast.Inflation).Sum()),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Inflation)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Inflation),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Inflation)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Inflation),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.Inflation),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateExpensesAndOverhead(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 14,
                RowSectionId = 4,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalDirectExpense)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.AssetSubTotalExpense)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.AssetSubTotalExpense),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.AssetSubTotalExpense)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.AssetSubTotalExpense),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.AssetSubTotalExpense),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateContingencyCost(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 15,
                RowSectionId = 4,
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };

            pLForecastDTO.Total = (pLForecastResult.ProjectPeriodTotalDTO.Count == 0 ? 0 : (from period in pLForecastDTO.PLForecastPeriodDTO select period.Price).Sum());

            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                for (int i = 0; i < pLForecastResult.ProjectPeriodTotalDTO.Count; i++)
                {

                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO();
                        pLForecastPeriod.BillingPeriodId = 0;
                        pLForecastPeriod.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        if (pLForecastResult.RiskManagementDTO != null)
                        {
                            if (pLForecastResult.RiskManagementDTO.IsOverride)
                            {
                                pLForecastPeriod.Price = pLForecastResult.RiskManagementPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.RiskAmount) ?? 0;
                            }
                            else
                            {
                                if ((pLForecastResult.CalculatedValueDTO.TotalCappedCost ?? 0) != 0)
                                {
                                    pLForecastPeriod.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) *
                               (pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.CappedCost)) / Convert.ToDecimal((pLForecastResult.CalculatedValueDTO.TotalCappedCost == 0 ? 1 : pLForecastResult.CalculatedValueDTO.TotalCappedCost));
                                }
                                else
                                {
                                    pLForecastPeriod.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) / pLForecastResult.ProjectPeriodTotalDTO.Count;
                                }
                            }
                        }
                        else
                        {
                            pLForecastPeriod.Price = 0;
                        }
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO();
                        pLForecastPeriod1.BillingPeriodId = pLForecastResult.ProjectPeriodTotalDTO[i].BillingPeriodId;
                        pLForecastPeriod1.DescriptionId = pLForecastDTO.DescriptionId;
                        pLForecastPeriod1.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        if (pLForecastResult.RiskManagementDTO != null)
                        {
                            if (pLForecastResult.RiskManagementDTO.IsOverride)
                            {
                                pLForecastPeriod1.Price = pLForecastResult.RiskManagementPeriodDTO[i].RiskAmount ?? 0;
                            }
                            else
                            {
                                if ((pLForecastResult.CalculatedValueDTO.TotalCappedCost ?? 0) != 0)
                                {
                                    pLForecastPeriod1.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) *
                               (pLForecastResult.ProjectPeriodTotalDTO[i].CappedCost) / Convert.ToDecimal((pLForecastResult.CalculatedValueDTO.TotalCappedCost == 0 ? 1 : pLForecastResult.CalculatedValueDTO.TotalCappedCost));
                                }
                                else
                                {
                                    pLForecastPeriod1.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) / pLForecastResult.ProjectPeriodTotalDTO.Count;
                                }
                            }
                        }
                        else
                        {
                            pLForecastPeriod1.Price = 0;
                        }
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO();
                        pLForecastPeriod1.BillingPeriodId = 0;
                        pLForecastPeriod1.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        if (pLForecastResult.RiskManagementDTO != null)
                        {
                            if (pLForecastResult.RiskManagementDTO.IsOverride)
                            {
                                pLForecastPeriod1.Price = pLForecastResult.RiskManagementPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.RiskAmount) ?? 0;
                            }
                            else
                            {
                                if ((pLForecastResult.CalculatedValueDTO.TotalCappedCost ?? 0) != 0)
                                {
                                    pLForecastPeriod1.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) *
                               (pLForecastResult.ProjectPeriodTotalDTO[i].CappedCost) / Convert.ToDecimal((pLForecastResult.CalculatedValueDTO.TotalCappedCost == 0 ? 1 : pLForecastResult.CalculatedValueDTO.TotalCappedCost));
                                }
                                else
                                {
                                    pLForecastPeriod1.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) / pLForecastResult.ProjectPeriodTotalDTO.Count;
                                }
                            }
                        }
                        else
                        {
                            pLForecastPeriod1.Price = 0;
                        }
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);



                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO();
                        pLForecastPeriod.BillingPeriodId = pLForecastResult.ProjectPeriodTotalDTO[i].BillingPeriodId;
                        pLForecastPeriod.DescriptionId = pLForecastDTO.DescriptionId;
                        pLForecastPeriod.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        if (pLForecastResult.RiskManagementDTO != null)
                        {
                            if (pLForecastResult.RiskManagementDTO.IsOverride)
                            {
                                pLForecastPeriod.Price = pLForecastResult.RiskManagementPeriodDTO[i].RiskAmount ?? 0;
                            }
                            else
                            {
                                if ((pLForecastResult.CalculatedValueDTO.TotalCappedCost ?? 0) != 0)
                                {
                                    pLForecastPeriod.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) *
                               (pLForecastResult.ProjectPeriodTotalDTO[i].CappedCost) / Convert.ToDecimal((pLForecastResult.CalculatedValueDTO.TotalCappedCost == 0 ? 1 : pLForecastResult.CalculatedValueDTO.TotalCappedCost));
                                }
                                else
                                {
                                    pLForecastPeriod.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) / pLForecastResult.ProjectPeriodTotalDTO.Count;
                                }
                            }
                        }
                        else
                        {
                            pLForecastPeriod.Price = 0;
                        }
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                        projectPeriodId++;
                    }
                    else
                    {


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO();
                        pLForecastPeriod.BillingPeriodId = pLForecastResult.ProjectPeriodTotalDTO[i].BillingPeriodId;
                        pLForecastPeriod.DescriptionId = pLForecastDTO.DescriptionId;
                        pLForecastPeriod.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        if (pLForecastResult.RiskManagementDTO != null)
                        {
                            if (pLForecastResult.RiskManagementDTO.IsOverride)
                            {
                                pLForecastPeriod.Price = pLForecastResult.RiskManagementPeriodDTO[i].RiskAmount ?? 0;
                            }
                            else
                            {
                                if ((pLForecastResult.CalculatedValueDTO.TotalCappedCost ?? 0) != 0)
                                {
                                    pLForecastPeriod.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) *
                               (pLForecastResult.ProjectPeriodTotalDTO[i].CappedCost) / Convert.ToDecimal((pLForecastResult.CalculatedValueDTO.TotalCappedCost == 0 ? 1 : pLForecastResult.CalculatedValueDTO.TotalCappedCost));
                                }
                                else
                                {
                                    pLForecastPeriod.Price = Convert.ToDecimal(pLForecastResult.RiskManagementDTO.TotalAssesedRiskOverrun) / pLForecastResult.ProjectPeriodTotalDTO.Count;
                                }
                            }
                        }
                        else
                        {
                            pLForecastPeriod.Price = 0;
                        }
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }


            return pLForecastDTO;
        }

        private PLForecastDTO CalculatePartnerCost(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 16,
                RowSectionId = 4,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.TotalPartnerCost)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                        Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.PartnerCost)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.PartnerCost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.PartnerCost)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.PartnerCost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.PartnerCost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }

            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateCapitalCharge(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 17,
                RowSectionId = 4,
                Total = Convert.ToDecimal((pLForecastResult.CapitalCharge == null ? 0 : pLForecastResult.CapitalCharge)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.CapitalCharge)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.CapitalCharge),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.CapitalCharge)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.CapitalCharge),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.CapitalCharge),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateProjectCost(PLForecastDBResultDTO pLForecastResult, IList<PLForecastDTO> pLForecastDTO)
        {
            int[] descriptionIds = new int[] { 12 , 13, 14, 15, 16, 17 };
            // Project Cost = Cost of Staffing + Inflation Only + Expenses and Overhead + Contingency Cost + Partner Cost + Capital Charges
            PLForecastDTO tmpPLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 11,
                RowSectionId = 4,
                Total = (from pl in pLForecastDTO where descriptionIds.Contains(pl.DescriptionId) select pl.Total).Sum(),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;
                    
                for (int j = 0; j < pLForecastResult.ProjectPeriodTotalDTO.Count; j++)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO();
                        pLForecastPeriod1.BillingPeriodId = 0;
                        pLForecastPeriod1.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        pLForecastPeriod1.DescriptionId = tmpPLForecastDTO.DescriptionId;
                        decimal? price1 = 0;
                        for (int i = 10; i < 16; i++)
                        {
                            price1 += pLForecastDTO[i].PLForecastPeriodDTO[projectPeriodId].Price;
                        }
                        pLForecastPeriod1.Price = price1;
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        projectPeriodId++;

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO();
                        pLForecastPeriod.BillingPeriodId = pLForecastResult.ProjectPeriodTotalDTO[j].BillingPeriodId;
                        pLForecastPeriod.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        pLForecastPeriod.DescriptionId = tmpPLForecastDTO.DescriptionId;
                        decimal? price = 0;
                        for (int i = 10; i < 16; i++)
                        {
                            price += pLForecastDTO[i].PLForecastPeriodDTO[projectPeriodId].Price;
                        }
                        pLForecastPeriod.Price = price;
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                       
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO();
                        pLForecastPeriod1.BillingPeriodId = 0;
                        pLForecastPeriod1.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        pLForecastPeriod1.DescriptionId = tmpPLForecastDTO.DescriptionId;
                        decimal? price1 = 0;
                        for (int i = 10; i < 16; i++)
                        {
                            price1 += pLForecastDTO[i].PLForecastPeriodDTO[projectPeriodId].Price;
                        }
                        pLForecastPeriod1.Price = price1;
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        projectPeriodId++;
                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO();
                        pLForecastPeriod.BillingPeriodId = pLForecastResult.ProjectPeriodTotalDTO[j].BillingPeriodId;
                        pLForecastPeriod.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        pLForecastPeriod.DescriptionId = tmpPLForecastDTO.DescriptionId;
                        decimal? price = 0;
                        for (int i = 10; i < 16; i++)
                        {
                            price += pLForecastDTO[i].PLForecastPeriodDTO[projectPeriodId].Price;
                        }
                        pLForecastPeriod.Price = price;
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                       
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO();
                        pLForecastPeriod.BillingPeriodId = pLForecastResult.ProjectPeriodTotalDTO[j].BillingPeriodId;
                        pLForecastPeriod.Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year;
                        pLForecastPeriod.DescriptionId = tmpPLForecastDTO.DescriptionId;
                        decimal? price = 0;
                        for (int i = 10; i < 16; i++)
                        {
                            price += pLForecastDTO[i].PLForecastPeriodDTO[projectPeriodId].Price;
                        }
                        pLForecastPeriod.Price = price;
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }

            }

            return tmpPLForecastDTO;
        }

        private PLForecastDTO CalculateGPAmount(PLForecastDBResultDTO pLForecastResult, IList<PLForecastDTO> pLForecastDTO)
        {
            PLForecastDTO estimatedRevenue = pLForecastDTO.ElementAt(9); // DescriptionId = 10; Field : Net Estimated Revenue
            PLForecastDTO projectCost = pLForecastDTO.ElementAt(10); // DescriptionId = 11; Field : Total Project Cost

            PLForecastDTO tmpPLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 18,
                RowSectionId = 5,
                Total = estimatedRevenue.Total - projectCost.Total,
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                for (int i = 0; i < pLForecastResult.ProjectPeriodDTO.Count; i++)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price - projectCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);
                        i++;
                        projectPeriodId++;
                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[projectPeriodId].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price - projectCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price - projectCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);
                       
                        i++;
                        projectPeriodId++;
                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[projectPeriodId].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price - projectCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                       
                    }
                    else
                    {
                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[projectPeriodId].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price - projectCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return tmpPLForecastDTO;
        }

        private PLForecastDTO CalculateGPMEstimated(PLForecastDBResultDTO pLForecastResult, IList<PLForecastDTO> pLForecastDTO)
        {
            PLForecastDTO projectCost = pLForecastDTO.ElementAt(10); // DescriptionId = 11; Field : Project Cost
            PLForecastDTO estimatedRevenue = pLForecastDTO.ElementAt(9); // DescriptionId = 10; Field : Net Estimated Revenue
            PLForecastDTO tmpPLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 19,
                RowSectionId = 6,
                Total = 0,
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            tmpPLForecastDTO.Total = CalculateGPPercent(projectCost.Total ?? 0, estimatedRevenue.Total ?? 0); //GPM Percent
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                for (int i = 0; i < pLForecastResult.ProjectPeriodDTO.Count; i++)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            //Price = estimatedRevenue.PLForecastPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Price) == 0 ? 0 : CalculatePeriodGPPercent(projectCost.PLForecastPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Price) ?? 0, estimatedRevenue.PLForecastPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Price) ?? 0),
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 0 : CalculatePeriodGPPercent(projectCost.PLForecastPeriodDTO[i].Price ?? 0, estimatedRevenue.PLForecastPeriodDTO[i].Price ?? 0),
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        i++;
                        projectPeriodId++;
                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[projectPeriodId].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 0 : CalculatePeriodGPPercent(projectCost.PLForecastPeriodDTO[i].Price ?? 0, estimatedRevenue.PLForecastPeriodDTO[i].Price ?? 0),
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);
                       
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            //Price = estimatedRevenue.PLForecastPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Price) == 0 ? 0 : CalculatePeriodGPPercent(projectCost.PLForecastPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Price) ?? 0, estimatedRevenue.PLForecastPeriodDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.Price) ?? 0),
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 0 : CalculatePeriodGPPercent(projectCost.PLForecastPeriodDTO[i].Price ?? 0, estimatedRevenue.PLForecastPeriodDTO[i].Price ?? 0),
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);
                        i++;
                        projectPeriodId++;
                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[projectPeriodId].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 0 : CalculatePeriodGPPercent(projectCost.PLForecastPeriodDTO[i].Price ?? 0, estimatedRevenue.PLForecastPeriodDTO[i].Price ?? 0),
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                      
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[projectPeriodId].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 0 : CalculatePeriodGPPercent(projectCost.PLForecastPeriodDTO[i].Price ?? 0, estimatedRevenue.PLForecastPeriodDTO[i].Price ?? 0),
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return tmpPLForecastDTO;
        }

        private decimal CalculatePeriodGPPercent(decimal projectCost, decimal estimatedRevenue)
        {
            return ((estimatedRevenue - projectCost) * 100 / (estimatedRevenue == 0 ? 1 : estimatedRevenue));
        }

        private PLForecastDTO CalculateEbitdaSeatCost(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 20,
                RowSectionId = 7,
                Total = Convert.ToDecimal((pLForecastResult.CalculatedValueDTO == null ? 0 : pLForecastResult.CalculatedValueDTO.SeatCostEbitda)),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            // Price = Convert.ToDecimal(period.EbitdaSeatCost),
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.EbitdaSeatCost)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);


                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.EbitdaSeatCost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            //Price = Convert.ToDecimal(period.EbitdaSeatCost),
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.EbitdaSeatCost)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod2 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.EbitdaSeatCost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod2);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.EbitdaSeatCost),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateAdjustedEbitda(PLForecastDBResultDTO pLForecastResult, IList<PLForecastDTO> pLForecastDTO)
        {
            PLForecastDTO gpAmount = pLForecastDTO.ElementAt(17); // DescriptionId = 18; Field : GP Amount
            PLForecastDTO ebitdaSeatCost = pLForecastDTO.ElementAt(19); // DescriptionId = 20; Field : Ebitda Seat Cost

            PLForecastDTO tmpPLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 21,
                RowSectionId = 8,
                Total = gpAmount.Total - ebitdaSeatCost.Total,
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                for (int i = 0; i < pLForecastResult.ProjectPeriodDTO.Count; i++)
                {

                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = gpAmount.PLForecastPeriodDTO[i].Price - ebitdaSeatCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);
                        i++;
                        projectPeriodId++;
                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[i].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = gpAmount.PLForecastPeriodDTO[i].Price - ebitdaSeatCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                       
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = gpAmount.PLForecastPeriodDTO[i].Price - ebitdaSeatCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        i++;
                        projectPeriodId++;
                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[i].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = gpAmount.PLForecastPeriodDTO[i].Price - ebitdaSeatCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                       
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[i].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = gpAmount.PLForecastPeriodDTO[i].Price - ebitdaSeatCost.PLForecastPeriodDTO[i].Price,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return tmpPLForecastDTO;
        }

        private PLForecastDTO CalculateAdjustedEbitdaPercent(PLForecastDBResultDTO pLForecastResult, IList<PLForecastDTO> pLForecastDTO)
        {
            PLForecastDTO estimatedRevenue = pLForecastDTO.ElementAt(9); // DescriptionId = 10; Field : Net Estimated Revenue
            PLForecastDTO adjustedEbitdaRevenue = pLForecastDTO.ElementAt(20); // DescriptionId = 21; Field : Adjusted Ebitda Seat Cost
            PLForecastDTO tmpPLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 22,
                RowSectionId = 9,
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>(),
                Total = 0
            };
            if (estimatedRevenue.Total > 0)
            {
                tmpPLForecastDTO.Total = (adjustedEbitdaRevenue.Total / (estimatedRevenue.Total == 0 ? 1 : estimatedRevenue.Total)) * 100;
            }

            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                for (int i = 0; i < pLForecastResult.ProjectPeriodDTO.Count; i++)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = 0,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        if (estimatedRevenue.PLForecastPeriodDTO[i].Price != 0)
                        {
                            pLForecastPeriod1.Price = (adjustedEbitdaRevenue.PLForecastPeriodDTO[i].Price / (estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 1 :
                                estimatedRevenue.PLForecastPeriodDTO[i].Price)) * 100;
                        }
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        i++;
                        projectPeriodId++;

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[i].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = 0,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };

                        if (estimatedRevenue.PLForecastPeriodDTO[i].Price != 0)
                        {
                            pLForecastPeriod.Price = (adjustedEbitdaRevenue.PLForecastPeriodDTO[i].Price / (estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 1 :
                                estimatedRevenue.PLForecastPeriodDTO[i].Price)) * 100;
                        }
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                                                
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = 0,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        if (estimatedRevenue.PLForecastPeriodDTO[i].Price != 0)
                        {
                            pLForecastPeriod1.Price = (adjustedEbitdaRevenue.PLForecastPeriodDTO[i].Price / (estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 1 :
                                estimatedRevenue.PLForecastPeriodDTO[i].Price)) * 100;
                        }
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        i++;
                        projectPeriodId++;

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[i].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = 0,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };
                        if (estimatedRevenue.PLForecastPeriodDTO[i].Price != 0)
                        {
                            pLForecastPeriod.Price = (adjustedEbitdaRevenue.PLForecastPeriodDTO[i].Price / (estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 1 :
                                estimatedRevenue.PLForecastPeriodDTO[i].Price)) * 100;
                        }
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);                        
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = pLForecastResult.ProjectPeriodDTO[i].BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = 0,
                            DescriptionId = tmpPLForecastDTO.DescriptionId
                        };

                        if (estimatedRevenue.PLForecastPeriodDTO[i].Price != 0)
                        {
                            pLForecastPeriod.Price = (adjustedEbitdaRevenue.PLForecastPeriodDTO[i].Price / (estimatedRevenue.PLForecastPeriodDTO[i].Price == 0 ? 1 :
                                estimatedRevenue.PLForecastPeriodDTO[i].Price)) * 100;
                        }
                        tmpPLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return tmpPLForecastDTO;
        }

        private PLForecastDTO CalculateHeadcountTotal(PLForecastDBResultDTO pLForecastResult)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 23,
                RowSectionId = 10,
                Total = Convert.ToDecimal((pLForecastResult.ProjectPeriodTotalDTO.Count == 0 ? 0 : pLForecastResult.ProjectPeriodTotalDTO.Select(x => x.FTE).Sum())),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            if (pLForecastResult.ProjectPeriodTotalDTO != null && pLForecastResult.ProjectPeriodTotalDTO.Count > 0)
            {
                int projectPeriodId = 0;

                foreach (ProjectPeriodTotalDTO period in pLForecastResult.ProjectPeriodTotalDTO)
                {
                    if (projectPeriodId == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);

                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.FTE)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.FTE),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

                        projectPeriodId++;
                    }
                    else if (projectPeriodId != 0 && pLForecastResult.ProjectPeriodDTO[projectPeriodId].Month == 0)
                    {
                        var months = pLForecastResult.ProjectPeriodDTO.Where(p => p.Year == pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year).Select(m => m.BillingPeriodId);


                        PLForecastPeriodDTO pLForecastPeriod1 = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = 0,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(pLForecastResult.ProjectPeriodTotalDTO.Where(a => months.Contains(a.BillingPeriodId)).Sum(a => a.FTE)),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod1);



                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.FTE),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                        projectPeriodId++;
                    }
                    else
                    {

                        PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                        {
                            BillingPeriodId = period.BillingPeriodId,
                            Year = pLForecastResult.ProjectPeriodDTO[projectPeriodId].Year,
                            Price = Convert.ToDecimal(period.FTE),
                            DescriptionId = pLForecastDTO.DescriptionId
                        };
                        pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
                    }
                    projectPeriodId++;
                }
            }

            return pLForecastDTO;
        }

        public async Task<List<SummaryYoyDTO>> GetYearOverYearComparisonData(int pipSheetId)
        {
            SummaryYoyDbResultDTO summaryYoyDbResultDTO = await this.summaryRepository.GetYearOverYearComparisonData(pipSheetId);
            List<SummaryYoyDTO> summaryYoyList = new List<SummaryYoyDTO>();

            summaryYoyList.Add(CalculateYoyTotalEstimatedRevenueYearWise(summaryYoyDbResultDTO));
            summaryYoyList.Add(CalculateYoyProjectCostYearWise(summaryYoyDbResultDTO));
            summaryYoyList.Add(CalculateYoyGPMAmountYearWise(summaryYoyDbResultDTO, summaryYoyList));
            summaryYoyList.Add(CalculateYoyGPMPercentYearWise(summaryYoyDbResultDTO, summaryYoyList));
            summaryYoyList.Add(CalculateYoyEbitdaYearWise(summaryYoyDbResultDTO, summaryYoyList));
            summaryYoyList.Add(CalculateYoyAdjustedEbitaPercentYearWise(summaryYoyDbResultDTO, summaryYoyList));

            return summaryYoyList;
        }

        private SummaryYoyDTO CalculateYoyTotalEstimatedRevenueYearWise(SummaryYoyDbResultDTO summaryYoyResult)
        {
            SummaryYoyDTO summaryYoyDTO = new SummaryYoyDTO
            {
                DescriptionId = 1,
                Total = Convert.ToDecimal((summaryYoyResult.CalculatedValueDTO == null ? 0 : summaryYoyResult.CalculatedValueDTO.TotalNetEstimatedRevenue)),
                SummaryYoyPeriodList = new List<SummaryYoyYearDTO>()
            };
            if (summaryYoyResult.ProjectYearTotalDTO != null && summaryYoyResult.ProjectYearTotalDTO.Count != 0)
            {
                foreach (ProjectYearTotalDTO year in summaryYoyResult.ProjectYearTotalDTO)
                {
                    SummaryYoyYearDTO summaryYoyYear = new SummaryYoyYearDTO
                    {
                        YearId = year.YearId,
                        Year = year.Year,
                        Price = year.NetEstimatedRevenue
                    };
                    summaryYoyDTO.SummaryYoyPeriodList.Add(summaryYoyYear);
                }
            }

            return summaryYoyDTO;
        }

        private SummaryYoyDTO CalculateYoyProjectCostYearWise(SummaryYoyDbResultDTO summaryYoyResult)
        {
            SummaryYoyDTO summaryYoyDTO = new SummaryYoyDTO
            {
                DescriptionId = 2,
                Total = 0,
                SummaryYoyPeriodList = new List<SummaryYoyYearDTO>()
            };
            decimal totalProjectCost = 0;
            if (summaryYoyResult.ProjectYearTotalDTO != null && summaryYoyResult.ProjectYearTotalDTO.Count != 0)
            {
                foreach (ProjectYearTotalDTO year in summaryYoyResult.ProjectYearTotalDTO)
                {
                    SummaryYoyYearDTO summaryYoyYear = new SummaryYoyYearDTO
                    {
                        YearId = year.YearId,
                        Year = year.Year,
                        Price = year.CappedCost +
                    year.AssetSubTotalExpense +
                    (Convert.ToDecimal(summaryYoyResult.RiskManagementDTO == null ? 0 : summaryYoyResult.RiskManagementDTO.TotalAssesedRiskOverrun) *
                    (year.CappedCost) / Convert.ToDecimal(((summaryYoyResult.CalculatedValueDTO == null ? 0 : summaryYoyResult.CalculatedValueDTO.TotalCappedCost ?? 0) == 0 ? 1 : summaryYoyResult.CalculatedValueDTO.TotalCappedCost))) +
                    year.PartnerCost +
                    year.CapitalCharge
                    };
                    if (summaryYoyYear.Price == 0)
                    {
                        summaryYoyYear.Price += year.CostContingency;
                    }
                    summaryYoyDTO.SummaryYoyPeriodList.Add(summaryYoyYear);
                    totalProjectCost += summaryYoyYear.Price;
                }
            }

            summaryYoyDTO.Total = totalProjectCost;
            return summaryYoyDTO;
        }

        private SummaryYoyDTO CalculateYoyGPMAmountYearWise(SummaryYoyDbResultDTO summaryYoyResult, IList<SummaryYoyDTO> summaryYoyList)
        {
            SummaryYoyDTO netEstimatedRevenue = summaryYoyList.ElementAt(0);
            SummaryYoyDTO totalProjectCost = summaryYoyList.ElementAt(1);
            SummaryYoyDTO summaryYoyDTO = new SummaryYoyDTO
            {
                DescriptionId = 3,
                Total = netEstimatedRevenue.Total - totalProjectCost.Total,
                SummaryYoyPeriodList = new List<SummaryYoyYearDTO>()
            };
            if (summaryYoyResult.ProjectYearTotalDTO != null && summaryYoyResult.ProjectYearTotalDTO.Count != 0)
            {
                for (int i = 0; i < summaryYoyResult.ProjectYearTotalDTO.Count; i++)
                {
                    SummaryYoyYearDTO summaryYoyYear = new SummaryYoyYearDTO
                    {
                        YearId = summaryYoyResult.ProjectYearTotalDTO[i].YearId,
                        Year = summaryYoyResult.ProjectYearTotalDTO[i].Year,
                        Price = netEstimatedRevenue.SummaryYoyPeriodList[i].Price - totalProjectCost.SummaryYoyPeriodList[i].Price
                    };
                    summaryYoyDTO.SummaryYoyPeriodList.Add(summaryYoyYear);
                }
            }

            return summaryYoyDTO;
        }

        private SummaryYoyDTO CalculateYoyGPMPercentYearWise(SummaryYoyDbResultDTO summaryYoyResult, IList<SummaryYoyDTO> summaryYoyList)
        {
            SummaryYoyDTO netEstimatedRevenue = summaryYoyList.ElementAt(0);
            SummaryYoyDTO projectCost = summaryYoyList.ElementAt(1);
            SummaryYoyDTO summaryYoyDTO = new SummaryYoyDTO
            {
                DescriptionId = 4,
                Total = 0,
                SummaryYoyPeriodList = new List<SummaryYoyYearDTO>()
            };
            summaryYoyDTO.Total = CalculateGPPercent(projectCost.Total, netEstimatedRevenue.Total);
            if (summaryYoyResult.ProjectYearTotalDTO != null && summaryYoyResult.ProjectYearTotalDTO.Count != 0)
            {
                for (int i = 0; i < summaryYoyResult.ProjectYearTotalDTO.Count; i++)
                {
                    SummaryYoyYearDTO summaryYoyYear = new SummaryYoyYearDTO
                    {
                        YearId = summaryYoyResult.ProjectYearTotalDTO[i].YearId,
                        Year = summaryYoyResult.ProjectYearTotalDTO[i].Year,
                        Price = 0
                    };
                    summaryYoyYear.Price = CalculateGPPercent(projectCost.SummaryYoyPeriodList[i].Price, netEstimatedRevenue.SummaryYoyPeriodList[i].Price);
                    summaryYoyDTO.SummaryYoyPeriodList.Add(summaryYoyYear);
                }
            }

            return summaryYoyDTO;
        }

        private SummaryYoyDTO CalculateYoyEbitdaYearWise(SummaryYoyDbResultDTO summaryYoyResult, IList<SummaryYoyDTO> summaryYoyList)
        {
            SummaryYoyDTO gpAmount = summaryYoyList.ElementAt(2);

            SummaryYoyDTO summaryYoyDTO = new SummaryYoyDTO
            {
                DescriptionId = 5,
                Total = Convert.ToDecimal(gpAmount.Total - ((summaryYoyResult.CalculatedValueDTO == null ? 0 : summaryYoyResult.CalculatedValueDTO.SeatCostEbitda))),
                SummaryYoyPeriodList = new List<SummaryYoyYearDTO>()
            };
            if (summaryYoyResult.ProjectYearTotalDTO != null && summaryYoyResult.ProjectYearTotalDTO.Count != 0)
            {
                for (int i = 0; i < summaryYoyResult.ProjectYearTotalDTO.Count; i++)
                {
                    SummaryYoyYearDTO summaryYoyYear = new SummaryYoyYearDTO
                    {
                        YearId = summaryYoyResult.ProjectYearTotalDTO[i].YearId,
                        Year = summaryYoyResult.ProjectYearTotalDTO[i].Year,
                        Price = gpAmount.SummaryYoyPeriodList[i].Price - summaryYoyResult.ProjectYearTotalDTO[i].EbitdaSeatCost
                    };
                    summaryYoyDTO.SummaryYoyPeriodList.Add(summaryYoyYear);
                }
            }

            return summaryYoyDTO;
        }

        private SummaryYoyDTO CalculateYoyAdjustedEbitaPercentYearWise(SummaryYoyDbResultDTO summaryYoyResult, IList<SummaryYoyDTO> summaryYoyList)
        {
            SummaryYoyDTO netEstimatedRevenue = summaryYoyList.ElementAt(0);
            SummaryYoyDTO seatCostEbitda = summaryYoyList.ElementAt(4);
            SummaryYoyDTO summaryYoyDTO = new SummaryYoyDTO
            {
                DescriptionId = 6,
                Total = 0,
                SummaryYoyPeriodList = new List<SummaryYoyYearDTO>()
            };
            if (netEstimatedRevenue.Total != 0)
            {
                summaryYoyDTO.Total = CalculatePeriodEBITDAPercent(netEstimatedRevenue.Total, seatCostEbitda.Total);
            }
            if (summaryYoyResult.ProjectYearTotalDTO != null && summaryYoyResult.ProjectYearTotalDTO.Count != 0)
            {
                for (int i = 0; i < summaryYoyResult.ProjectYearTotalDTO.Count; i++)
                {
                    SummaryYoyYearDTO summaryYoyYear = new SummaryYoyYearDTO
                    {
                        YearId = summaryYoyResult.ProjectYearTotalDTO[i].YearId,
                        Year = summaryYoyResult.ProjectYearTotalDTO[i].Year,
                        Price = CalculatePeriodEBITDAPercent(netEstimatedRevenue.SummaryYoyPeriodList[i].Price, seatCostEbitda.SummaryYoyPeriodList[i].Price)
                    };

                    summaryYoyDTO.SummaryYoyPeriodList.Add(summaryYoyYear);
                }
            }

            return summaryYoyDTO;
        }

        private decimal CalculateGPPercent(decimal projectCost, decimal estimatedRevenue)
        {
            if (estimatedRevenue - projectCost == 0)
            {
                return 0;
            }
            else if (estimatedRevenue >= 0 && projectCost < 0)
            {
                return 100;
            }
            else if (estimatedRevenue <= 0 && projectCost >= 0)
            {
                return -100;
            }
            else if (projectCost < 0)
            {
                return (-1 * (estimatedRevenue - projectCost) * 100 / (estimatedRevenue == 0 ? 1 : estimatedRevenue));
            }
            else
            {
                return ((estimatedRevenue - projectCost) * 100 / (estimatedRevenue == 0 ? 1 : estimatedRevenue));
            }
        }

        private decimal CalculatePeriodEBITDAPercent(decimal totalEstimatedRevenue, decimal totalEbitda)
        {
            if (totalEbitda - totalEstimatedRevenue == 0)
            {
                return 0;
            }
            else if (totalEbitda >= 0 && totalEstimatedRevenue < 0)
            {
                return 0;
            }
            else if (totalEbitda <= 0 && totalEstimatedRevenue >= 0)
            {
                return 0;
            }
            else if (totalEstimatedRevenue < 0)
            {
                if (totalEbitda < 0 && totalEstimatedRevenue < 0)
                {
                    return (-1 * Math.Abs(totalEbitda * 100) /
                   Math.Abs(totalEstimatedRevenue == 0 ? 1 : totalEstimatedRevenue));
                }
                else
                {
                    return (-1 * (totalEbitda * 100) /
                        Math.Abs(totalEstimatedRevenue == 0 ? 1 : totalEstimatedRevenue));
                }
            }
            else
            {
                return (totalEbitda * 100) /
                    Math.Abs(totalEstimatedRevenue == 0 ? 1 : totalEstimatedRevenue);
            }
        }

        public async Task SavePLForecastData(string userName, IList<PLForecastDTO> plForecastDTO, int pipSheetId)
        {
            IList<PLForecastSubDTO> plForecastSubDTO = plForecastDTO
                .Select(plForecast => new PLForecastSubDTO
                {  
                    DescriptionId = plForecast.DescriptionId,
                    RowSectionId = plForecast.RowSectionId,
                    Total = plForecast.Total
                }).ToList();

            IList<PLForecastPeriodDTO> plForecastPeriods = plForecastDTO.SelectMany(plForecast => plForecast.PLForecastPeriodDTO).Where(a=>a.BillingPeriodId!=0).ToList();

            await this.summaryRepository.SavePLForecastData(userName, pipSheetId, plForecastSubDTO, plForecastPeriods);
        }

        public async Task<KeyPerformanceIndicatorDTO> GetKeyPerformanceIndicatorsData(int pipSheetId)
        {
            KeyPerformanceIndicatorDTO keyPerformanceIndicatorDTO = await summaryRepository.GetKeyPerformanceIndicatorsData(pipSheetId);

            keyPerformanceIndicatorDTO.ServiceLineRevenueList = keyPerformanceIndicatorDTO.ServiceLineRevenueList.OrderByDescending(revenue => revenue.ServiceLineRevenue).ToList();
            keyPerformanceIndicatorDTO.ServiceLineEbitdaPercentList = keyPerformanceIndicatorDTO.ServiceLineEbitdaPercentList.OrderByDescending(ebitdaPercent => ebitdaPercent.ServiceLineEbitdaPercent).ToList();

            return keyPerformanceIndicatorDTO;
        }

        public async Task<LocationWiseSummaryDTO> GetLocationWiseDetails(int pipSheetId)
        {
            LocationWiseCalculationMainDTO locationWiseCalculationMainDTO = await this.summaryRepository.GetLocationWiseDetail(pipSheetId);

            LocationWiseSummaryDTO locationWiseSummaryDTO = new LocationWiseSummaryDTO();
            locationWiseSummaryDTO.LocationWiseDetails = new List<LocationWiseDetailsDTO>();
            locationWiseSummaryDTO.SummaryLocations = new List<ProjectSummaryLocationDTO>();
            locationWiseSummaryDTO.SummaryLocations.AddRange(locationWiseCalculationMainDTO.projectSummaryLocationDTO);

            LocationWiseDetailsDTO locationWiseDetailsDTO = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO.SummaryLocationDTO = new List<SummaryLocationDTO>();

            LocationWiseDetailsDTO locationWiseDetailsDTO1 = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO1.SummaryLocationDTO = new List<SummaryLocationDTO>();

            LocationWiseDetailsDTO locationWiseDetailsDTO2 = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO2.SummaryLocationDTO = new List<SummaryLocationDTO>();

            LocationWiseDetailsDTO locationWiseDetailsDTO3 = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO3.SummaryLocationDTO = new List<SummaryLocationDTO>();

            LocationWiseDetailsDTO locationWiseDetailsDTO4 = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO4.SummaryLocationDTO = new List<SummaryLocationDTO>();

            LocationWiseDetailsDTO locationWiseDetailsDTO5 = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO5.SummaryLocationDTO = new List<SummaryLocationDTO>();

            LocationWiseDetailsDTO locationWiseDetailsDTO6 = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO6.SummaryLocationDTO = new List<SummaryLocationDTO>();

            LocationWiseDetailsDTO locationWiseDetailsDTO7 = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO7.SummaryLocationDTO = new List<SummaryLocationDTO>();

            LocationWiseDetailsDTO locationWiseDetailsDTO8 = new LocationWiseDetailsDTO();
            locationWiseDetailsDTO8.SummaryLocationDTO = new List<SummaryLocationDTO>();

            decimal totalCostHours = 0;
            decimal totalBilledHours = 0;


            for (int i = 0; i < locationWiseCalculationMainDTO.projectSummaryLocationDTO.Count(); i++)
            {
                //Row 1
                SummaryLocationDTO resourceRevenue = new SummaryLocationDTO();
                resourceRevenue.DescriptionId = 1;
                resourceRevenue.Amount = (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceRevenue);
                resourceRevenue.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;

                locationWiseDetailsDTO.DescriptionId = 1;
                locationWiseDetailsDTO.Total = locationWiseDetailsDTO.Total + resourceRevenue.Amount;
                locationWiseDetailsDTO.SummaryLocationDTO.Add(resourceRevenue);

                //Row 2
                SummaryLocationDTO resourceCost = new SummaryLocationDTO();
                resourceCost.DescriptionId = 2;
                resourceCost.Amount = (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceCost);
                resourceCost.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;

                locationWiseDetailsDTO1.DescriptionId = 2;
                locationWiseDetailsDTO1.Total = locationWiseDetailsDTO1.Total + resourceCost.Amount;
                locationWiseDetailsDTO1.SummaryLocationDTO.Add(resourceCost);

                //Row 3
                SummaryLocationDTO ebitdaPercent = new SummaryLocationDTO();
                ebitdaPercent.DescriptionId = 3;
                if (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceRevenue == 0)
                {
                    ebitdaPercent.Amount = 0;
                }
                else
                {
                    ebitdaPercent.Amount = ((1 - (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceCost /
                        locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceRevenue)) * 100);
                }
                ebitdaPercent.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;
                locationWiseDetailsDTO2.DescriptionId = 3;
                locationWiseDetailsDTO2.SummaryLocationDTO.Add(ebitdaPercent);

                //Row 4
                SummaryLocationDTO avgCostRate = new SummaryLocationDTO();
                decimal totalCostHrs = locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalCostHrs == 0 ? 1 :
                    locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalCostHrs;
                avgCostRate.DescriptionId = 4;
                if (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalCostHrs == 0)
                {
                    avgCostRate.Amount = 0;
                }
                else
                {
                    avgCostRate.Amount = (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceCost /
                        locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalCostHrs);
                }
                avgCostRate.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;
                totalCostHours = totalCostHours + locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalCostHrs;
                locationWiseDetailsDTO3.DescriptionId = 4;
                locationWiseDetailsDTO3.SummaryLocationDTO.Add(avgCostRate);

                //Row 5
                SummaryLocationDTO avgBillRate = new SummaryLocationDTO();
                avgBillRate.DescriptionId = 5;

                if (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalBilledHrs == 0)
                {
                    avgBillRate.Amount = 0;
                }
                else
                {
                    avgBillRate.Amount = (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceRevenue /
                        locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalBilledHrs);
                }

                avgBillRate.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;
                totalBilledHours = totalBilledHours + locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalBilledHrs;
                locationWiseDetailsDTO4.DescriptionId = 5;
                locationWiseDetailsDTO4.SummaryLocationDTO.Add(avgBillRate);

                //Row 6
                SummaryLocationDTO totalFTE = new SummaryLocationDTO();
                totalFTE.DescriptionId = 6;
                totalFTE.Amount = (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].TotalFTE);
                totalFTE.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;

                locationWiseDetailsDTO5.DescriptionId = 6;
                locationWiseDetailsDTO5.Total = locationWiseDetailsDTO5.Total + totalFTE.Amount;
                locationWiseDetailsDTO5.SummaryLocationDTO.Add(totalFTE);

                //Row 7
                SummaryLocationDTO nonBillableFTE = new SummaryLocationDTO();
                nonBillableFTE.DescriptionId = 7;
                nonBillableFTE.Amount = (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].NonBillableFTE);
                nonBillableFTE.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;

                locationWiseDetailsDTO6.DescriptionId = 7;
                locationWiseDetailsDTO6.Total = locationWiseDetailsDTO6.Total + nonBillableFTE.Amount;
                locationWiseDetailsDTO6.SummaryLocationDTO.Add(nonBillableFTE);

                //Row 8
                SummaryLocationDTO nonBillableCost = new SummaryLocationDTO();
                nonBillableCost.DescriptionId = 8;
                nonBillableCost.Amount = (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].NonBillableCost);
                nonBillableCost.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;

                locationWiseDetailsDTO7.DescriptionId = 8;
                locationWiseDetailsDTO7.Total = locationWiseDetailsDTO7.Total + nonBillableCost.Amount;
                locationWiseDetailsDTO7.SummaryLocationDTO.Add(nonBillableCost);

                //Row 9
                SummaryLocationDTO nonBillableCostPercent = new SummaryLocationDTO();

                nonBillableCostPercent.DescriptionId = 9;
                if (locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceRevenue == 0)
                {
                    nonBillableCostPercent.Amount = 0;
                }
                else
                {
                    nonBillableCostPercent.Amount = ((locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].NonBillableCost / locationWiseCalculationMainDTO.LocationWiseDetailDTO[i].ResourceRevenue) * 100);
                }
                locationWiseDetailsDTO8.DescriptionId = 9;
                nonBillableCostPercent.LocationId = locationWiseCalculationMainDTO.projectSummaryLocationDTO[i].LocationId;
                locationWiseDetailsDTO8.SummaryLocationDTO.Add(nonBillableCostPercent);
            }

            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO);
            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO1);
            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO2);
            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO3);
            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO4);
            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO5);
            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO6);
            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO7);
            locationWiseSummaryDTO.LocationWiseDetails.Add(locationWiseDetailsDTO8);

            //Row 3 total
            if (locationWiseSummaryDTO.LocationWiseDetails[0].Total == 0)
            {
                locationWiseSummaryDTO.LocationWiseDetails[2].Total = 0;
            }
            else
            {
                locationWiseSummaryDTO.LocationWiseDetails[2].Total = ((1 - (locationWiseSummaryDTO.LocationWiseDetails[1].Total /
                                               locationWiseSummaryDTO.LocationWiseDetails[0].Total)) * 100);
            }

            //Row 4 total
            if (totalCostHours == 0)
            {
                locationWiseSummaryDTO.LocationWiseDetails[3].Total = 0;
            }
            else
            {
                locationWiseSummaryDTO.LocationWiseDetails[3].Total = locationWiseSummaryDTO.LocationWiseDetails[1].Total / totalCostHours;
            }

            //Row 5 total
            if (totalBilledHours == 0)
            {
                locationWiseSummaryDTO.LocationWiseDetails[4].Total = 0;
            }
            else
            {
                locationWiseSummaryDTO.LocationWiseDetails[4].Total = (locationWiseSummaryDTO.LocationWiseDetails[0].Total / totalBilledHours);
            }

            //Row 9 total
            if (locationWiseSummaryDTO.LocationWiseDetails[0].Total == 0)
            {
                locationWiseSummaryDTO.LocationWiseDetails[8].Total = 0;
            }
            else
            {
                locationWiseSummaryDTO.LocationWiseDetails[8].Total = ((locationWiseSummaryDTO.LocationWiseDetails[7].Total /
                    locationWiseSummaryDTO.LocationWiseDetails[0].Total) * 100);
            }
            return locationWiseSummaryDTO;
        }

        public async Task<List<TotalDealFinancialsDTO>> GetTotalDealFinancials(int pipSheetId)
        {
            List<TotalDealFinancialsDTO> totalDealFinancials = new List<TotalDealFinancialsDTO>();
            TotalDealFinancialsDBResultDTO totalDealFinancialsDBResult = await this.summaryRepository.GetTotalDealFinancials(pipSheetId);
            totalDealFinancials.Add(CalculateTDFPriceToClient(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFClientServiceFees(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFClientReimbursableExpenses(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFClientPartnerFees(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFFeeAtRisk(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFNetRevenue(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFNetRevenueServiceFees(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFNetRevenueReimbursableExpenses(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFNetRevenuePartnerFees(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFTotalProjectCosts(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFTotalProjectResourceCost(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFTotalProjectDirectExpenseCost(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFTotalProjectPartnerCosts(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFGrossMargin(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFGrossMarginPercent(totalDealFinancialsDBResult, totalDealFinancials));
            totalDealFinancials.Add(CalculateTDFPartnerMarginPercent(totalDealFinancialsDBResult, totalDealFinancials));
            totalDealFinancials.Add(CalculateTDFSGACost(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFEbitda(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFProjectEbitdaPercent(totalDealFinancialsDBResult, totalDealFinancials));
            totalDealFinancials.Add(CalculateTDFCorporateVerticalOverheadPercent(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateTDFCorporateEbitdaPercent(totalDealFinancialsDBResult, totalDealFinancials));
            totalDealFinancials.Add(CalculateTDFBeatTaxImpactPercent(totalDealFinancialsDBResult));
            totalDealFinancials.Add(CalculateContributionPercent(totalDealFinancialsDBResult, totalDealFinancials));

            return totalDealFinancials;
        }

        private TotalDealFinancialsDTO CalculateTDFPriceToClient(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalPriceToClient = 0;
            TotalDealFinancialsDTO priceToClient = new TotalDealFinancialsDTO();
            priceToClient.DescriptionId = 1;        // Price To Client
            priceToClient.RowSectionId = 1;
            priceToClient.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].ClientPrice;
                totalPriceToClient += year.Amount;
                priceToClient.TotalDealFinancialsYearList.Add(year);
            }
            priceToClient.TotalLocal = totalPriceToClient;
            priceToClient.TotalUSD = totalPriceToClient * tdfDBResult.LocalToUSDCurrencyFactor;

            return priceToClient;
        }

        private TotalDealFinancialsDTO CalculateTDFClientServiceFees(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalClientServiceFees = 0;
            TotalDealFinancialsDTO clientServiceFees = new TotalDealFinancialsDTO();
            clientServiceFees.DescriptionId = 2;        // Client Service Fees
            clientServiceFees.RowSectionId = 1;
            clientServiceFees.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].ClientServiceFees;
                totalClientServiceFees += year.Amount;
                clientServiceFees.TotalDealFinancialsYearList.Add(year);
            }
            clientServiceFees.TotalLocal = totalClientServiceFees;
            clientServiceFees.TotalUSD = totalClientServiceFees * tdfDBResult.LocalToUSDCurrencyFactor;

            return clientServiceFees;
        }

        private TotalDealFinancialsDTO CalculateTDFClientReimbursableExpenses(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalClientReimbursableExpenses = 0;
            TotalDealFinancialsDTO ClientReimbursableExpenses = new TotalDealFinancialsDTO();
            ClientReimbursableExpenses.DescriptionId = 3;        // Client Reimbursable Expenses
            ClientReimbursableExpenses.RowSectionId = 1;
            ClientReimbursableExpenses.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].ClientReimbursableExpense;
                totalClientReimbursableExpenses += year.Amount;
                ClientReimbursableExpenses.TotalDealFinancialsYearList.Add(year);
            }
            ClientReimbursableExpenses.TotalLocal = totalClientReimbursableExpenses;
            ClientReimbursableExpenses.TotalUSD = totalClientReimbursableExpenses * tdfDBResult.LocalToUSDCurrencyFactor;

            return ClientReimbursableExpenses;
        }

        private TotalDealFinancialsDTO CalculateTDFClientPartnerFees(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalClientPartnerFees = 0;
            TotalDealFinancialsDTO ClientPartnerFees = new TotalDealFinancialsDTO();
            ClientPartnerFees.DescriptionId = 4;        // Client Partner Fees
            ClientPartnerFees.RowSectionId = 1;
            ClientPartnerFees.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].ClientPartnerFees;
                totalClientPartnerFees += year.Amount;
                ClientPartnerFees.TotalDealFinancialsYearList.Add(year);
            }
            ClientPartnerFees.TotalLocal = totalClientPartnerFees;
            ClientPartnerFees.TotalUSD = totalClientPartnerFees * tdfDBResult.LocalToUSDCurrencyFactor;

            return ClientPartnerFees;
        }

        private TotalDealFinancialsDTO CalculateTDFFeeAtRisk(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalFeesAtRisk = 0;
            TotalDealFinancialsDTO FeeAtRisk = new TotalDealFinancialsDTO();
            FeeAtRisk.DescriptionId = 5;        // Fee at risk
            FeeAtRisk.RowSectionId = 2;
            FeeAtRisk.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].FeeAtRisk;
                totalFeesAtRisk += year.Amount;
                FeeAtRisk.TotalDealFinancialsYearList.Add(year);
            }
            FeeAtRisk.TotalLocal = totalFeesAtRisk;
            FeeAtRisk.TotalUSD = totalFeesAtRisk * tdfDBResult.LocalToUSDCurrencyFactor;

            return FeeAtRisk;
        }

        private TotalDealFinancialsDTO CalculateTDFNetRevenue(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalNetRevenue = 0;
            TotalDealFinancialsDTO NetRevenue = new TotalDealFinancialsDTO();
            NetRevenue.DescriptionId = 6;        // Net Revenue
            NetRevenue.RowSectionId = 3;
            NetRevenue.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].NetRevenue;
                totalNetRevenue += year.Amount;
                NetRevenue.TotalDealFinancialsYearList.Add(year);
            }
            NetRevenue.TotalLocal = totalNetRevenue;
            NetRevenue.TotalUSD = totalNetRevenue * tdfDBResult.LocalToUSDCurrencyFactor;

            return NetRevenue;
        }

        private TotalDealFinancialsDTO CalculateTDFNetRevenueServiceFees(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalNetRevenueServiceFees = 0;
            TotalDealFinancialsDTO NetRevenueServiceFees = new TotalDealFinancialsDTO();
            NetRevenueServiceFees.DescriptionId = 7;        // Net Revenue Service Fees
            NetRevenueServiceFees.RowSectionId = 3;
            NetRevenueServiceFees.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].NetRevenueServiceFees;
                totalNetRevenueServiceFees += year.Amount;
                NetRevenueServiceFees.TotalDealFinancialsYearList.Add(year);
            }
            NetRevenueServiceFees.TotalLocal = totalNetRevenueServiceFees;
            NetRevenueServiceFees.TotalUSD = totalNetRevenueServiceFees * tdfDBResult.LocalToUSDCurrencyFactor;

            return NetRevenueServiceFees;
        }

        private TotalDealFinancialsDTO CalculateTDFNetRevenueReimbursableExpenses(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalNetRevenueReimbursableExpenses = 0;
            TotalDealFinancialsDTO NetRevenueReimbursableExpenses = new TotalDealFinancialsDTO();
            NetRevenueReimbursableExpenses.DescriptionId = 8;        // Net Revenue Reimbursable Expenses
            NetRevenueReimbursableExpenses.RowSectionId = 3;
            NetRevenueReimbursableExpenses.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].NetRevenueReimbursableExp;
                totalNetRevenueReimbursableExpenses += year.Amount;
                NetRevenueReimbursableExpenses.TotalDealFinancialsYearList.Add(year);
            }
            NetRevenueReimbursableExpenses.TotalLocal = totalNetRevenueReimbursableExpenses;
            NetRevenueReimbursableExpenses.TotalUSD = totalNetRevenueReimbursableExpenses * tdfDBResult.LocalToUSDCurrencyFactor;

            return NetRevenueReimbursableExpenses;
        }

        private TotalDealFinancialsDTO CalculateTDFNetRevenuePartnerFees(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalNetRevenuePartnerFees = 0;
            TotalDealFinancialsDTO NetRevenuePartnerFees = new TotalDealFinancialsDTO();
            NetRevenuePartnerFees.DescriptionId = 9;        // Net Revenue Partner Fees
            NetRevenuePartnerFees.RowSectionId = 3;
            NetRevenuePartnerFees.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].NetRevenuePartnerFees;
                totalNetRevenuePartnerFees += year.Amount;
                NetRevenuePartnerFees.TotalDealFinancialsYearList.Add(year);
            }
            NetRevenuePartnerFees.TotalLocal = totalNetRevenuePartnerFees;
            NetRevenuePartnerFees.TotalUSD = totalNetRevenuePartnerFees * tdfDBResult.LocalToUSDCurrencyFactor;

            return NetRevenuePartnerFees;
        }

        private TotalDealFinancialsDTO CalculateTDFTotalProjectCosts(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalProjectCosts = 0;
            TotalDealFinancialsDTO TotalProjectCosts = new TotalDealFinancialsDTO();
            TotalProjectCosts.DescriptionId = 10;        // Total Project Cost
            TotalProjectCosts.RowSectionId = 4;
            TotalProjectCosts.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].TotalProjectCost;
                totalProjectCosts += year.Amount;
                TotalProjectCosts.TotalDealFinancialsYearList.Add(year);
            }
            TotalProjectCosts.TotalLocal = totalProjectCosts;
            TotalProjectCosts.TotalUSD = totalProjectCosts * tdfDBResult.LocalToUSDCurrencyFactor;

            return TotalProjectCosts;
        }

        private TotalDealFinancialsDTO CalculateTDFTotalProjectResourceCost(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalProjectResourceCost = 0;
            TotalDealFinancialsDTO TotalProjectResourceCost = new TotalDealFinancialsDTO();
            TotalProjectResourceCost.DescriptionId = 11;        // Total Project Resource Cost
            TotalProjectResourceCost.RowSectionId = 4;
            TotalProjectResourceCost.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].ResourceCost;
                totalProjectResourceCost += year.Amount;
                TotalProjectResourceCost.TotalDealFinancialsYearList.Add(year);
            }
            TotalProjectResourceCost.TotalLocal = totalProjectResourceCost;
            TotalProjectResourceCost.TotalUSD = totalProjectResourceCost * tdfDBResult.LocalToUSDCurrencyFactor;

            return TotalProjectResourceCost;
        }

        private TotalDealFinancialsDTO CalculateTDFTotalProjectDirectExpenseCost(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalProjectDirectExpenseCost = 0;
            TotalDealFinancialsDTO TotalProjectDirectExpenseCost = new TotalDealFinancialsDTO();
            TotalProjectDirectExpenseCost.DescriptionId = 12;        // Total Project Direct Expense Cost
            TotalProjectDirectExpenseCost.RowSectionId = 4;
            TotalProjectDirectExpenseCost.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].DirectExpenseCost;
                totalProjectDirectExpenseCost += year.Amount;
                TotalProjectDirectExpenseCost.TotalDealFinancialsYearList.Add(year);
            }
            TotalProjectDirectExpenseCost.TotalLocal = totalProjectDirectExpenseCost;
            TotalProjectDirectExpenseCost.TotalUSD = totalProjectDirectExpenseCost * tdfDBResult.LocalToUSDCurrencyFactor;

            return TotalProjectDirectExpenseCost;
        }

        private TotalDealFinancialsDTO CalculateTDFTotalProjectPartnerCosts(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalProjectPartnerCosts = 0;
            TotalDealFinancialsDTO TotalProjectPartnerCosts = new TotalDealFinancialsDTO();
            TotalProjectPartnerCosts.DescriptionId = 13;        // Total Project Partner Costs
            TotalProjectPartnerCosts.RowSectionId = 4;
            TotalProjectPartnerCosts.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].PartnerCost;
                totalProjectPartnerCosts += year.Amount;
                TotalProjectPartnerCosts.TotalDealFinancialsYearList.Add(year);
            }
            TotalProjectPartnerCosts.TotalLocal = totalProjectPartnerCosts;
            TotalProjectPartnerCosts.TotalUSD = totalProjectPartnerCosts * tdfDBResult.LocalToUSDCurrencyFactor;

            return TotalProjectPartnerCosts;
        }

        private TotalDealFinancialsDTO CalculateTDFGrossMargin(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalGrossMargin = 0;
            TotalDealFinancialsDTO TotalGrossMargin = new TotalDealFinancialsDTO();
            TotalGrossMargin.DescriptionId = 14;        // Gross Margin
            TotalGrossMargin.RowSectionId = 5;
            TotalGrossMargin.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].GrossMargin;
                totalGrossMargin += year.Amount;
                TotalGrossMargin.TotalDealFinancialsYearList.Add(year);
            }
            TotalGrossMargin.TotalLocal = totalGrossMargin;
            TotalGrossMargin.TotalUSD = totalGrossMargin * tdfDBResult.LocalToUSDCurrencyFactor;

            return TotalGrossMargin;
        }

        private TotalDealFinancialsDTO CalculateTDFGrossMarginPercent(TotalDealFinancialsDBResultDTO tdfDBResult,
            IList<TotalDealFinancialsDTO> totalDealFinancials)
        {
            TotalDealFinancialsDTO GrossMarginPercent = new TotalDealFinancialsDTO();
            GrossMarginPercent.DescriptionId = 15;        // Gross Margin Percent
            GrossMarginPercent.RowSectionId = 6;
            GrossMarginPercent.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                if (totalDealFinancials[5].TotalDealFinancialsYearList[i].Amount == 0)
                {
                    year.Amount = 0;
                }
                else
                {
                    year.Amount = ((totalDealFinancials[13].TotalDealFinancialsYearList[i].Amount / (totalDealFinancials[5].TotalDealFinancialsYearList[i].Amount)) * 100);
                }
                GrossMarginPercent.TotalDealFinancialsYearList.Add(year);
            }
            if (totalDealFinancials[5].TotalLocal == 0)
            {
                GrossMarginPercent.TotalLocal = 0;
                GrossMarginPercent.TotalUSD = 0;
            }
            else
            {
                GrossMarginPercent.TotalLocal = ((totalDealFinancials[13].TotalLocal / (totalDealFinancials[5].TotalLocal)) * 100);
                GrossMarginPercent.TotalUSD = ((totalDealFinancials[13].TotalUSD / (totalDealFinancials[5].TotalUSD)) * 100);
            }

            return GrossMarginPercent;
        }

        private TotalDealFinancialsDTO CalculateTDFPartnerMarginPercent(TotalDealFinancialsDBResultDTO tdfDBResult,
           IList<TotalDealFinancialsDTO> totalDealFinancials)
        {
            TotalDealFinancialsDTO PartnerMarginPercent = new TotalDealFinancialsDTO();
            PartnerMarginPercent.DescriptionId = 16;        // Partner Margin Percent
            PartnerMarginPercent.RowSectionId = 7;
            PartnerMarginPercent.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                if (totalDealFinancials[8].TotalDealFinancialsYearList[i].Amount == 0)
                {
                    year.Amount = 0;
                }
                else
                {
                    year.Amount = (1 - (totalDealFinancials[12].TotalDealFinancialsYearList[i].Amount /
                    (totalDealFinancials[8].TotalDealFinancialsYearList[i].Amount))) * 100;
                }
                PartnerMarginPercent.TotalDealFinancialsYearList.Add(year);
            }

            if (totalDealFinancials[8].TotalLocal == 0)
            {
                PartnerMarginPercent.TotalLocal = 0;
                PartnerMarginPercent.TotalUSD = 0;
            }
            else
            {
                PartnerMarginPercent.TotalLocal = (1 - (totalDealFinancials[12].TotalLocal / (totalDealFinancials[8].TotalLocal))) * 100;
                PartnerMarginPercent.TotalUSD = (1 - (totalDealFinancials[12].TotalUSD / (totalDealFinancials[8].TotalUSD))) * 100;
            }

            return PartnerMarginPercent;
        }

        private TotalDealFinancialsDTO CalculateTDFSGACost(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalSGACost = 0;
            TotalDealFinancialsDTO SGACost = new TotalDealFinancialsDTO();
            SGACost.DescriptionId = 17;        // SGA Cost
            SGACost.RowSectionId = 8;
            SGACost.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].SGACost;
                totalSGACost += year.Amount;
                SGACost.TotalDealFinancialsYearList.Add(year);
            }
            SGACost.TotalLocal = totalSGACost;
            SGACost.TotalUSD = totalSGACost * tdfDBResult.LocalToUSDCurrencyFactor;

            return SGACost;
        }

        private TotalDealFinancialsDTO CalculateTDFEbitda(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            decimal totalEbitda = 0;
            TotalDealFinancialsDTO Ebitda = new TotalDealFinancialsDTO();
            Ebitda.DescriptionId = 18;        // Ebitda
            Ebitda.RowSectionId = 9;
            Ebitda.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = tdfDBResult.DealFinancialsYearTotalDTO[i].Ebitda;
                totalEbitda += year.Amount;
                Ebitda.TotalDealFinancialsYearList.Add(year);
            }
            Ebitda.TotalLocal = totalEbitda;
            Ebitda.TotalUSD = totalEbitda * tdfDBResult.LocalToUSDCurrencyFactor;

            return Ebitda;
        }

        private TotalDealFinancialsDTO CalculateTDFProjectEbitdaPercent(TotalDealFinancialsDBResultDTO tdfDBResult,
          IList<TotalDealFinancialsDTO> totalDealFinancials)
        {
            TotalDealFinancialsDTO ProjectEbitdaPercent = new TotalDealFinancialsDTO();
            ProjectEbitdaPercent.DescriptionId = 19;        // Project Ebitda Percent
            ProjectEbitdaPercent.RowSectionId = 10;
            ProjectEbitdaPercent.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                if (totalDealFinancials[5].TotalDealFinancialsYearList[i].Amount == 0)
                {
                    year.Amount = 0;
                }
                else
                {
                    year.Amount = (totalDealFinancials[17].TotalDealFinancialsYearList[i].Amount /
                    (totalDealFinancials[5].TotalDealFinancialsYearList[i].Amount)) * 100;
                }

                ProjectEbitdaPercent.TotalDealFinancialsYearList.Add(year);
            }

            if (totalDealFinancials[5].TotalLocal == 0)
            {
                ProjectEbitdaPercent.TotalLocal = 0;
                ProjectEbitdaPercent.TotalUSD = 0;
            }
            else
            {
                ProjectEbitdaPercent.TotalLocal = (totalDealFinancials[17].TotalLocal / (totalDealFinancials[5].TotalLocal)) * 100;
                ProjectEbitdaPercent.TotalUSD = (((totalDealFinancials[17].TotalUSD / ((totalDealFinancials[5].TotalUSD) == 0 ? 1 : (totalDealFinancials[5].TotalUSD)))) * 100);
            }

            return ProjectEbitdaPercent;
        }

        private TotalDealFinancialsDTO CalculateTDFCorporateVerticalOverheadPercent(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            TotalDealFinancialsDTO CorporateVerticalOverheadPercent = new TotalDealFinancialsDTO();
            CorporateVerticalOverheadPercent.DescriptionId = 20;        // Corporate / Vertical Overhead Percent
            CorporateVerticalOverheadPercent.RowSectionId = 10;
            CorporateVerticalOverheadPercent.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = Convert.ToDecimal(tdfDBResult.DealFinancialsYearTotalDTO[i].CorporateVerticalOverheadPercent);
                CorporateVerticalOverheadPercent.TotalDealFinancialsYearList.Add(year);
            }
            CorporateVerticalOverheadPercent.TotalLocal = Convert.ToDecimal(tdfDBResult.DealFinancialsYearTotalDTO[0].CorporateVerticalOverheadPercent);
            CorporateVerticalOverheadPercent.TotalUSD = Convert.ToDecimal(tdfDBResult.DealFinancialsYearTotalDTO[0].CorporateVerticalOverheadPercent);

            return CorporateVerticalOverheadPercent;
        }

        private TotalDealFinancialsDTO CalculateTDFCorporateEbitdaPercent(TotalDealFinancialsDBResultDTO tdfDBResult,
           IList<TotalDealFinancialsDTO> totalDealFinancials)
        {
            TotalDealFinancialsDTO CorporateEbitdaPercent = new TotalDealFinancialsDTO();
            CorporateEbitdaPercent.DescriptionId = 21;        // Corporate Ebitda Percent
            CorporateEbitdaPercent.RowSectionId = 11;
            CorporateEbitdaPercent.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = (totalDealFinancials[18].TotalDealFinancialsYearList[i].Amount -
                    totalDealFinancials[19].TotalDealFinancialsYearList[i].Amount);
                CorporateEbitdaPercent.TotalDealFinancialsYearList.Add(year);
            }
            CorporateEbitdaPercent.TotalLocal = (totalDealFinancials[18].TotalLocal - totalDealFinancials[19].TotalLocal);
            CorporateEbitdaPercent.TotalUSD = (totalDealFinancials[18].TotalUSD - totalDealFinancials[19].TotalUSD);

            return CorporateEbitdaPercent;
        }

        private TotalDealFinancialsDTO CalculateTDFBeatTaxImpactPercent(TotalDealFinancialsDBResultDTO tdfDBResult)
        {
            TotalDealFinancialsDTO beatTaxImpactPercent = new TotalDealFinancialsDTO();
            beatTaxImpactPercent.DescriptionId = 22;        // Corporate Ebitda Percent
            beatTaxImpactPercent.RowSectionId = 12;
            beatTaxImpactPercent.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                if (tdfDBResult.BeatTaxYearDTO.Count > 0)
                {
                    year.Amount = tdfDBResult.BeatTaxYearDTO[i].BeatTaxImpactPercent;
                }
                else
                {
                    year.Amount = 0;
                }

                beatTaxImpactPercent.TotalDealFinancialsYearList.Add(year);
            }
            beatTaxImpactPercent.TotalLocal = tdfDBResult.TotalBeatTaxImpactPercent;
            beatTaxImpactPercent.TotalUSD = tdfDBResult.TotalBeatTaxImpactPercent;

            return beatTaxImpactPercent;
        }

        private TotalDealFinancialsDTO CalculateContributionPercent(TotalDealFinancialsDBResultDTO tdfDBResult,
         IList<TotalDealFinancialsDTO> totalDealFinancials)
        {
            TotalDealFinancialsDTO ContributionPercent = new TotalDealFinancialsDTO();
            ContributionPercent.DescriptionId = 23;        // Contribution Percent
            ContributionPercent.RowSectionId = 13;
            ContributionPercent.TotalDealFinancialsYearList = new List<TotalDealFinancialsYearDTO>();

            for (int i = 0; i < tdfDBResult.DealFinancialsYearTotalDTO.Count; i++)
            {
                TotalDealFinancialsYearDTO year = new TotalDealFinancialsYearDTO();
                year.YearId = tdfDBResult.DealFinancialsYearTotalDTO[i].YearId;
                year.Year = tdfDBResult.DealFinancialsYearTotalDTO[i].Year;
                year.Amount = (totalDealFinancials[20].TotalDealFinancialsYearList[i].Amount -
                    totalDealFinancials[21].TotalDealFinancialsYearList[i].Amount);
                ContributionPercent.TotalDealFinancialsYearList.Add(year);
            }
            ContributionPercent.TotalLocal = (totalDealFinancials[20].TotalLocal - totalDealFinancials[21].TotalLocal);
            ContributionPercent.TotalUSD = (totalDealFinancials[20].TotalUSD - totalDealFinancials[21].TotalUSD);

            return ContributionPercent;
        }
    }
}
