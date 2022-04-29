import { OtherPriceAdjustment } from './../domain/otherpriceadjustment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IOtherPriceAdjustment } from '@shared/domain/IOtherPriceAdjustment';
import { ProjectPeriod, Constants } from '@shared';
import { Observable } from 'rxjs';
import { IPeriodOtherPriceAdjustment } from '@shared/domain/IPeriodOtherPriceAdjustment';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';
@Injectable({
  providedIn: 'root'
})
export class OtherpriceadjustmentService {

  constructor(private httpClient: HttpClient) { }

  getOtherPriceAdjustment(pipSheetId: any): Observable<OtherPriceAdjustment> {
    return this.httpClient.get<OtherPriceAdjustment>(Constants.webApis.getOtherPriceAdjustment.replace('{pipSheetId}', pipSheetId));
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }


  getDefaultOtherPriceAdjustment(periods: ProjectPeriod[], pipSheetId: number, firstRow: IOtherPriceAdjustment): IOtherPriceAdjustment[] {
    const otherPriceAdjustment: IOtherPriceAdjustment[] = [];
    otherPriceAdjustment.push(firstRow);
    for (let i = 0; i < 2; i++) {
      otherPriceAdjustment.push({
        uId: 0,
        otherPriceAdjustmentId: 0,
        pipSheetId: pipSheetId,
        milestoneId: -1,
        description: '',
        totalRevenue: 0,
        isDeleted: false,
        createdBy: 1,
        updatedBy: 1,
        rowType: 3,
        otherPriceAdjustmentPeriodDetail: this.computeOtherPriceAdjustmentPeriods(periods)
      });
    }
    return otherPriceAdjustment;
  }

  addOtherPriceAdjustmentRow(pipSheetId: number, periods: ProjectPeriod[]): IOtherPriceAdjustment {
    return {
      uId: 0,
      otherPriceAdjustmentId: 0,
      pipSheetId: pipSheetId,
      milestoneId: -1,
      description: '',
      totalRevenue: 0,
      isDeleted: false,
      createdBy: 1,
      updatedBy: 1,
      rowType: 3,
      otherPriceAdjustmentPeriodDetail: this.computeOtherPriceAdjustmentPeriods(periods)
    };
  }

  private computeOtherPriceAdjustmentPeriods(periods: ProjectPeriod[]): IPeriodOtherPriceAdjustment[] {
    const periodWiseRevenue: IPeriodOtherPriceAdjustment[] = [];
    periods.forEach(period => {
      periodWiseRevenue.push({
        uId: 0,
        otherPriceAdjustmentId: 0,
        billingPeriodId: period.billingPeriodId,
        revenue: 0
      });
    });
    return periodWiseRevenue;
  }

  saveOtherPriceAdjustment(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveOtherPriceAdjustment, data);
  }

  costCalculations(otherPriceAdjustmentData: IOtherPriceAdjustment[], projectPeriod: ProjectPeriod[]) {
    const amountPaidTotals = new Array(otherPriceAdjustmentData.length);
    const periodTotals = new Array(projectPeriod.length);
    periodTotals.fill(0);
    amountPaidTotals.fill(0);
    otherPriceAdjustmentData.forEach((costData, costIndex) => {
      if (!costData.isDeleted) {
        costData.otherPriceAdjustmentPeriodDetail.forEach((periodRevenue, periodIndex) => {
          amountPaidTotals[costIndex] += +periodRevenue.revenue;
          periodTotals[periodIndex] += +periodRevenue.revenue;
        });
      }
    });
    return { amountPaidTotals, periodTotals };
  }

  calculatetotalAdjustedRevenuePeriodWise(otherPriceAdjustmentData: IOtherPriceAdjustment[], projectPeriod: ProjectPeriod[]) {
    const amountPaidTotals = new Array(otherPriceAdjustmentData.length - 1);
    const periodTotals = new Array(projectPeriod.length);
    periodTotals.fill(0);
    amountPaidTotals.fill(0);
    otherPriceAdjustmentData.forEach((costData, costIndex) => {
      if (!costData.isDeleted && costIndex > 0) {
        costData.otherPriceAdjustmentPeriodDetail.forEach((periodRevenue, periodIndex) => {
          amountPaidTotals[costIndex] += +periodRevenue.revenue;
          periodTotals[periodIndex] += +periodRevenue.revenue;
        });
      }
    });
    return { amountPaidTotals, periodTotals };
  }
}
