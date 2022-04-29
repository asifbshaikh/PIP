using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;


namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ResourcePlanningService : IResourcePlanningService
    {
        private readonly IResourcePlanningRepository resourcePlanningRepository;

        public ResourcePlanningService(IResourcePlanningRepository repo)
        {
            this.resourcePlanningRepository = repo;
        }

        public async Task<List<LocationDTO>> GetProjectLocations(int pipSheetId)
        {
            return await this.resourcePlanningRepository.GetProjectLocations(pipSheetId);
        }

        public async Task<List<ProjectMilestoneDTO>> GetProjectMilestones(int pipSheetId)
        {
            return await this.resourcePlanningRepository.GetProjectMilestones(pipSheetId);
        }

        public async Task SaveResourcePlanningData(string userName, IList<ResourcePlanningDTO> resourcePlanningDTO)
        {
            IList<ProjectResourcePeriodDTO> projectPeriods = resourcePlanningDTO.Where(x => x.IsDeleted == false).SelectMany(x => x.ProjectPeriod).ToList();
            IList<ProjectResourceDTO> projectResource = resourcePlanningDTO.Select(x => new ProjectResourceDTO
            {
                UId = x.UId,
                ProjectResourceId = x.ProjectResourceId,
                PipSheetId = x.PipSheetId,
                LocationId = x.LocationId,
                MarkupId = x.MarkupId == -1 ? null : x.MarkupId,
                MilestoneId = x.MilestoneId == -1 ? null : x.MilestoneId,
                ResourceGroupId = x.ResourceGroupId,
                ResourceId = x.ResourceId,
                UtilizationType = x.UtilizationType,
                IsDeleted = x.IsDeleted,
                TotalHoursPerResource = x.TotalhoursPerResource,
                CostHrsPerResource = x.CostHrsPerResource,
                FTEPerResource = CalculateResourceFTETotal(projectPeriods, x.UId),
                Alias = x.Alias,
                ClientRole = x.ClientRole,
                ResourceServiceLineId = x.ResourceServiceLineId
            }).ToList();
            decimal inflatedCostHours = CalculateTotalInflatedCostHours(projectPeriods);
            IList<ProjectPeriodTotalDTO> projectPeriodTotals = CalculatePeriodFTETotals(projectPeriods, resourcePlanningDTO[0].PipSheetId);

            await this.resourcePlanningRepository.SaveResourcePlanningData(userName, projectResource, projectPeriods, projectPeriodTotals, inflatedCostHours);
        }

        public async Task<ResourcePlanningMainDTO> GetResourcePlanningData(int pipSheetId)
        {
            return await resourcePlanningRepository.GetResourcePlanningData(pipSheetId);
        }

        public async Task<ResourcePlanningSaveDependencyDTO> GetResourcePlanningDataForSaveDependency(int pipSheetId)
        {
            return await resourcePlanningRepository.GetResourcePlanningDataForSaveDependency(pipSheetId);
        }

        public IList<ProjectPeriodTotalDTO> CalculatePeriodFTETotals(IList<ProjectResourcePeriodDTO> projectPeriods, int pipSheetId)
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
                decimal fteTotal = 0;
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    fteTotal += entry.Value[i].FTEValue;
                }
                ProjectPeriodTotalDTO ppt = new ProjectPeriodTotalDTO()
                {
                    BillingPeriodId = entry.Key,
                    PipSheetId = pipSheetId,
                    FTE = fteTotal
                };
                projectPeriodTotals.Add(ppt);
            }
            return projectPeriodTotals;
        }

        private decimal CalculateTotalInflatedCostHours(IList<ProjectResourcePeriodDTO> projectPeriods)
        {
            return (from pp in projectPeriods select pp.InflatedCostHours).Sum();
        }

        private decimal CalculateResourceFTETotal(IList<ProjectResourcePeriodDTO> projectPeriods, int uId)
        {
            return (from period in projectPeriods where period.UId == uId select period.FTEValue).Sum();
        }

        private int GetHolidaysCountInParticularMonth(DateTime startDate, DateTime endDate, int locationId, IList<HolidayDTO> resourceHolidayDTO,
         DateTime projectStartDate, DateTime projectEndDate)
        {
            int count = 0;
            List<HolidayDTO> holidayDTO = new List<HolidayDTO>();
            holidayDTO = resourceHolidayDTO.Where(holiday => holiday.LocationId == locationId).ToList();

            holidayDTO.ForEach(holiday =>
            {
                if (((startDate.Month == projectStartDate.Month) && (startDate.Year == projectStartDate.Year))
                  && ((holiday.Date.Month == startDate.Month) && (holiday.Date.Year == endDate.Year))
                  && holiday.Date.DayOfWeek != DayOfWeek.Sunday && holiday.Date.DayOfWeek != DayOfWeek.Saturday)
                {
                    count++;
                }
                else if (((endDate.Month == projectEndDate.Month) && (endDate.Year == projectEndDate.Year))
                  && ((holiday.Date.Month == endDate.Month) && (holiday.Date.Year == endDate.Year))
                  && holiday.Date.DayOfWeek != DayOfWeek.Sunday && holiday.Date.DayOfWeek != DayOfWeek.Saturday)
                {
                    count++;
                }
                else if (holiday.Date >= startDate && holiday.Date <= endDate &&
                         holiday.Date.DayOfWeek != DayOfWeek.Sunday && holiday.Date.DayOfWeek != DayOfWeek.Saturday)
                {
                    // 0 - Sunday, 6 - Saturday
                    count++;
                }
            });
            return count;
        }

        private CostValueAndFTEPerResourceDTO CalculateResourcePerHour(IList<CostAndFTEPerResourceDTO> costAndFTEPerResource, int rowIndex)
        {
            CostValueAndFTEPerResourceDTO costValueAndFTEPerResourceDTO = new CostValueAndFTEPerResourceDTO();

            foreach (CostAndFTEPerResourceDTO costAndFTEPerResourceDTO in costAndFTEPerResource)
            {
                if (costAndFTEPerResourceDTO.Key == rowIndex)
                {
                    costValueAndFTEPerResourceDTO.TotalCostValue += costAndFTEPerResourceDTO.CostHours;
                    costValueAndFTEPerResourceDTO.FTEPerResource += costAndFTEPerResourceDTO.FTE;
                }
            }
            return costValueAndFTEPerResourceDTO;
        }

        private decimal GetTotalCostHours(IList<ResourcePlanningDTO> resource)
        {
            decimal totalCostHours = 0;

            foreach (ResourcePlanningDTO singleResource in resource)
            {
                foreach (ProjectResourcePeriodDTO period in singleResource.ProjectPeriod)
                {
                    totalCostHours += period.CostHours.GetValueOrDefault(0);
                }
            }
            return totalCostHours;
        }

        private decimal GetCostHourFactor(string firstOrLastperiod, int locationId, ResourcePlanningSaveDependencyDTO resourcePlanningSaveDependencyDTO)
        {
            decimal costHourFactor = 0, noOfHolidays = 0, daysWorked = 0, totalWorkingDays = 0;
            DateTime startDate = new DateTime(resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].StartDate.Year
                , resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].StartDate.Month
                , resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].StartDate.Day);
            DateTime endDate = new DateTime(resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].EndDate.Year
                , resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].EndDate.Month
                , resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].EndDate.Day);
            DateTime lastDayofMonth = new DateTime();
            DateTime firstDayOfMonth = new DateTime();

            if (firstOrLastperiod == Constants.First)
            {
                lastDayofMonth = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1).AddDays(-1);
                firstDayOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
                if (resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].HolidayOption)
                {
                    noOfHolidays = DateHelper.GetHolidaysCount(startDate, lastDayofMonth, locationId, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO);

                    if (startDate > firstDayOfMonth && endDate < lastDayofMonth)
                    {
                        daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, endDate) -
           DateHelper.GetHolidaysCount(startDate, lastDayofMonth, locationId, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO);
                    }

                    else if (startDate > firstDayOfMonth)
                    { // if start date is greater than 1st day of month
                        daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, lastDayofMonth) -
                          DateHelper.GetHolidaysCount(startDate, lastDayofMonth, locationId, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO);
                    }
                    else
                    {
                        if (endDate < lastDayofMonth)
                        { // if end date is in the same month
                            daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, endDate) -
                              DateHelper.GetHolidaysCount(startDate, endDate, locationId, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO);
                        }
                        else
                        {
                            daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, lastDayofMonth) -
                              DateHelper.GetHolidaysCount(startDate, lastDayofMonth, locationId, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO);
                        }
                    }
                    totalWorkingDays = DateHelper.GetNumberOfWorkingDays(firstDayOfMonth, lastDayofMonth) -
                    DateHelper.GetHolidaysCount(firstDayOfMonth, lastDayofMonth, locationId, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO);
                    costHourFactor = daysWorked / totalWorkingDays;
                }
                else
                {
                    if (startDate > firstDayOfMonth && endDate < lastDayofMonth)
                    {
                        daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, endDate);
                    }
                    else if (startDate > firstDayOfMonth)
                    {
                        daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, lastDayofMonth);
                    }
                    else
                    {
                        if (endDate < lastDayofMonth)
                        { // if end date is in the same month
                            daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, endDate);
                        }
                        else
                        {
                            daysWorked = DateHelper.GetNumberOfWorkingDays(startDate, lastDayofMonth);
                        }

                    }
                    totalWorkingDays = DateHelper.GetNumberOfWorkingDays(firstDayOfMonth, lastDayofMonth);
                    costHourFactor = daysWorked / totalWorkingDays;
                }
            }
            // Last Period Column
            else
            {
                lastDayofMonth = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(1).AddDays(-1);
                firstDayOfMonth = new DateTime(endDate.Year, endDate.Month, 1);
                if (resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].HolidayOption)
                {
                    noOfHolidays = DateHelper.GetHolidaysCount(firstDayOfMonth, endDate, locationId, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO);
                    daysWorked = DateHelper.GetNumberOfWorkingDays(firstDayOfMonth, endDate) - noOfHolidays;
                    totalWorkingDays = DateHelper.GetNumberOfWorkingDays(firstDayOfMonth, lastDayofMonth)
                      - DateHelper.GetHolidaysCount(firstDayOfMonth, lastDayofMonth, locationId, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO);
                    costHourFactor = daysWorked / totalWorkingDays;
                }
                else
                {
                    daysWorked = DateHelper.GetNumberOfWorkingDays(firstDayOfMonth, endDate);
                    totalWorkingDays = DateHelper.GetNumberOfWorkingDays(firstDayOfMonth, lastDayofMonth);
                    costHourFactor = daysWorked / totalWorkingDays;
                }
            }
            return Math.Round(costHourFactor, 4, MidpointRounding.AwayFromZero);
        }

        private List<MonthlyData> GetMonthlyData(IList<LocationDTO> resourceLocationDTO, IList<HolidayDTO> resourceHolidayDTO, DateTime startDate, DateTime endDate)
        {
            List<MonthlyData> monthlyData = new List<MonthlyData>();
            int startMonth = startDate.Month, endMonth = endDate.Month, startYear = startDate.Year, endYear = endDate.Year, daysInCurrentMonth = 0, totalWorkingDaysInMonth = 0;
            int actualWorkingDaysInMonth = 0;
            DateTime startDateOfCurrentMonth, endDateOfCurrentMonth;
            bool isFirstMonthOfProject, isLastMonthOfProject;
            List<LocationHoliday> locationHolidays = null;
            DateTime comparerDate = new DateTime(startYear, startMonth, 1);

            for (int i = startMonth, j = startYear; comparerDate <= endDate;)
            {
                locationHolidays = new List<LocationHoliday>();
                isFirstMonthOfProject = (i == startMonth && startYear == j);
                isLastMonthOfProject = (i == endMonth && endYear == j);

                daysInCurrentMonth = DateHelper.getDaysInMonth(i, j);

                startDateOfCurrentMonth = new DateTime(j, i, 1);
                endDateOfCurrentMonth = new DateTime(j, i, daysInCurrentMonth);

                totalWorkingDaysInMonth = DateHelper.GetNumberOfWorkingDays(startDateOfCurrentMonth, endDateOfCurrentMonth);
                actualWorkingDaysInMonth = DateHelper.GetNumberOfWorkingDays(isFirstMonthOfProject ? startDate : startDateOfCurrentMonth,
                                                     isLastMonthOfProject ? endDate : endDateOfCurrentMonth);

                for (int x = 0; x < resourceLocationDTO.Count; x++)
                {
                    locationHolidays.Add(new LocationHoliday()
                    {
                        LocationId = resourceLocationDTO[x].LocationId,
                        NumberOfHolidays = DateHelper.GetHolidaysCount(isFirstMonthOfProject ? startDate : startDateOfCurrentMonth
                                                                             , isLastMonthOfProject ? endDate : endDateOfCurrentMonth
                                                                             , resourceLocationDTO[x].LocationId, resourceHolidayDTO),
                        NumberOfHolidaysInParticularMonth = this.GetHolidaysCountInParticularMonth(
                                                                    isFirstMonthOfProject ? startDate : startDateOfCurrentMonth
                                                                   , isLastMonthOfProject ? endDate : endDateOfCurrentMonth
                                                                   , resourceLocationDTO[x].LocationId, resourceHolidayDTO
                                                                   , startDate
                                                                   , endDate)
                    });
                }
                monthlyData.Add(new MonthlyData()
                {
                    WorkingDays = totalWorkingDaysInMonth,
                    LocationHolidays = locationHolidays,
                    TotalDays = daysInCurrentMonth,
                    ActualWorkingDays = actualWorkingDaysInMonth
                });
                i++;
                if (i > 12)
                {
                    i = 1;
                    j++;
                    comparerDate = new DateTime(j, i, 1);
                }
                else
                {
                    comparerDate = new DateTime(j, i, 1);
                }
            }
            return monthlyData;
        }

        private decimal CalculateFTE(ResourcePlanningDTO resourcePlanningData, List<MonthlyData> monthlyData, ResourcePlanningSaveDependencyDTO resourcePlanningSaveDependencyDTO,
             int projectDurationInDays, int totalDaysInProjectDurationMonths, int resourceIndex, LocationDTO resourceLocation, int selectedResourceLocationId)
        {
            int index = 0;
            decimal totalHrs = 0, totalHrsForTheMonth = 0;
            List<decimal> fte = new List<decimal>();

            int noOfHolidays = 0, noOfHolidaysInParticularMonth = 0, totalNumberOfWorkingDays = 0, daysWorked = 0;

            string selectedBillingTypeName = resourcePlanningSaveDependencyDTO.ProjectBillingTypeDTO.FirstOrDefault(billingType =>
            billingType.ProjectBillingTypeId == resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].ProjectBillingTypeID).BillingTypeName;

            foreach (ProjectResourcePeriodDTO projectPeriod in resourcePlanningData.ProjectPeriod)
            {
                fte.Add(projectPeriod.FTEValue == 0 ? 0 : projectPeriod.FTEValue);
            }

            monthlyData.ForEach(currentMonthData =>
            {
                decimal computedValue = 0;
                resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].UId = resourceIndex;

                if (fte[index] != 0)
                {
                    if (selectedBillingTypeName == Constants.FlatFeeMonthly || selectedBillingTypeName == Constants.MonthlyFixedHours)
                    {
                        if (projectDurationInDays == totalDaysInProjectDurationMonths)
                        {
                            computedValue = (decimal)(fte[index] * (resourceLocation.HoursPerMonth));
                            totalHrs += computedValue;
                            resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                        }
                        else
                        {
                            if (currentMonthData.WorkingDays == currentMonthData.ActualWorkingDays)
                            {
                                computedValue = (decimal)(fte[index] * (resourceLocation.HoursPerMonth));
                                totalHrs += computedValue;
                                resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                            }
                            else
                            {
                                if (resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].HolidayOption)
                                {
                                    noOfHolidays = currentMonthData.LocationHolidays.Find(location =>
                                      location.LocationId == selectedResourceLocationId).NumberOfHolidays;

                                    noOfHolidaysInParticularMonth = currentMonthData.LocationHolidays.Find(location =>
                                      location.LocationId == selectedResourceLocationId).NumberOfHolidaysInParticularMonth;

                                    totalNumberOfWorkingDays = currentMonthData.WorkingDays - noOfHolidaysInParticularMonth;
                                    totalHrsForTheMonth = (decimal)(resourceLocation.HoursPerMonth / totalNumberOfWorkingDays);
                                    daysWorked = currentMonthData.ActualWorkingDays - noOfHolidays;
                                    computedValue = fte[index] * (daysWorked * totalHrsForTheMonth);
                                    totalHrs += computedValue;
                                    resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                                }
                                else
                                {
                                    totalHrsForTheMonth = (decimal)(resourceLocation.HoursPerMonth / currentMonthData.WorkingDays);
                                    daysWorked = currentMonthData.ActualWorkingDays;
                                    computedValue = fte[index] * (daysWorked * totalHrsForTheMonth);
                                    totalHrs += computedValue;
                                    resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                                }
                            }
                        }
                    }
                    //Other than Flat fee monthly and Monthly fixed hours
                    else
                    {
                        if (projectDurationInDays == totalDaysInProjectDurationMonths)
                        {
                            if (resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].HolidayOption)
                            {
                                noOfHolidays = currentMonthData.LocationHolidays.Find(location =>
                                     location.LocationId == selectedResourceLocationId).NumberOfHolidays;
                                totalNumberOfWorkingDays = currentMonthData.WorkingDays - noOfHolidays;
                                computedValue = (decimal)(fte[index] * (resourceLocation.HoursPerDay * totalNumberOfWorkingDays));
                                totalHrs += computedValue;
                                resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                            }
                            else
                            {
                                computedValue = (decimal)(fte[index] * (currentMonthData.WorkingDays * resourceLocation.HoursPerDay));
                                totalHrs += computedValue;
                                resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                            }
                        }
                        else
                        {
                            if (currentMonthData.WorkingDays == currentMonthData.ActualWorkingDays)
                            {
                                if (resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].HolidayOption)
                                {
                                    noOfHolidays = currentMonthData.LocationHolidays.Find(location =>
                                       location.LocationId == selectedResourceLocationId).NumberOfHolidays;
                                    noOfHolidaysInParticularMonth = currentMonthData.LocationHolidays.Find(location =>
                                     location.LocationId == selectedResourceLocationId).NumberOfHolidaysInParticularMonth;
                                    totalNumberOfWorkingDays = currentMonthData.WorkingDays - noOfHolidaysInParticularMonth;
                                    computedValue = (decimal)(fte[index] * (resourceLocation.HoursPerDay * totalNumberOfWorkingDays));
                                    totalHrs += computedValue;
                                    resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                                }
                                else
                                {
                                    noOfHolidays = currentMonthData.LocationHolidays.Find(location =>
                                       location.LocationId == selectedResourceLocationId).NumberOfHolidays;
                                    totalNumberOfWorkingDays = currentMonthData.WorkingDays;
                                    computedValue = (decimal)(fte[index] * (resourceLocation.HoursPerDay * totalNumberOfWorkingDays));
                                    totalHrs += computedValue;
                                    resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                                }
                            }
                            else
                            {
                                if (resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].HolidayOption)
                                {
                                    noOfHolidays = currentMonthData.LocationHolidays.Find(location =>
                                      location.LocationId == selectedResourceLocationId).NumberOfHolidays;
                                    totalNumberOfWorkingDays = currentMonthData.ActualWorkingDays - noOfHolidays;
                                    computedValue = (decimal)(fte[index] * (resourceLocation.HoursPerDay * totalNumberOfWorkingDays));
                                    totalHrs += computedValue;
                                    resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                                }
                                else
                                {
                                    computedValue = (decimal)(fte[index] * currentMonthData.ActualWorkingDays * resourceLocation.HoursPerDay);
                                    totalHrs += computedValue;
                                    resourcePlanningSaveDependencyDTO.Resources[resourceIndex].ProjectPeriod[index].TotalHours = computedValue;
                                }
                            }
                        }
                    }
                }
                index++;
            });
            return totalHrs;
        }

        private void DisplayFTEValueAndTotalCostHours(ResourcePlanningSaveDependencyDTO resourcePlanningSaveDependencyDTO)
        {
            decimal totalFTE = 0, totalNormalizedCostHoursFTE = 0, costHrs = 0, costHrsFactor = 0;
            decimal costHoursEquivalent = Constants.costHoursEquivalent, totalCostHours = 0;
            List<CostAndFTEPerResourceDTO> costAndFTEPerResource = new List<CostAndFTEPerResourceDTO>();

            for (int index = 0; index < resourcePlanningSaveDependencyDTO.ProjectPeriods.Count; index++)
            {
                for (int i = 0; i < resourcePlanningSaveDependencyDTO.Resources.Count; i++)
                {
                    int locationId = resourcePlanningSaveDependencyDTO.Resources[i].LocationId;
                    for (int j = index; j < resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod.Count; j++)
                    {
                        if (resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue == 0)
                        {
                            resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue = 0;
                        }
                        if (index == 0)
                        {
                            costHrsFactor = this.GetCostHourFactor(Constants.First, locationId, resourcePlanningSaveDependencyDTO);
                            totalNormalizedCostHoursFTE = (totalNormalizedCostHoursFTE + (costHrsFactor * resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue));
                            totalFTE = (totalFTE + resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue);
                            if (Array.Exists(Constants.ContractorMarkUpIds, markUpId => markUpId == resourcePlanningSaveDependencyDTO.Resources[i].MarkupId))
                            {
                                costHrs = (resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].TotalHours) / costHoursEquivalent;
                            }
                            else
                            {
                                costHrs = costHrsFactor * (resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue);
                            }

                            // point 1
                            costAndFTEPerResource.Add(new CostAndFTEPerResourceDTO()
                            {
                                Key = i,
                                CostHours = (costHrs * costHoursEquivalent),
                                FTE = resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue
                            });
                        }
                        else if (index == (resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod.Count - 1))
                        {
                            costHrsFactor = this.GetCostHourFactor(Constants.Last, locationId, resourcePlanningSaveDependencyDTO);
                            totalNormalizedCostHoursFTE = totalNormalizedCostHoursFTE + costHrsFactor * resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue;
                            totalFTE = totalFTE + resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue;
                            if (Array.Exists(Constants.ContractorMarkUpIds, markUpId => markUpId == resourcePlanningSaveDependencyDTO.Resources[i].MarkupId))
                            {
                                costHrs = resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].TotalHours / costHoursEquivalent;
                            }
                            else
                            {
                                costHrs = costHrsFactor * resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue;
                            }

                            // point 2
                            costAndFTEPerResource.Add(new CostAndFTEPerResourceDTO()
                            {
                                Key = i,
                                CostHours = costHrs * costHoursEquivalent,
                                FTE = resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue
                            });
                        }
                        else
                        {
                            totalNormalizedCostHoursFTE = totalNormalizedCostHoursFTE + resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue;
                            totalFTE = totalFTE + resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue;

                            if (Array.Exists(Constants.ContractorMarkUpIds, markUpId => markUpId == resourcePlanningSaveDependencyDTO.Resources[i].MarkupId))
                            {
                                costHrs = resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].TotalHours / costHoursEquivalent;
                            }
                            else
                            {
                                costHrs = resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue;
                            }

                            // point 3
                            costAndFTEPerResource.Add(new CostAndFTEPerResourceDTO()
                            {
                                Key = i,
                                CostHours = costHrs * costHoursEquivalent,
                                FTE = resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].FTEValue
                            });
                        }
                        if (Array.Exists(Constants.ContractorMarkUpIds, markUpId => markUpId == resourcePlanningSaveDependencyDTO.Resources[i].MarkupId))
                        {
                            resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].CostHours = resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].TotalHours;
                        }
                        else
                        {
                            resourcePlanningSaveDependencyDTO.Resources[i].ProjectPeriod[j].CostHours = costHrs * costHoursEquivalent;
                        }
                        break;
                    }
                }

                totalCostHours = this.GetTotalCostHours(resourcePlanningSaveDependencyDTO.Resources);
                totalNormalizedCostHoursFTE = 0;
                totalFTE = 0;
            }

            // recording cost per resource per location
            for (int i = 0; i < resourcePlanningSaveDependencyDTO.Resources.Count; i++)
            {
                CostValueAndFTEPerResourceDTO costValueAndFTEPerResourceDTO = this.CalculateResourcePerHour(costAndFTEPerResource, i);
                resourcePlanningSaveDependencyDTO.Resources[i].CostHrsPerResource = costValueAndFTEPerResourceDTO.TotalCostValue;
                resourcePlanningSaveDependencyDTO.Resources[i].FTEPerResource = costValueAndFTEPerResourceDTO.FTEPerResource;
            }
        }

        public IList<ResourcePlanningDTO> CreateResourcePlanningObject(ResourcePlanningSaveDependencyDTO resourcePlanningSaveDependencyDTO)
        {
            IList<ResourcePlanningDTO> resourcePlanningDTO = new List<ResourcePlanningDTO>();
            int projectDurationInDays = 0, totalDaysInProjectDurationMonths = 0, resourceIndex = 0;

            List<MonthlyData> monthlyData = this.GetMonthlyData(resourcePlanningSaveDependencyDTO.ResourceLocationDTO, resourcePlanningSaveDependencyDTO.ResourceHolidayDTO,
                     resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].StartDate, resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].EndDate);

            projectDurationInDays = DateHelper.GetTotalDaysBetweenDates(resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].StartDate,
                                                                        resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].EndDate);

            totalDaysInProjectDurationMonths = DateHelper.GetTotalDaysInMonths(resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].StartDate,
                                                                               resourcePlanningSaveDependencyDTO.ResourcePlanMasterData[0].EndDate);

            foreach (ResourcePlanningDTO resourcePlanningData in resourcePlanningSaveDependencyDTO.Resources)
            {
                LocationDTO resourceLocation = resourcePlanningSaveDependencyDTO.ResourceLocationDTO.FirstOrDefault(val => val.LocationId == resourcePlanningData.LocationId);
                int selectedResourceLocationId = resourcePlanningData.LocationId;

                decimal fte = this.CalculateFTE(resourcePlanningData, monthlyData, resourcePlanningSaveDependencyDTO, projectDurationInDays,
                                    totalDaysInProjectDurationMonths, resourceIndex, resourceLocation, selectedResourceLocationId);
                resourcePlanningData.TotalhoursPerResource = fte;
                resourcePlanningData.UId = resourceIndex;
                resourceIndex++;
            }

            DisplayFTEValueAndTotalCostHours(resourcePlanningSaveDependencyDTO);

            foreach (ResourcePlanningDTO resourcePlanningData in resourcePlanningSaveDependencyDTO.Resources)
            {
                resourcePlanningDTO.Add(new ResourcePlanningDTO()
                {
                    UId = resourcePlanningData.UId,
                    PipSheetId = resourcePlanningData.PipSheetId,
                    LocationId = resourcePlanningData.LocationId,
                    MarkupId = resourcePlanningData.MarkupId,
                    MilestoneId = resourcePlanningData.MilestoneId,
                    ResourceGroupId = resourcePlanningData.ResourceGroupId,
                    ResourceId = resourcePlanningData.ResourceId,
                    UtilizationType = resourcePlanningData.UtilizationType,
                    IsDeleted = resourcePlanningData.IsDeleted,
                    TotalhoursPerResource = resourcePlanningData.TotalhoursPerResource,
                    CostHrsPerResource = resourcePlanningData.CostHrsPerResource,
                    FTEPerResource = resourcePlanningData.FTEPerResource,
                    Alias = resourcePlanningData.Alias,
                    NonBillableCategoryId = resourcePlanningData.NonBillableCategoryId,
                    ProjectResourceId = resourcePlanningData.ProjectResourceId,
                    ResourceServiceLineId = resourcePlanningData.ResourceServiceLineId,
                    ProjectPeriod = resourcePlanningData.ProjectPeriod
                });
            }
            return resourcePlanningDTO;
        }

        public async Task<LocationDependentCalculationDTO> GetLocationDependentCalculationData(int pipSheetId)
        {
            return await this.resourcePlanningRepository.GetLocationDependentCalculationData(pipSheetId);
        }

        public async Task SaveLocationDependentCalculations(string userName, int pipSheetId)
        {
            await this.resourcePlanningRepository.SaveLocationDependentCalculations(userName, pipSheetId);
        }
    }
}
