using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ExpenseAndAssetService : IExpenseAndAssetService
    {
        private readonly IExpenseAndAssetRepository expenseAndAssetRepository;

        public ExpenseAndAssetService(IExpenseAndAssetRepository expenseAndAssetRepository)
        {
            this.expenseAndAssetRepository = expenseAndAssetRepository;
        }

        public async Task<ExpenseAndAssetDTO> GetExpenseAndAsset(int pipSheetId)
        {
            ExpenseAndAssetParentDTO expenseAndAssetParentDTO = await this.expenseAndAssetRepository.GetExpenseAndAsset(pipSheetId);
            ExpenseAndAssetDTO expenseAndAssetDTO = new ExpenseAndAssetDTO();

            if (expenseAndAssetParentDTO.ProjectPeriodDTO.Count > 0)
            {
                List<SeatCostAndStdOverheadDTO> seatCostAndStdOverheadDTO = CalculateEbidtaSeatCostAndStdOverhead(expenseAndAssetParentDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.ResourcePlanningPipSheetDTO[0],
                                       expenseAndAssetParentDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.LaborPricingEbidtaCalculationDTO,
                                       expenseAndAssetParentDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.HolidayList);

                List<DirectExpensePeriodDTO> directExpensePeriodSeatCostDTO = new List<DirectExpensePeriodDTO>();
                List<DirectExpensePeriodDTO> directExpensePeriodOverheadDTO = new List<DirectExpensePeriodDTO>();

                for (int j = 0; j < seatCostAndStdOverheadDTO.Count; j++)
                {
                    DirectExpensePeriodDTO directExpenseSeatCost = new DirectExpensePeriodDTO();
                    DirectExpensePeriodDTO directExpenseStdOverHead = new DirectExpensePeriodDTO();

                    directExpenseSeatCost.Expense = seatCostAndStdOverheadDTO[j].SeatCost;
                    directExpenseStdOverHead.Expense = seatCostAndStdOverheadDTO[j].StdOverHead;

                    directExpensePeriodSeatCostDTO.Add(directExpenseSeatCost);
                    directExpensePeriodOverheadDTO.Add(directExpenseStdOverHead);
                }

                List<string> description = new List<string>();
                description.Add("Seat Cost");
                description.Add("Standard Overhead");

                List<DirectExpenseParentDTO> directExpenseListDTO = new List<DirectExpenseParentDTO>();
                for (int length = 0; length < 2; length++)
                {
                    DirectExpenseParentDTO directExpenseDTO = new DirectExpenseParentDTO();
                    directExpenseDTO.DirectExpensePeriodDTO = new List<DirectExpensePeriodDTO>();
                    directExpenseDTO.PipSheetId = pipSheetId;
                    directExpenseDTO.Description = description[length];

                    if (length == 0)
                    {
                        directExpenseDTO.DirectExpensePeriodDTO.AddRange(directExpensePeriodSeatCostDTO);
                    }
                    else if (length == 1)
                    {
                        directExpenseDTO.DirectExpensePeriodDTO.AddRange(directExpensePeriodOverheadDTO);
                    }
                    directExpenseListDTO.Add(directExpenseDTO);
                }

                expenseAndAssetParentDTO.DirectExpenseDTO.Insert(0, directExpenseListDTO[0]);
                expenseAndAssetParentDTO.DirectExpenseDTO.Insert(1, directExpenseListDTO[1]);

                expenseAndAssetDTO.AssetDTO = expenseAndAssetParentDTO.AssetDTO;
                expenseAndAssetDTO.DirectExpenseDTO = expenseAndAssetParentDTO.DirectExpenseDTO;
                expenseAndAssetDTO.ProjectPeriodDTO = expenseAndAssetParentDTO.ProjectPeriodDTO;
                expenseAndAssetDTO.ProjectMilestoneDTO = expenseAndAssetParentDTO.ProjectMilestoneDTO;
                expenseAndAssetDTO.PeriodLaborRevenueDTO = expenseAndAssetParentDTO.PeriodLaborRevenueDTO;
            }
            return expenseAndAssetDTO;
        }

        private List<SeatCostAndStdOverheadDTO> CalculateEbidtaSeatCostAndStdOverhead(ResourcePlanningPipSheetDTO resourcePlanningPipSheetDTO,
        IList<LaborPricingEbidtaCalculationDTO> laborPricingEbidtaCalculationDTO, IList<HolidayDTO> holidayList)
        {
            DateTime startDate = resourcePlanningPipSheetDTO.StartDate;
            DateTime endDate = resourcePlanningPipSheetDTO.EndDate;
            bool holidayOption = resourcePlanningPipSheetDTO.HolidayOption;

            DateTime startDateFirstDayOfMonth = DateHelper.GetFirstDayOfMonth(startDate);
            DateTime startDateLastDayOfMonth = DateHelper.GetLastDayOfMonth(startDate);
            DateTime endDateFirstDayOfMonth = DateHelper.GetFirstDayOfMonth(endDate);
            DateTime endDateLastDayOfMonth = DateHelper.GetLastDayOfMonth(endDate);

            int noOfPeriods = DateHelper.GetMonthsBetween(startDate, endDate);

            List<int> uniqueProjResId = (from l in laborPricingEbidtaCalculationDTO select l.ProjectResourceId).Distinct().ToList();

            IDictionary<int, IList<LaborPricingEbidtaCalculationDTO>> dict = new Dictionary<int, IList<LaborPricingEbidtaCalculationDTO>>();
            foreach (int resId in uniqueProjResId)
            {
                List<LaborPricingEbidtaCalculationDTO> obj = new List<LaborPricingEbidtaCalculationDTO>();
                obj.AddRange(laborPricingEbidtaCalculationDTO.Where(singleItem => singleItem.ProjectResourceId == resId));
                dict.Add(resId, obj);
            }
            List<SeatCostAndStdOverheadDTO> seatCostAndStdOverheadDTO = new List<SeatCostAndStdOverheadDTO>();

            for (int period = 0; period < noOfPeriods; period++)
            {
                SeatCostAndStdOverheadDTO seatCostAndStdOverheadDTO1 = new SeatCostAndStdOverheadDTO();
                foreach (KeyValuePair<int, IList<LaborPricingEbidtaCalculationDTO>> entry in dict)
                {
                    for (int i = period; i < entry.Value.Count;)
                    {
                        if (i != 0 && i != noOfPeriods - 1)
                        {
                            seatCostAndStdOverheadDTO1.SeatCost = Math.Round(seatCostAndStdOverheadDTO1.SeatCost + (entry.Value[i].EbitdaSeatCost * entry.Value[i].FTEValue), 2);
                            seatCostAndStdOverheadDTO1.StdOverHead = Math.Round(seatCostAndStdOverheadDTO1.StdOverHead + entry.Value[i].OverheadAmount * entry.Value[i].FTEValue, 2);
                            break;
                        }
                        else if (i == 0)
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

                                seatCostAndStdOverheadDTO1.SeatCost = Math.Round(seatCostAndStdOverheadDTO1.SeatCost + ebidtaSeatCostPerDay * daysWorked * entry.Value[i].FTEValue, 2);
                                seatCostAndStdOverheadDTO1.StdOverHead = Math.Round(seatCostAndStdOverheadDTO1.StdOverHead + stdOverheadPerDay * daysWorked * entry.Value[i].FTEValue, 2);
                                break;
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
                                seatCostAndStdOverheadDTO1.SeatCost = Math.Round(seatCostAndStdOverheadDTO1.SeatCost + ebidtaSeatCostPerDay * daysWorked * entry.Value[i].FTEValue, 2);
                                seatCostAndStdOverheadDTO1.StdOverHead = Math.Round(seatCostAndStdOverheadDTO1.StdOverHead + stdOverheadPerDay * daysWorked * entry.Value[i].FTEValue, 2);
                                break;
                            }
                        }
                        else
                        {
                            if (holidayOption)
                            {
                                int daysWorked = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDate)
                                                - DateHelper.GetHolidaysCount(endDateFirstDayOfMonth, endDate, entry.Value[i].LocationId, holidayList);
                                int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDateLastDayOfMonth)
                                                - DateHelper.GetHolidaysCount(endDateFirstDayOfMonth, endDateLastDayOfMonth, entry.Value[i].LocationId, holidayList);

                                decimal ebidtaSeatCostPerDay = entry.Value[i].EbitdaSeatCost / noOfWorkingDays;
                                decimal stdOverheadPerDay = entry.Value[i].OverheadAmount / noOfWorkingDays;
                                seatCostAndStdOverheadDTO1.SeatCost = Math.Round(seatCostAndStdOverheadDTO1.SeatCost + ebidtaSeatCostPerDay * daysWorked * entry.Value[i].FTEValue, 2);
                                seatCostAndStdOverheadDTO1.StdOverHead = Math.Round(seatCostAndStdOverheadDTO1.StdOverHead + stdOverheadPerDay * daysWorked * entry.Value[i].FTEValue, 2);
                                break;
                            }
                            else
                            {
                                int daysWorked = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDate);
                                int noOfWorkingDays = DateHelper.GetNumberOfWorkingDays(endDateFirstDayOfMonth, endDateLastDayOfMonth);

                                decimal ebidtaSeatCostPerDay = entry.Value[i].EbitdaSeatCost / noOfWorkingDays;
                                decimal stdOverheadPerDay = entry.Value[i].OverheadAmount / noOfWorkingDays;

                                seatCostAndStdOverheadDTO1.SeatCost = Math.Round(seatCostAndStdOverheadDTO1.SeatCost + ebidtaSeatCostPerDay * daysWorked * entry.Value[i].FTEValue, 2);
                                seatCostAndStdOverheadDTO1.StdOverHead = Math.Round(seatCostAndStdOverheadDTO1.StdOverHead + stdOverheadPerDay * daysWorked * entry.Value[i].FTEValue, 2);
                                break;
                            }
                        }
                    }
                }
                seatCostAndStdOverheadDTO.Add(seatCostAndStdOverheadDTO1);
            }
            return seatCostAndStdOverheadDTO;
        }

        public async Task<ExpenseAndAssetDTO> CreateExpenseAndAssetObject(ExpenseAndAssetSaveDependencyDTO expenseAndAssetSaveDependencyDTO)
        {
            int uid = 0;
            decimal? total;
            int count;
            BasicAssetDTO basicAsset = null;

            expenseAndAssetSaveDependencyDTO.DirectExpenseDTO.ForEach(expense =>
            {
                count = 0;
                total = 0;
                expense.UId = uid;
                if (expense.PercentRevenue > 0)
                {
                    for (int i = 0; i < expense.DirectExpensePeriodDTO.Count; i++)
                    {
                        expense.DirectExpensePeriodDTO[i].BillingPeriodId = expenseAndAssetSaveDependencyDTO.ProjectPeriodDTO[count].BillingPeriodId;
                        expense.DirectExpensePeriodDTO[i].UId = uid;
                        expense.DirectExpensePeriodDTO[i].Expense = (expense.PercentRevenue / 100) * expenseAndAssetSaveDependencyDTO.PeriodLaborRevenueDTO[i].Revenue;
                        total = total + expense.DirectExpensePeriodDTO[i].Expense;
                        count++;
                    }
                }
                else 
                {
                    expense.DirectExpensePeriodDTO.ForEach(period =>
                    {
                        period.BillingPeriodId = expenseAndAssetSaveDependencyDTO.ProjectPeriodDTO[count].BillingPeriodId;
                        period.UId = uid;
                        total = total + period.Expense;
                        count++;
                    });
                }
                expense.TotalExpense = total == null ? 0 : (Int32)total;
                uid++;
            });

            expenseAndAssetSaveDependencyDTO.AssetDTO.ForEach(asset =>
            {
                if (asset.BasicAssetId != null)
                {
                    basicAsset = new BasicAssetDTO();
                    basicAsset = expenseAndAssetSaveDependencyDTO.BasicAssetDTO.FirstOrDefault(x => x.BasicAssetId == asset.BasicAssetId);
                    asset.Amount = asset.Count * basicAsset.CostToProject;
                    asset.CostToProject = basicAsset.CostToProject;
                }
            });

            ExpenseAndAssetDTO expenseAndAssetDTO = new ExpenseAndAssetDTO();
            expenseAndAssetDTO.AssetDTO = expenseAndAssetSaveDependencyDTO.AssetDTO;
            expenseAndAssetDTO.DirectExpenseDTO = expenseAndAssetSaveDependencyDTO.DirectExpenseDTO;
            expenseAndAssetDTO.ProjectPeriodDTO = expenseAndAssetSaveDependencyDTO.ProjectPeriodDTO;
            expenseAndAssetDTO.ProjectMilestoneDTO = expenseAndAssetSaveDependencyDTO.ProjectMilestoneDTO;

            return expenseAndAssetDTO;
        }

        public async Task SaveExpenseAndAssetData(string userName, ExpenseAndAssetDTO expenseAndAssetDto)
        {
            IList<DirectExpensePeriodDTO> directExpensePeriods = expenseAndAssetDto.DirectExpenseDTO.SelectMany(x => x.DirectExpensePeriodDTO).ToList();
            IList<DirectExpenseDTO> directExpense = expenseAndAssetDto.DirectExpenseDTO.Select(x => new DirectExpenseDTO
            {
                UId = x.UId,
                DirectExpenseId = x.DirectExpenseId,
                PipSheetId = x.PipSheetId,
                Description = x.Description,
                Label = x.Label,
                TotalExpense = x.TotalExpense,
                MilestoneId = x.MilestoneId == -1 ? null : x.MilestoneId,       // Passing null if nothing is selected from dropdown (-1)
                IsDeleted = x.IsDeleted,
                PercentRevenue = x.PercentRevenue,
                IsReimbursable = x.IsReimbursable,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            }).ToList();

            foreach (var asset in expenseAndAssetDto.AssetDTO)
            {
                asset.UpdatedOn = DateTime.Now;
                asset.CreatedOn = DateTime.Now;
            }

            await expenseAndAssetRepository.SaveExpenseAndAssetData(userName, directExpensePeriods, directExpense, expenseAndAssetDto.AssetDTO);
        }

        public async Task<ExpenseAndAssetSaveDependencyDTO> GetExpenseAndAssetForSaveDependency(int pipSheetId)
        {
            ExpenseAndAssetSaveDependencyDTO expenseAndAssetSaveDependencyDTO = await this.expenseAndAssetRepository.GetExpenseAndAssetForSaveDependency(pipSheetId);

            List<SeatCostAndStdOverheadDTO> seatCostAndStdOverheadDTO = CalculateEbidtaSeatCostAndStdOverhead(expenseAndAssetSaveDependencyDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.ResourcePlanningPipSheetDTO[0],
                                   expenseAndAssetSaveDependencyDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.LaborPricingEbidtaCalculationDTO,
                                   expenseAndAssetSaveDependencyDTO.ExpenseAndAssetSeatCostAndStdOverheadCalcDTO.HolidayList);

            List<DirectExpensePeriodDTO> directExpensePeriodSeatCostDTO = new List<DirectExpensePeriodDTO>();
            List<DirectExpensePeriodDTO> directExpensePeriodOverheadDTO = new List<DirectExpensePeriodDTO>();

            for (int j = 0; j < seatCostAndStdOverheadDTO.Count; j++)
            {
                DirectExpensePeriodDTO directExpenseSeatCost = new DirectExpensePeriodDTO();
                DirectExpensePeriodDTO directExpenseStdOverHead = new DirectExpensePeriodDTO();

                directExpenseSeatCost.Expense = seatCostAndStdOverheadDTO[j].SeatCost;
                directExpenseStdOverHead.Expense = seatCostAndStdOverheadDTO[j].StdOverHead;

                directExpensePeriodSeatCostDTO.Add(directExpenseSeatCost);
                directExpensePeriodOverheadDTO.Add(directExpenseStdOverHead);
            }

            List<string> description = new List<string>();
            description.Add("Seat cost (for Ebitda calc) ");
            description.Add("Standard Overhead (labor exp)");

            List<DirectExpenseParentDTO> directExpenseListDTO = new List<DirectExpenseParentDTO>();
            for (int length = 0; length < 2; length++)
            {
                DirectExpenseParentDTO directExpenseDTO = new DirectExpenseParentDTO();
                directExpenseDTO.DirectExpensePeriodDTO = new List<DirectExpensePeriodDTO>();
                directExpenseDTO.PipSheetId = pipSheetId;
                directExpenseDTO.Description = description[length];

                if (length == 0)
                {
                    directExpenseDTO.DirectExpensePeriodDTO.AddRange(directExpensePeriodSeatCostDTO);
                }
                else if (length == 1)
                {
                    directExpenseDTO.DirectExpensePeriodDTO.AddRange(directExpensePeriodOverheadDTO);
                }
                directExpenseListDTO.Add(directExpenseDTO);
            }

            expenseAndAssetSaveDependencyDTO.DirectExpenseDTO.Insert(0, directExpenseListDTO[0]);
            expenseAndAssetSaveDependencyDTO.DirectExpenseDTO.Insert(1, directExpenseListDTO[1]);

            return expenseAndAssetSaveDependencyDTO;
        }
    }
}
