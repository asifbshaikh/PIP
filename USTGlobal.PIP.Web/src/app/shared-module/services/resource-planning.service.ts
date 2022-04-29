import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared';
import { SharedDataService } from '@global';
import {
  ILocation, IProjectMilestone, Resource, ResourcePlanning, MonthlyData, LocationHoliday
  , ResourcePeriod, ResourcePlanningMasterData, ProjectPeriod
} from '@shared/domain';
import { ResourceMapper } from '@shared/mapper/master/resourcemapper';
import { DateService } from '@core/services/date.service';
import { SelectItem } from 'primeng/api';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';
import { map } from 'rxjs/internal/operators';

@Injectable({
  providedIn: 'root'
})

export class ResourcePlanningService {
  constructor(
    private httpClient: HttpClient,
    private dateService: DateService,
    private sharedDataService: SharedDataService
  ) {
  }

  getResourceLocations(pipSheetId: any): Observable<ILocation[]> {
    return this.httpClient.get<ILocation[]>(Constants.webApis.getAllResourceLocations.replace('{pipSheetId}', pipSheetId));
  }

  getResourceOptionalPhase(pipSheetId: any): Observable<IProjectMilestone[]> {
    return this.httpClient.get<IProjectMilestone[]>(Constants.webApis.getAllResourceOptionalPhase.replace('{pipSheetId}', pipSheetId));
  }

  GetResourcePlanningData(pipSheetId: any): Observable<ResourcePlanning> {
    return this.httpClient.get<ResourcePlanning[]>(Constants.webApis.getResourcePlanningData.replace('{pipSheetId}', pipSheetId))
    .pipe(map(data => new ResourceMapper(this.dateService).mapper(data)));
  }

  getDummyData(): Resource[] {
    return [
      {
        'uId': 0,
        'id': 0,
        'pipSheetId': 0,
        'projectResourceId': 0,
        'alias': '',
        'locationId': -1,
        'resourceGroupId': -1,
        'resourceId': -1,
        'technologyId': -1,
        'utilizationType': true,
        'milestoneId': 1,
        'contractorFlag': '',
        'markupId': -1,
        'totalhoursPerResource': 0,
        'costHrsPerResource': 0,
        'grade': '',
        'projectPeriod': [],
        'isDeleted': false,
        'createdBy': 1,
        'updatedBy': 1,
        'totalFTE': 0,
        'resourceServiceLineId': 0,
        'clientRole': ''
      }
    ];

  }

  saveResourcePlanning(resources: Resource[]): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveResourcePlanningData, JSON.stringify(resources));
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }

  getMonthlyData(resourceLocations: SelectItem[], resourceHolidays: SelectItem[], startDate: Date
    , endDate: Date): MonthlyData[] {

    const monthlyData: MonthlyData[] = [];
    const locationHolidays: LocationHoliday[] = [];

    const startMonth = startDate.getMonth();
    const endMonth = endDate.getMonth();
    const startYear = startDate.getFullYear();
    const endYear = endDate.getFullYear();

    let daysInCurrentMonth = 0;
    let totalWorkingDaysInMonth = 0;
    let actualWorkingDaysInMonth = 0;

    let startDateOfCurrentMonth: Date;
    let endDateOfCurrentMonth: Date;
    let isFirstMonthOfProject: boolean;
    let isLastMonthOfProject: boolean;

    let comparerDate: Date = new Date(startYear, startMonth, 1);

    for (let i = startMonth, j = startYear; comparerDate <= endDate;) {

      isFirstMonthOfProject = (i === startMonth && startYear === j);
      isLastMonthOfProject = (i === endMonth && endYear === j);

      daysInCurrentMonth = this.dateService.getDaysInMonth(i, j);

      startDateOfCurrentMonth = new Date(j, i, 1);
      endDateOfCurrentMonth = new Date(j, i, daysInCurrentMonth);

      totalWorkingDaysInMonth = this.dateService.getNumberOfWorkingDays(startDateOfCurrentMonth, endDateOfCurrentMonth);

      actualWorkingDaysInMonth = this.dateService.getNumberOfWorkingDays(
        isFirstMonthOfProject ? startDate : startDateOfCurrentMonth,
        isLastMonthOfProject ? endDate : endDateOfCurrentMonth);

      for (let x = 1; x < resourceLocations.length; x++) {
        locationHolidays.push(
          {
            locationId: resourceLocations[x].value.id,
            noOfHolidays: this.getHolidaysCount(
              isFirstMonthOfProject ? startDate : startDateOfCurrentMonth
              , isLastMonthOfProject ? endDate : endDateOfCurrentMonth
              , resourceLocations[x].value.id, resourceHolidays
            ),
            noOfHolidaysInParticularMonth: this.getHolidaysCountInParticularMonth(
              isFirstMonthOfProject ? startDate : startDateOfCurrentMonth
              , isLastMonthOfProject ? endDate : endDateOfCurrentMonth
              , resourceLocations[x].value.id, resourceHolidays
              , startDate
              , endDate)
          });
      }

      monthlyData.push(
        {
          workingDays: totalWorkingDaysInMonth,
          locationHolidays: locationHolidays,
          totalDays: daysInCurrentMonth,
          actualWorkingDays: actualWorkingDaysInMonth
        });

      i++;
      if (i > 11) {
        i = 0;
        j++;
        comparerDate = new Date(j, i, 1);
      }
      else {
        comparerDate = new Date(j, i, 1);
      }
    }
    return monthlyData;
  }

  // Get holidays count in particular month
  getHolidaysCountInParticularMonth(startDate: Date, endDate: Date, locationId, resourceHolidays: SelectItem[],
    projectStartDate: Date, projectEndDate: Date): number {
    let count = 0;
    const locationHolidays = resourceHolidays.filter(holiday => holiday.value.locationId === locationId);

    locationHolidays.forEach(holiday => {
      const holidayDate = new Date(new Date(holiday.value.holidayDate).toDateString());

      if (((startDate.getMonth() === projectStartDate.getMonth()) && (startDate.getFullYear() === projectStartDate.getFullYear()))
        && ((holidayDate.getMonth() === startDate.getMonth()) && (holidayDate.getFullYear() === endDate.getFullYear()))
        && holidayDate.getDay() !== 0 && holidayDate.getDay() !== 6) {
        count++;
      }

      else if (((endDate.getMonth() === projectEndDate.getMonth()) && (endDate.getFullYear() === projectEndDate.getFullYear()))
        && ((holidayDate.getMonth() === endDate.getMonth()) && (holidayDate.getFullYear() === endDate.getFullYear()))
        && holidayDate.getDay() !== 0 && holidayDate.getDay() !== 6) {
        count++;
      }

      else if (holidayDate >= startDate && holidayDate <= endDate && holidayDate.getDay() !== 0 && holidayDate.getDay() !== 6) {
        // 0 - Sunday, 6 - Saturday
        count++;
      }
    });
    return count;
  }

  // Get holidays count
  getHolidaysCount(startDate: Date, endDate: Date, locationId, resourceHolidays: SelectItem[]) {
    let count = 0;
    const locationHolidays = resourceHolidays.filter(holiday => holiday.value.locationId === locationId);

    locationHolidays.forEach(holiday => {
      const holidayDate = new Date(new Date(holiday.value.holidayDate).toDateString());

      if (holidayDate >= startDate && holidayDate <= endDate && holidayDate.getDay() !== 0 && holidayDate.getDay() !== 6) {
        // 0 - Sunday, 6 - Saturday
        count++;
      }
    });
    return count;
  }

  getDataForNewResource(pipSheetId: number, uIdForNextRow: number, resourcePeriod: ResourcePeriod[], projectPeriod: ProjectPeriod[]) {

    const periods: ResourcePeriod[] = [];

    if (resourcePeriod) {
      resourcePeriod.forEach(val => {
        periods.push({
          billingPeriodId: val.billingPeriodId,
          fteValue: 0,
          revenue: 0,
          projectResourceId: 0,
          projectResourcePeriodDetailsId: 0,
          uId: uIdForNextRow,
          totalHours: 0,
          priceAdjustment: 0,
          costHours: 0,
          inflation: 0,
          inflatedCostHours: 0,
          cappedCost: 0,
          billRate: 0,
          costRate: 0
        });
      });
    } else {
      projectPeriod.forEach(val => {
        periods.push({
          billingPeriodId: val.billingPeriodId,
          fteValue: 0,
          revenue: 0,
          projectResourceId: 0,
          projectResourcePeriodDetailsId: 0,
          uId: uIdForNextRow,
          totalHours: 0,
          priceAdjustment: 0,
          costHours: 0,
          inflation: 0,
          inflatedCostHours: 0,
          cappedCost: 0,
          billRate: 0,
          costRate: 0
        });
      });
    }


    const newResourceData: Resource = {

      uId: uIdForNextRow,
      id: 0,
      isDeleted: false,
      locationId: -1,
      alias: '',
      markupId: -1,
      milestoneId: -1,
      pipSheetId: pipSheetId,
      projectPeriod: periods,
      projectResourceId: 0,
      resourceGroupId: -1,
      resourceId: -1,
      technologyId: -1,
      utilizationType: true,
      contractorFlag: '',
      grade: '',
      totalhoursPerResource: 0,
      costHrsPerResource: 0,
      createdBy: 1,
      updatedBy: 1,
      totalFTE: 0,
      resourceServiceLineId: 0,
      clientRole: ''
    };

    return newResourceData;
  }

  composeCopyData(copiedResources: any[], uIdForNextRow: number, projectPeriod: ResourcePeriod[], pipSheetId: number): Resource[] {
    const resourceData: Resource[] = [];
    let copyresource: Resource;
    let periods: ResourcePeriod[] = [];

    copiedResources.forEach((resource, rindex) => {
      periods = [];
      // copy periods data
      resource.periods.forEach((period, pindex) => {
        periods.push({
          billingPeriodId: projectPeriod[pindex].billingPeriodId,
          fteValue: period.FTEValue,
          revenue: 0,
          projectResourceId: 0,
          projectResourcePeriodDetailsId: 0,
          uId: uIdForNextRow,
          totalHours: period.TotalHours,
          priceAdjustment: 0,
          costHours: period.costHours,
          inflation: 0,
          inflatedCostHours: 0,
          cappedCost: 0,
          billRate: 0,
          costRate: 0
        });
      });
      // copy resource data
      copyresource = {
        uId: uIdForNextRow,
        id: 0,
        isDeleted: resource.isDeleted,
        alias: resource.alias,
        locationId: resource.location ? resource.location.id : -1,
        markupId: resource.contractorMarkup ? resource.contractorMarkup.id : -1,
        milestoneId: resource.phase ? resource.phase.id : -1,
        pipSheetId: pipSheetId,
        projectPeriod: periods,
        projectResourceId: 0,
        resourceGroupId: resource.roleGroup ? resource.roleGroup.id : -1,
        resourceId: resource.ustRole ? resource.ustRole.id : -1,
        technologyId: null,
        utilizationType: true,
        contractorFlag: resource.contractorFlag,
        grade: resource.grade,
        totalhoursPerResource: resource.totalHours,
        costHrsPerResource: resource.totalCostHours,
        createdBy: 1,
        updatedBy: 1,
        totalFTE: resource.totalFTE,
        resourceServiceLineId: resource.resourceServiceLineId,
        clientRole: resource.clientRole
      };

      // push copied row to resource data
      resourceData.push(copyresource);
      uIdForNextRow += 1;
    });
    return resourceData;
  }

  getCostHourFactor(firstOrLastperiod: string, locationId: number, resourceMasterData: ResourcePlanningMasterData,
    resourceHolidays: SelectItem[]): string {

    const startDate = new Date(new Date(resourceMasterData.startDate).toDateString());
    const endDate = new Date(new Date(resourceMasterData.endDate).toDateString());
    const holidayOption = resourceMasterData.isHolidayOptionOn;
    let lastDayofMonth = new Date();
    let firstDayOfMonth = new Date();
    let daysWorked = 0;
    let totalWorkingDays = 0;
    let costHourFactor = 0;
    let noOfHolidays = 0;
    if (firstOrLastperiod === 'first') {
      lastDayofMonth = new Date(new Date(startDate.getFullYear(), (startDate.getMonth() + 1), 0).toDateString());
      firstDayOfMonth = new Date(new Date(startDate.getFullYear(), (startDate.getMonth()), 1).toDateString());
      if (holidayOption) {
        noOfHolidays = this.getHolidaysCount(startDate, lastDayofMonth, locationId, resourceHolidays);
        if (startDate > firstDayOfMonth && endDate < lastDayofMonth) {
          daysWorked = this.dateService.getNumberOfWorkingDays(startDate, endDate) -
            this.getHolidaysCount(startDate, lastDayofMonth, locationId, resourceHolidays);
        }
        else if (startDate > firstDayOfMonth) { // if start date is greater than 1st day of month
          daysWorked = this.dateService.getNumberOfWorkingDays(startDate, lastDayofMonth) -
            this.getHolidaysCount(startDate, lastDayofMonth, locationId, resourceHolidays);
        }
        else {
          if (endDate < lastDayofMonth) { // if end date is in the same month
            daysWorked = this.dateService.getNumberOfWorkingDays(startDate, endDate) -
              this.getHolidaysCount(startDate, endDate, locationId, resourceHolidays);
          }
          else {
            daysWorked = this.dateService.getNumberOfWorkingDays(startDate, lastDayofMonth) -
              this.getHolidaysCount(startDate, lastDayofMonth, locationId, resourceHolidays);
          }

        }
        totalWorkingDays = this.dateService.getNumberOfWorkingDays(firstDayOfMonth, lastDayofMonth) -
          this.getHolidaysCount(firstDayOfMonth, lastDayofMonth, locationId, resourceHolidays);
        costHourFactor = daysWorked / totalWorkingDays;
      }
      else {
        if (startDate > firstDayOfMonth && endDate < lastDayofMonth) {
          daysWorked = this.dateService.getNumberOfWorkingDays(startDate, endDate);
        }
        else if (startDate > firstDayOfMonth) {
          daysWorked = this.dateService.getNumberOfWorkingDays(startDate, lastDayofMonth);
        }
        else {
          if (endDate < lastDayofMonth) { // if end date is in the same month
            daysWorked = this.dateService.getNumberOfWorkingDays(startDate, endDate);
          }
          else {
            daysWorked = this.dateService.getNumberOfWorkingDays(startDate, lastDayofMonth);
          }

        }
        totalWorkingDays = this.dateService.getNumberOfWorkingDays(firstDayOfMonth, lastDayofMonth);
        costHourFactor = daysWorked / totalWorkingDays;
      }
    }

    // Last Period Column
    else {
      lastDayofMonth = new Date(new Date(endDate.getFullYear(), (endDate.getMonth() + 1), 0).toDateString());
      firstDayOfMonth = new Date(new Date(endDate.getFullYear(), (endDate.getMonth()), 1).toDateString());
      if (holidayOption) {
        noOfHolidays = this.getHolidaysCount(firstDayOfMonth, endDate, locationId, resourceHolidays);
        daysWorked = this.dateService.getNumberOfWorkingDays(firstDayOfMonth, endDate) - noOfHolidays;
        totalWorkingDays = this.dateService.getNumberOfWorkingDays(firstDayOfMonth, lastDayofMonth)
          - this.getHolidaysCount(firstDayOfMonth, lastDayofMonth, locationId, resourceHolidays);
        costHourFactor = daysWorked / totalWorkingDays;
      }
      else {
        daysWorked = this.dateService.getNumberOfWorkingDays(firstDayOfMonth, endDate);
        totalWorkingDays = this.dateService.getNumberOfWorkingDays(firstDayOfMonth, lastDayofMonth);
        costHourFactor = daysWorked / totalWorkingDays;
      }
    }
    return costHourFactor.toFixed(4).toString();
  }

  getTotalHours(computedValue: number, totalHrsForTheMonth: number, daysWorked: number, noOfHolidays: number,
    noOfHolidaysInParticularMonth: number,
    totalNumberOfWorkingDays: number, totalHrs: number, resourceLocation: any, totalDaysInProjectDurationMonths: number,
    projectDurationInDays: number, selectedResourceLocationId: any, fte: number[],
    resourceMasterData: ResourcePlanningMasterData, currentMonthData: MonthlyData,
    index: number) {

    const calculatedValueArray: number[] = [];

    if (fte[index] !== 0) {
      if (resourceMasterData.sfBillingType === 'Flat Fee Monthly'
        || resourceMasterData.sfBillingType === 'Monthly fixed hours') {
        if (projectDurationInDays === totalDaysInProjectDurationMonths) {
          computedValue = fte[index] * (resourceLocation.codepermonth);
          totalHrs += computedValue;
        }
        else {
          if (currentMonthData.workingDays === currentMonthData.actualWorkingDays) {
            computedValue = fte[index] * (resourceLocation.codepermonth);
            totalHrs += computedValue;
          }
          else {
            if (resourceMasterData.isHolidayOptionOn) {
              noOfHolidays = currentMonthData.locationHolidays.filter(location =>
                location.locationId === selectedResourceLocationId)[index].noOfHolidays;
              noOfHolidaysInParticularMonth = currentMonthData.locationHolidays.filter(location =>
                location.locationId === selectedResourceLocationId)[index].noOfHolidaysInParticularMonth;
              totalNumberOfWorkingDays = currentMonthData.workingDays - noOfHolidaysInParticularMonth;
              totalHrsForTheMonth = resourceLocation.codepermonth / totalNumberOfWorkingDays;
              daysWorked = currentMonthData.actualWorkingDays - noOfHolidays;
              computedValue = fte[index] * (daysWorked * totalHrsForTheMonth);
              totalHrs += computedValue;
            }
            else {
              totalHrsForTheMonth = resourceLocation.codepermonth / currentMonthData.workingDays;
              daysWorked = currentMonthData.actualWorkingDays;
              computedValue = fte[index] * (daysWorked * totalHrsForTheMonth);
              totalHrs += computedValue;
            }
          }
        }
      }
      else {
        if (projectDurationInDays === totalDaysInProjectDurationMonths) {
          if (resourceMasterData.isHolidayOptionOn) {
            noOfHolidays = currentMonthData.locationHolidays.filter(location =>
              location.locationId === selectedResourceLocationId)[index].noOfHolidays;
            totalNumberOfWorkingDays = currentMonthData.workingDays - noOfHolidays;
            computedValue = fte[index] * (resourceLocation.codeperday * totalNumberOfWorkingDays);
            totalHrs += computedValue;
          }
          else {
            computedValue = fte[index] * (currentMonthData.workingDays * resourceLocation.codeperday);
            totalHrs += computedValue;
          }
        }
        else {
          if (currentMonthData.workingDays === currentMonthData.actualWorkingDays) {
            if (resourceMasterData.isHolidayOptionOn) {
              noOfHolidays = currentMonthData.locationHolidays.filter(location =>
                location.locationId === selectedResourceLocationId)[index].noOfHolidays;
              noOfHolidaysInParticularMonth = currentMonthData.locationHolidays.filter(location =>
                location.locationId === selectedResourceLocationId)[index].noOfHolidaysInParticularMonth;
              totalNumberOfWorkingDays = currentMonthData.workingDays - noOfHolidaysInParticularMonth;
              computedValue = fte[index] * (resourceLocation.codeperday * totalNumberOfWorkingDays);
              totalHrs += computedValue;
            }
            else {
              noOfHolidays = currentMonthData.locationHolidays.filter(location =>
                location.locationId === selectedResourceLocationId)[index].noOfHolidays;
              totalNumberOfWorkingDays = currentMonthData.workingDays;
              computedValue = fte[index] * (resourceLocation.codeperday * totalNumberOfWorkingDays);
              totalHrs += computedValue;
            }
          } else {
            if (resourceMasterData.isHolidayOptionOn) {
              noOfHolidays = currentMonthData.locationHolidays.filter(location =>
                location.locationId === selectedResourceLocationId)[index].noOfHolidays;
              totalNumberOfWorkingDays = currentMonthData.actualWorkingDays - noOfHolidays;
              computedValue = fte[index] * (resourceLocation.codeperday * totalNumberOfWorkingDays);
              totalHrs += computedValue;
            }
            else {
              computedValue = fte[index] * currentMonthData.actualWorkingDays * resourceLocation.codeperday;
              totalHrs += computedValue;
            }
          }
        }
      }
    }

    calculatedValueArray[0] = totalHrs;
    calculatedValueArray[1] = computedValue;
    return calculatedValueArray;
  }

  calculateInflatedCostHours(resourcePeriod: ResourcePeriod,
    projectPeriod: ProjectPeriod[], locationId: number): number {
    const yrsDifference: number = parseInt(projectPeriod.find(
      x => x.billingPeriodId === resourcePeriod.billingPeriodId).year.toString(), 0) - projectPeriod[0].year;
    let inflatedCostHours = resourcePeriod.costHours;
    for (let i = 0; i < yrsDifference; i++) {
      inflatedCostHours = inflatedCostHours + inflatedCostHours *
        (this.sharedDataService.sharedData.locationDTO.find(x => x.locationId === locationId).inflationRate / 100);
    }
    return inflatedCostHours;
  }

  calculateResourcePerHour(data: { key: number, ch: number, fte: number }[], rowIndex: number) {
    let totalCostValue = 0;
    let ftePerResource = 0;
    data.forEach(val => {
      if (val.key === rowIndex) {
        totalCostValue = totalCostValue + val.ch;
        ftePerResource = ftePerResource + val.fte;
      }
    });
    return { totalCostValue, ftePerResource };
  }

  calculateTotalFTEPerResource(resource: any): number {
    let totalFTE = 0;
    resource.value.periods.forEach(period => {
      totalFTE = totalFTE + parseFloat(period.FTEValue === '' ? 0 : period.FTEValue);
    });
    return totalFTE;
  }

  getTotalCostHours(resource: any): number {
    let totalCostHours = 0;
    resource.value.forEach(res => {
      res.periods.forEach(period => {
        totalCostHours += parseFloat(period.costHours);
      });
    });
    return totalCostHours;
  }

}
