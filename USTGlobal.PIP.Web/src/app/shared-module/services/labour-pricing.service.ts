import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ILaborPricing } from '@shared/domain/laborpricing';
import { HttpClient } from '@angular/common/http';
import { Constants } from '@shared/infrastructure';
import { LaborPricingMapper } from '@shared/mapper/master/laborpricingmapper';
import { IBackgroundCalculations } from '@shared/domain/IBackgroundCalculations';
import { ILabor, ICorpBillingRate, ProjectPeriod } from '@shared/domain';
import { isNullOrUndefined } from 'util';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';
import { map } from 'rxjs/internal/operators';
import { IBackgroundParam } from '@shared/domain/IBackgroundParam';

@Injectable({
  providedIn: 'root'
})
export class LabourPricingService {
  inflationPerResource = 0;
  inflatedCappedCost = 0;

  constructor(private httpClient: HttpClient) { }

  getLaborPricingDetails(pipSheetId: number): Observable<LaborPricingMapper> {
    return this.httpClient.get<ILaborPricing>(Constants.webApis.getLaborPricingData + pipSheetId)
      .pipe(map(data => new LaborPricingMapper().mapper(data)));
  }

  saveLaborPricingDetails(data: ILaborPricing): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveLaborPricingData, JSON.stringify(data));
  }

  getBackgroundCalculations(backgroundParam: IBackgroundParam): Observable<IBackgroundCalculations> {
    return this.httpClient.post<IBackgroundCalculations>(Constants.webApis.getBackgroundCalculations, backgroundParam);
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }

  // Technical depth fixes  :

  computeTotalStaffHours(data: ILabor[]) {
    let staffHours = 0;
    data.forEach(item => {
      staffHours = staffHours + item.totalHoursPerResource;
    });
    return staffHours;
  }


  computeTotalMargin(totalRevenue: number, cappedCost: number) {
    const marginPercentage = ((totalRevenue - cappedCost) / totalRevenue) * 100;
    return marginPercentage === (-Infinity || Infinity) ? 0 : marginPercentage;
  }

  calculateMarginPercent(totalRevenue: number, cappedCost: number): number {
    let margin = 0, marginPercent = 0;
    if (totalRevenue === 0) {
      marginPercent = 0;
    } else {
      margin = + ((totalRevenue - cappedCost) / totalRevenue);
      marginPercent = + (margin * 100);
    }
    return parseFloat(marginPercent.toFixed(2));
  }


  // inflation work

  checkInflation(laborPricingDetails: any): boolean {
    const ProjectYears = this.getYearsinProject(laborPricingDetails);
    let isApplicable = false;

    if (ProjectYears.length > 0) {
      isApplicable = true;
    }
    return isApplicable;
  }

  getYearsinProject(laborPricingDetails: any) {
    const years = laborPricingDetails.projectPeriodDTO.filter(
      (period, index, periods) => periods.findIndex(t => t.year === period.year) === index
    );

    return years.sort();
  }

  computeTotalCappedCostWithoutInflationPerResource(laborPricingDetails: LaborPricingMapper): number {
    let totalInflation = 0;
    laborPricingDetails.resourceLaborPricingDTOs.forEach(res => {
      totalInflation += +res.totalInflation;
    });
    return totalInflation;
  }


}
