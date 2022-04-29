using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ClientPriceService : IClientPriceService
    {
        private readonly IClientPriceRepository repository;

        public ClientPriceService(IClientPriceRepository repository, ICapitalChargeService capitalChargeService, ISharedRepository sharedRepository)
        {
            this.repository = repository;
        }

        public async Task<ClientPriceMainDTO> GetClientPriceData(int pipSheetId)
        {
            ClientPriceDBResultDTO clientPriceDBResultDTO = await this.repository.GetClientPriceData(pipSheetId);
            ClientPriceMainDTO clientPriceMainDTO = new ClientPriceMainDTO();
            ClientPriceParentDTO calculatedClientPrice = CalculateTotalClientPriceRow(clientPriceDBResultDTO, pipSheetId);
            clientPriceMainDTO.ClientPriceDTO = new List<ClientPriceParentDTO>();
            clientPriceMainDTO.PLForecastDTO = new List<PLForecastDTO>();
            clientPriceMainDTO.FeesAtRisk = (Convert.ToDecimal(-1 * clientPriceDBResultDTO.FeesAtRisk) / 100) * calculatedClientPrice.TotalPrice;
            clientPriceMainDTO.NetEstimatedRevenue = calculatedClientPrice.TotalPrice + clientPriceMainDTO.FeesAtRisk;

            clientPriceMainDTO.ClientPriceDTO.Add(calculatedClientPrice);

            clientPriceMainDTO = await CreateClientPriceObject(clientPriceDBResultDTO, clientPriceMainDTO);

            clientPriceMainDTO.ProjectPeriodDTO = clientPriceDBResultDTO.ProjectPeriodDTO;
            clientPriceMainDTO.IsFixedBid = Convert.ToBoolean(clientPriceDBResultDTO.IsFixedBid);
            clientPriceMainDTO.CurrencyId = clientPriceDBResultDTO.CurrencyId;

            clientPriceMainDTO.PLForecastDTO.Add(CalculateCostOfStaffing(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculateInflationOnly(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculateExpensesAndOverhead(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculateContingencyCost(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculatePartnerCost(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculateCapitalCharge(clientPriceDBResultDTO));

            return clientPriceMainDTO;
        }

        public async Task SaveClientPriceData(ClientPriceMainDTO clientPriceMainDTO, string userName)
        {
            int pipSheetId = clientPriceMainDTO.ClientPriceDTO[0].PipSheetId;
            ClientPriceDBResultDTO clientPriceDBResultDTO = await this.repository.GetClientPriceForCalculation(pipSheetId);
            IList<ProjectPeriodTotalDTO> projectPeriodTotalDTOs = CalculateFeesAtRiskAndEstimatedRevenue(clientPriceDBResultDTO, pipSheetId);
            IList<ClientPricePeriodDTO> clientPricePeriods = clientPriceMainDTO.ClientPriceDTO.SelectMany(x => x.ClientPricePeriodDTO).ToList();
            IList<ClientPriceDTO> clientPrice = clientPriceMainDTO.ClientPriceDTO.Select(x => new ClientPriceDTO
            {
                UId = x.UId,
                ClientPriceId = x.ClientPriceId,
                PipSheetId = x.PipSheetId,
                DescriptionId = x.DescriptionId,
                TotalPrice = x.TotalPrice,
                IsOverrideUpdated = x.IsOverrideUpdated,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            }).ToList();

            await this.repository.SaveClientPriceData(clientPricePeriods, clientPrice, projectPeriodTotalDTOs, userName);
        }

        private ClientPriceParentDTO CalculateTotalClientPriceRow(ClientPriceDBResultDTO clientPriceDBResultDTO, int pipSheetId)
        {
            ClientPriceParentDTO clientPriceParentDTO = new ClientPriceParentDTO();
            List<ClientPricePeriodDTO> clientPricePeriodListDTO = new List<ClientPricePeriodDTO>();

            clientPriceParentDTO.ClientPriceId = 0;
            clientPriceParentDTO.PipSheetId = pipSheetId;
            clientPriceParentDTO.DescriptionId = 0;
            clientPriceParentDTO.TotalPrice = Convert.ToDecimal(clientPriceDBResultDTO.CalculatedValueDTO == null ? 0 : clientPriceDBResultDTO.CalculatedValueDTO.TotalOtherPriceAdjustment ?? 0) + (clientPriceDBResultDTO.FixBidCalcDTO == null ? 0 : clientPriceDBResultDTO.FixBidCalcDTO.TotalCost);
            foreach (int billingPeriodId in clientPriceDBResultDTO.ProjectPeriodDTO.Select(x => x.BillingPeriodId))
            {
                ClientPricePeriodDTO clientPricePeriod = new ClientPricePeriodDTO();
                clientPricePeriod.BillingPeriodId = billingPeriodId;
                clientPricePeriod.ClientPriceId = 0;
                if (clientPriceDBResultDTO.FixBidCalcPeriodDTO != null)
                {
                    clientPricePeriod.Price = Convert.ToDecimal(clientPriceDBResultDTO.FixBidCalcPeriodDTO.Where(x => x.BillingPeriodId == billingPeriodId).Select(x => x.Cost).FirstOrDefault()) +
                    Convert.ToDecimal(clientPriceDBResultDTO.ProjectPeriodTotalDTO.Where(x => x.BillingPeriodId == billingPeriodId).Select(x => x.OtherPriceAdjustment).FirstOrDefault());
                }
                else
                {
                    clientPricePeriod.Price = 0;
                }

                clientPricePeriodListDTO.Add(clientPricePeriod);
            }
            clientPriceParentDTO.ClientPricePeriodDTO = clientPricePeriodListDTO;
            return clientPriceParentDTO;
        }

        private IList<ProjectPeriodTotalDTO> CalculateFeesAtRiskAndEstimatedRevenue(ClientPriceDBResultDTO clientPriceDBResultDTO, int pipSheetId)
        {
            //Calculate Client Price Period-wise and TotalClientPrice
            List<ProjectPeriodTotalDTO> projectPeriodTotalListDTO = new List<ProjectPeriodTotalDTO>();
            foreach (int billingPeriodId in clientPriceDBResultDTO.FixBidCalcPeriodDTO.Select(x => x.BillingPeriodId))
            {
                ProjectPeriodTotalDTO projectPeriodTotal = new ProjectPeriodTotalDTO();
                projectPeriodTotal.BillingPeriodId = billingPeriodId;
                projectPeriodTotal.PipSheetId = pipSheetId;
                projectPeriodTotal.ClientPrice = Convert.ToDecimal(clientPriceDBResultDTO.FixBidCalcPeriodDTO.Where(x => x.BillingPeriodId == billingPeriodId).Select(x => x.Cost).FirstOrDefault()) +
                    Convert.ToDecimal(clientPriceDBResultDTO.ProjectPeriodTotalDTO.Where(x => x.BillingPeriodId == billingPeriodId).Select(x => x.OtherPriceAdjustment).FirstOrDefault());
                projectPeriodTotal.FeesAtRisk = (Convert.ToDecimal(clientPriceDBResultDTO.FeesAtRisk) / 100) * projectPeriodTotal.ClientPrice;
                projectPeriodTotal.NetEstimatedRevenue = projectPeriodTotal.ClientPrice - projectPeriodTotal.FeesAtRisk;
                projectPeriodTotalListDTO.Add(projectPeriodTotal);
            }

            return projectPeriodTotalListDTO;
        }

        //To-DO : Calculation to be done after Inflation User Story is completed
        private PLForecastDTO CalculateCostOfStaffing(ClientPriceDBResultDTO clientPriceDBResultDTO)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 0,
                Total = clientPriceDBResultDTO.CalculatedValueDTO == null ? 0 : clientPriceDBResultDTO.CalculatedValueDTO.TotalCappedCost -
                        (from client in clientPriceDBResultDTO.ProjectPeriodTotalDTO select client.Inflation).Sum(),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };

            foreach (ProjectPeriodTotalDTO period in clientPriceDBResultDTO.ProjectPeriodTotalDTO)
            {
                PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                {
                    BillingPeriodId = period.BillingPeriodId,
                    Price = period.CappedCost - period.Inflation
                };
                pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
            }

            return pLForecastDTO;
        }

        private PLForecastDTO CalculateInflationOnly(ClientPriceDBResultDTO clientPriceDBResultDTO)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 1,
                Total = (from client in clientPriceDBResultDTO.ProjectPeriodTotalDTO select client.Inflation).Sum(),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };

            foreach (ProjectPeriodTotalDTO period in clientPriceDBResultDTO.ProjectPeriodTotalDTO)
            {
                PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                {
                    BillingPeriodId = period.BillingPeriodId,
                    Price = period.Inflation
                };
                pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
            }
            return pLForecastDTO;
        }

        private PLForecastDTO CalculateExpensesAndOverhead(ClientPriceDBResultDTO clientPriceDBResultDTO)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 2,
                Total = Convert.ToDecimal(clientPriceDBResultDTO.CalculatedValueDTO == null ? 0 : clientPriceDBResultDTO.CalculatedValueDTO.TotalDirectExpense),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            foreach (ProjectPeriodTotalDTO period in clientPriceDBResultDTO.ProjectPeriodTotalDTO)
            {
                PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                {
                    BillingPeriodId = period.BillingPeriodId,
                    Price = Convert.ToDecimal(period.AssetSubTotalExpense)
                };
                pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
            }
            return pLForecastDTO;
        }

        private PLForecastDTO CalculateContingencyCost(ClientPriceDBResultDTO clientPriceDBResultDTO)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 3,
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            //foreach (ProjectPeriodTotalDTO period in clientPriceDBResultDTO.ProjectPeriodTotalDTO)
            for (int i = 0; i < clientPriceDBResultDTO.ProjectPeriodTotalDTO.Count; i++)
            {
                PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO();
                pLForecastPeriod.BillingPeriodId = clientPriceDBResultDTO.ProjectPeriodTotalDTO[i].BillingPeriodId;

                if (clientPriceDBResultDTO.RiskManagementDTO != null)
                {
                    if (clientPriceDBResultDTO.RiskManagementDTO.IsOverride)
                    {
                        pLForecastPeriod.Price = Convert.ToDecimal(clientPriceDBResultDTO.RiskManagementPeriodDTO[i].RiskAmount ?? 0);
                    }
                    else
                    {
                        if (clientPriceDBResultDTO.CalculatedValueDTO.TotalCappedCost != 0 && clientPriceDBResultDTO.CalculatedValueDTO.TotalCappedCost != null)
                        {
                            pLForecastPeriod.Price = Convert.ToDecimal(clientPriceDBResultDTO.RiskManagementDTO.TotalAssesedRiskOverrun) *
                           (Convert.ToDecimal(clientPriceDBResultDTO.ProjectPeriodTotalDTO[i].CappedCost) / Convert.ToDecimal(clientPriceDBResultDTO.CalculatedValueDTO.TotalCappedCost == 0
                           ? 1 : clientPriceDBResultDTO.CalculatedValueDTO.TotalCappedCost));
                        }
                        else
                        {
                            pLForecastPeriod.Price = Convert.ToDecimal(clientPriceDBResultDTO.RiskManagementDTO.TotalAssesedRiskOverrun) / clientPriceDBResultDTO.ProjectPeriodTotalDTO.Count;
                        }

                    }
                }
                else
                {
                    pLForecastPeriod.Price = 0;
                }
                pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);

            }
            pLForecastDTO.Total = (from period in pLForecastDTO.PLForecastPeriodDTO select period.Price).Sum();
            return pLForecastDTO;
        }

        private PLForecastDTO CalculatePartnerCost(ClientPriceDBResultDTO clientPriceDBResultDTO)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 4,
                Total = Convert.ToDecimal(clientPriceDBResultDTO.CalculatedValueDTO == null ? 0 : clientPriceDBResultDTO.CalculatedValueDTO.TotalPartnerCost),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            foreach (ProjectPeriodTotalDTO period in clientPriceDBResultDTO.ProjectPeriodTotalDTO)
            {
                PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                {
                    BillingPeriodId = period.BillingPeriodId,
                    Price = Convert.ToDecimal(period.PartnerCost)
                };
                pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
            }
            return pLForecastDTO;
        }

        private PLForecastDTO CalculateCapitalCharge(ClientPriceDBResultDTO clientPriceDBResultDTO)
        {
            PLForecastDTO pLForecastDTO = new PLForecastDTO
            {
                DescriptionId = 5,
                Total = Convert.ToDecimal(clientPriceDBResultDTO.CapitalCharge == null ? 0 : clientPriceDBResultDTO.CapitalCharge),
                PLForecastPeriodDTO = new List<PLForecastPeriodDTO>()
            };
            foreach (ProjectPeriodTotalDTO period in clientPriceDBResultDTO.ProjectPeriodTotalDTO)
            {
                PLForecastPeriodDTO pLForecastPeriod = new PLForecastPeriodDTO
                {
                    BillingPeriodId = period.BillingPeriodId,
                    Price = Convert.ToDecimal(period.CapitalCharge)
                };
                pLForecastDTO.PLForecastPeriodDTO.Add(pLForecastPeriod);
            }
            return pLForecastDTO;
        }

        // To recalculate Client Price Screen when called from other screen
        public async Task<ClientPriceMainDTO> CalculateTotalClientPrice(int pipSheetId, string userName)
        {
            ClientPriceDBResultDTO clientPriceDBResultDTO = await this.repository.GetClientPriceData(pipSheetId);
            ClientPriceMainDTO clientPriceMainDTO = new ClientPriceMainDTO();
            ClientPriceParentDTO calculatedClientPrice = CalculateTotalClientPriceRow(clientPriceDBResultDTO, pipSheetId);
            clientPriceMainDTO.ClientPriceDTO = new List<ClientPriceParentDTO>();
            clientPriceMainDTO.PLForecastDTO = new List<PLForecastDTO>();

            clientPriceMainDTO.ClientPriceDTO.Add(calculatedClientPrice);

            clientPriceMainDTO = await CreateClientPriceObject(clientPriceDBResultDTO, clientPriceMainDTO);

            clientPriceMainDTO.ProjectPeriodDTO = clientPriceDBResultDTO.ProjectPeriodDTO;
            clientPriceMainDTO.IsFixedBid = Convert.ToBoolean(clientPriceDBResultDTO.IsFixedBid);

            // Project Cost components addition
            clientPriceMainDTO.PLForecastDTO.Add(CalculateCostOfStaffing(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculateInflationOnly(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculateExpensesAndOverhead(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculateContingencyCost(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculatePartnerCost(clientPriceDBResultDTO));
            clientPriceMainDTO.PLForecastDTO.Add(CalculateCapitalCharge(clientPriceDBResultDTO));

            // Re-calculating Client Price : Invoicing Schedule
            if (clientPriceMainDTO.ClientPriceDTO.Count > 1)
            {
                decimal? overridenPrice = 0;
                foreach (var totalOverridenPrice in clientPriceMainDTO.ClientPriceDTO[1].ClientPricePeriodDTO)
                {
                    overridenPrice += totalOverridenPrice.Price ?? 0;
                }

                if (clientPriceMainDTO.ClientPriceDTO[1].TotalPrice != null && (clientPriceMainDTO.ClientPriceDTO[1].TotalPrice > 0 || overridenPrice > 0))
                {
                    // Calculate User Input Invoicing Schedule Row
                    if (Math.Abs(Math.Round(calculatedClientPrice.TotalPrice ?? 0, 2) - Math.Round(overridenPrice ?? 0, 2)) > 1)
                    {
                        clientPriceMainDTO.ClientPriceDTO[1].IsOverrideUpdated = true;
                        clientPriceMainDTO.ClientPriceDTO[1].TotalPrice = calculatedClientPrice.TotalPrice;
                    }
                    else
                    {
                        clientPriceMainDTO.ClientPriceDTO[1].IsOverrideUpdated = false;
                    }

                    // Calculate Total Invoiced Row
                    clientPriceMainDTO.ClientPriceDTO[2].TotalPrice = calculatedClientPrice.TotalPrice;
                }
                else
                {
                    clientPriceMainDTO.ClientPriceDTO[1].IsOverrideUpdated = false;
                    // Calculate Total Invoiced Row                    
                    clientPriceMainDTO.ClientPriceDTO[2].TotalPrice = calculatedClientPrice.TotalPrice;
                    clientPriceMainDTO.ClientPriceDTO[2].ClientPricePeriodDTO = (from cp in calculatedClientPrice.ClientPricePeriodDTO
                                                                                 join ti in clientPriceMainDTO.ClientPriceDTO[2].ClientPricePeriodDTO
                                                                                 on cp.BillingPeriodId equals ti.BillingPeriodId
                                                                                 select new ClientPricePeriodDTO
                                                                                 {
                                                                                     UId = 0,
                                                                                     ClientPriceId = ti.ClientPriceId,
                                                                                     BillingPeriodId = ti.BillingPeriodId,
                                                                                     Price = cp.Price
                                                                                 }).ToList();
                }

                // Calculate Total Project Cost
                clientPriceMainDTO.ClientPriceDTO[3].TotalPrice = clientPriceMainDTO.PLForecastDTO[0].Total     // Cost of Staffing
                                                                + clientPriceMainDTO.PLForecastDTO[1].Total     // Inflation in Cost
                                                                + clientPriceMainDTO.PLForecastDTO[2].Total     // Expenses and Overhead
                                                                + clientPriceMainDTO.PLForecastDTO[3].Total     // Cost Contingency
                                                                + clientPriceMainDTO.PLForecastDTO[4].Total     // Partner Cost
                                                                + clientPriceMainDTO.PLForecastDTO[5].Total;    // Capital Charges

                // Calculate Total Net Cashflow
                clientPriceMainDTO.ClientPriceDTO[4].TotalPrice = clientPriceMainDTO.ClientPriceDTO[2].TotalPrice - clientPriceMainDTO.ClientPriceDTO[3].TotalPrice;

                // Calculate Cumulative Cashflow
                clientPriceMainDTO.ClientPriceDTO[5].TotalPrice = 0;

                // Reassign UId to Client Price Object
                clientPriceMainDTO.ClientPriceDTO[0].UId = 0;
                clientPriceMainDTO.ClientPriceDTO[1].UId = 1;
                clientPriceMainDTO.ClientPriceDTO[2].UId = 2;
                clientPriceMainDTO.ClientPriceDTO[3].UId = 3;
                clientPriceMainDTO.ClientPriceDTO[4].UId = 4;
                clientPriceMainDTO.ClientPriceDTO[5].UId = 5;

                // Period-Wise Combined calculations
                for (int i = 0; i < clientPriceMainDTO.ClientPriceDTO[3].ClientPricePeriodDTO.Count; i++)
                {
                    // Assigning UId to Period Wise Client Price objects
                    clientPriceMainDTO.ClientPriceDTO[0].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[0].UId;
                    clientPriceMainDTO.ClientPriceDTO[1].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[1].UId;
                    clientPriceMainDTO.ClientPriceDTO[2].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[2].UId;
                    clientPriceMainDTO.ClientPriceDTO[3].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[3].UId;
                    clientPriceMainDTO.ClientPriceDTO[4].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[4].UId;
                    clientPriceMainDTO.ClientPriceDTO[5].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[5].UId;

                    // Calculate Period-Wise Project Cost
                    clientPriceMainDTO.ClientPriceDTO[3].ClientPricePeriodDTO[i].Price = clientPriceMainDTO.PLForecastDTO[0].PLForecastPeriodDTO[i].Price   // Cost of Staffing
                                                                + clientPriceMainDTO.PLForecastDTO[1].PLForecastPeriodDTO[i].Price  // Inflation in Cost
                                                                + clientPriceMainDTO.PLForecastDTO[2].PLForecastPeriodDTO[i].Price  // Expenses and Overhead
                                                                + clientPriceMainDTO.PLForecastDTO[3].PLForecastPeriodDTO[i].Price  // Cost Contingency
                                                                + clientPriceMainDTO.PLForecastDTO[4].PLForecastPeriodDTO[i].Price  // Partner Cost
                                                                + clientPriceMainDTO.PLForecastDTO[5].PLForecastPeriodDTO[i].Price; // Capital Charges

                    // Calculate Period-Wise Net Cashflow
                    clientPriceMainDTO.ClientPriceDTO[4].ClientPricePeriodDTO[i].Price = clientPriceMainDTO.ClientPriceDTO[2].ClientPricePeriodDTO[i].Price
                                                                - clientPriceMainDTO.ClientPriceDTO[3].ClientPricePeriodDTO[i].Price;

                    //Calculate Period-Wise Cumulative Cashflow
                    if (i == 0)
                    {
                        clientPriceMainDTO.ClientPriceDTO[5].ClientPricePeriodDTO[i].Price = clientPriceMainDTO.ClientPriceDTO[4].ClientPricePeriodDTO[i].Price;
                    }
                    else
                    {
                        clientPriceMainDTO.ClientPriceDTO[5].ClientPricePeriodDTO[i].Price = clientPriceMainDTO.ClientPriceDTO[5].ClientPricePeriodDTO[i - 1].Price
                                                                + clientPriceMainDTO.ClientPriceDTO[4].ClientPricePeriodDTO[i].Price;
                    }
                }

                // Calculating Fees at Risk Revenue and Net Estimated Revenue
                clientPriceMainDTO.FeesAtRisk = (Convert.ToDecimal(-1 * clientPriceDBResultDTO.FeesAtRisk) / 100) * calculatedClientPrice.TotalPrice;
                clientPriceMainDTO.NetEstimatedRevenue = calculatedClientPrice.TotalPrice + clientPriceMainDTO.FeesAtRisk;

                return clientPriceMainDTO;
            }
            else       // Calculate First Time on Resource Planning Click
            {
                return CalculateInvoivingScheduleOnFirstTimeSave(clientPriceMainDTO, calculatedClientPrice, clientPriceDBResultDTO);
            }
        }

        private ClientPriceMainDTO CalculateInvoivingScheduleOnFirstTimeSave(ClientPriceMainDTO clientPriceMainDTO, ClientPriceParentDTO calculatedClientPrice, ClientPriceDBResultDTO clientPriceDBResultDTO)
        {
            int noOfRowsToBeAdded = 5;

            // Creating Dummy Objects for First Time Save Click
            for (int i = 1; i <= noOfRowsToBeAdded; i++)
            {
                clientPriceMainDTO.ClientPriceDTO.Add(new ClientPriceParentDTO());
                clientPriceMainDTO.ClientPriceDTO[i].DescriptionId = i;
                clientPriceMainDTO.ClientPriceDTO[i].PipSheetId = clientPriceMainDTO.ClientPriceDTO[0].PipSheetId;
                clientPriceMainDTO.ClientPriceDTO[i].TotalPrice = 0;
                clientPriceMainDTO.ClientPriceDTO[i].ClientPricePeriodDTO = new List<ClientPricePeriodDTO>();
                clientPriceMainDTO.ClientPriceDTO[i].CreatedOn = new DateTime();
                clientPriceMainDTO.ClientPriceDTO[i].UpdatedOn = new DateTime();

                for (int j = 0; j < clientPriceMainDTO.ProjectPeriodDTO.Count; j++)
                {
                    ClientPricePeriodDTO clientPricePeriod = new ClientPricePeriodDTO()
                    {
                        BillingPeriodId = clientPriceMainDTO.ProjectPeriodDTO[j].BillingPeriodId,
                        Price = 0
                    };
                    clientPriceMainDTO.ClientPriceDTO[i].ClientPricePeriodDTO.Add(clientPricePeriod);
                }
            }

            // Calculate Total Invoiced Row
            clientPriceMainDTO.ClientPriceDTO[2].TotalPrice = calculatedClientPrice.TotalPrice;
            clientPriceMainDTO.ClientPriceDTO[2].ClientPricePeriodDTO = (from cp in calculatedClientPrice.ClientPricePeriodDTO
                                                                         join ti in clientPriceMainDTO.ClientPriceDTO[2].ClientPricePeriodDTO
                                                                         on cp.BillingPeriodId equals ti.BillingPeriodId
                                                                         select new ClientPricePeriodDTO
                                                                         {
                                                                             UId = 0,
                                                                             ClientPriceId = ti.ClientPriceId,
                                                                             BillingPeriodId = ti.BillingPeriodId,
                                                                             Price = cp.Price
                                                                         }).ToList();

            // Calculate Total Project Cost
            clientPriceMainDTO.ClientPriceDTO[3].TotalPrice = clientPriceMainDTO.PLForecastDTO[0].Total     // Cost of Staffing
                                                            + clientPriceMainDTO.PLForecastDTO[1].Total     // Inflation in Cost
                                                            + clientPriceMainDTO.PLForecastDTO[2].Total     // Expenses and Overhead
                                                            + clientPriceMainDTO.PLForecastDTO[3].Total     // Cost Contingency
                                                            + clientPriceMainDTO.PLForecastDTO[4].Total     // Partner Cost
                                                            + clientPriceMainDTO.PLForecastDTO[5].Total;    // Capital Charges

            // Calculate Total Net Cashflow
            clientPriceMainDTO.ClientPriceDTO[4].TotalPrice = clientPriceMainDTO.ClientPriceDTO[2].TotalPrice - clientPriceMainDTO.ClientPriceDTO[3].TotalPrice;

            // Calculate Cumulative Cashflow
            clientPriceMainDTO.ClientPriceDTO[5].TotalPrice = 0;

            // Reassign UId to Client Price Object
            clientPriceMainDTO.ClientPriceDTO[0].UId = 0;
            clientPriceMainDTO.ClientPriceDTO[1].UId = 1;
            clientPriceMainDTO.ClientPriceDTO[2].UId = 2;
            clientPriceMainDTO.ClientPriceDTO[3].UId = 3;
            clientPriceMainDTO.ClientPriceDTO[4].UId = 4;
            clientPriceMainDTO.ClientPriceDTO[5].UId = 5;

            // Period-Wise Combined calculations
            for (int i = 0; i < clientPriceMainDTO.ClientPriceDTO[3].ClientPricePeriodDTO.Count; i++)
            {
                // Assigning UId to Period Wise Client Price objects
                clientPriceMainDTO.ClientPriceDTO[0].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[0].UId;
                clientPriceMainDTO.ClientPriceDTO[1].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[1].UId;
                clientPriceMainDTO.ClientPriceDTO[2].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[2].UId;
                clientPriceMainDTO.ClientPriceDTO[3].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[3].UId;
                clientPriceMainDTO.ClientPriceDTO[4].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[4].UId;
                clientPriceMainDTO.ClientPriceDTO[5].ClientPricePeriodDTO[i].UId = clientPriceMainDTO.ClientPriceDTO[5].UId;

                // Calculate Period-Wise Project Cost
                clientPriceMainDTO.ClientPriceDTO[3].ClientPricePeriodDTO[i].Price = clientPriceMainDTO.PLForecastDTO[0].PLForecastPeriodDTO[i].Price   // Cost of Staffing
                                                            + clientPriceMainDTO.PLForecastDTO[1].PLForecastPeriodDTO[i].Price  // Inflation in Cost
                                                            + clientPriceMainDTO.PLForecastDTO[2].PLForecastPeriodDTO[i].Price  // Expenses and Overhead
                                                            + clientPriceMainDTO.PLForecastDTO[3].PLForecastPeriodDTO[i].Price  // Cost Contingency
                                                            + clientPriceMainDTO.PLForecastDTO[4].PLForecastPeriodDTO[i].Price  // Partner Cost
                                                            + clientPriceMainDTO.PLForecastDTO[5].PLForecastPeriodDTO[i].Price; // Capital Charges

                // Calculate Period-Wise Net Cashflow
                clientPriceMainDTO.ClientPriceDTO[4].ClientPricePeriodDTO[i].Price = clientPriceMainDTO.ClientPriceDTO[2].ClientPricePeriodDTO[i].Price
                                                            - clientPriceMainDTO.ClientPriceDTO[3].ClientPricePeriodDTO[i].Price;

                //Calculate Period-Wise Cumulative Cashflow
                if (i == 0)
                {
                    clientPriceMainDTO.ClientPriceDTO[5].ClientPricePeriodDTO[i].Price = clientPriceMainDTO.ClientPriceDTO[4].ClientPricePeriodDTO[i].Price;
                }
                else
                {
                    clientPriceMainDTO.ClientPriceDTO[5].ClientPricePeriodDTO[i].Price = clientPriceMainDTO.ClientPriceDTO[5].ClientPricePeriodDTO[i - 1].Price
                                                            + clientPriceMainDTO.ClientPriceDTO[4].ClientPricePeriodDTO[i].Price;
                }
            }

            // Calculating Fees at Risk Revenue and Net Estimated Revenue
            clientPriceMainDTO.FeesAtRisk = (Convert.ToDecimal(-1 * clientPriceDBResultDTO.FeesAtRisk) / 100) * calculatedClientPrice.TotalPrice;
            clientPriceMainDTO.NetEstimatedRevenue = calculatedClientPrice.TotalPrice + clientPriceMainDTO.FeesAtRisk;

            return clientPriceMainDTO;
        }

        public async Task<ClientPriceMainDTO> CreateClientPriceObject(ClientPriceDBResultDTO clientPriceDBResultDTO, ClientPriceMainDTO clientPriceMainDTO)
        {
            foreach (ClientPriceDTO clientPriceDTO in clientPriceDBResultDTO.ClientPriceDTO)
            {
                ClientPriceParentDTO clientPriceParentDTO = new ClientPriceParentDTO();
                clientPriceParentDTO.ClientPriceId = clientPriceDTO.ClientPriceId;
                clientPriceParentDTO.DescriptionId = clientPriceDTO.DescriptionId;
                clientPriceParentDTO.PipSheetId = clientPriceDTO.PipSheetId;
                clientPriceParentDTO.TotalPrice = clientPriceDTO.TotalPrice;
                clientPriceParentDTO.CreatedBy = clientPriceDTO.CreatedBy;
                clientPriceParentDTO.UpdatedBy = clientPriceDTO.UpdatedBy;
                clientPriceParentDTO.CreatedOn = clientPriceDTO.CreatedOn;
                clientPriceParentDTO.UpdatedOn = clientPriceDTO.UpdatedOn;
                clientPriceParentDTO.ClientPricePeriodDTO = clientPriceDBResultDTO.ClientPricePeriodDTO.Where(x => x.ClientPriceId == clientPriceDTO.ClientPriceId).ToList();
                clientPriceMainDTO.ClientPriceDTO.Add(clientPriceParentDTO);
            }
            return clientPriceMainDTO;
        }
    }
}
