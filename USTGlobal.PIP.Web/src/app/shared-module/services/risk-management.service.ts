import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IRiskManagement } from '@shared/domain/IRiskManagement';
import { Constants } from '@shared/infrastructure/constants';
import { Observable } from 'rxjs';
import { ProjectPeriod } from '@shared/domain';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';


@Injectable({
  providedIn: 'root'
})
export class RiskManagementService {
  constructor(private httpClient: HttpClient) {
  }

  getRiskmanagementDetails(pipSheetId: number) {
    return this.httpClient.get<IRiskManagement>(Constants.webApis.getRiskManagementData + pipSheetId);
  }

  saveRiskManagementData(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveRiskManagementData, data);
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }

  calculateTotalRiskOverrun(percent: number, M155_costSubTotal: number): number {
    let riskOverrun = 0;
    riskOverrun = (percent / 100) * M155_costSubTotal;
    return Math.abs(riskOverrun);
  }

  calculateH156(fixBidPercent: number, M155_costSubTotal: number): number {
    let h156 = 0;
    h156 = (fixBidPercent / 100) * M155_costSubTotal;
    return Math.abs(h156);
  }

  calculateH157(percent: number, M155_costSubTotal: number): number {
    let h157 = 0;
    h157 = (percent / 100) * M155_costSubTotal;
    return Math.abs(h157);
  }

  calculateFeesAtRiski159(feesAtRisk: number): number {
    return feesAtRisk / 100;
  }

  calculateRevenue(feesAtRiski159: number, clinetPrice: number): number {
    return -1 * (feesAtRiski159 * 1);
  }

  calculatePeriodWiseRisk(projectPeriod: ProjectPeriod[], totalAssesedRiskOverrun: number, totalCappedCost: number): Array<number> {
    const periodRisks = new Array(projectPeriod.length);
    periodRisks.fill(0);
    projectPeriod.forEach((period, index) => {
      if (totalCappedCost !== 0 && totalCappedCost !== null) {
        periodRisks[index] = totalAssesedRiskOverrun * (period.cappedCost / totalCappedCost);
      }
      else {
        periodRisks[index] = totalAssesedRiskOverrun / projectPeriod.length;
      }
    });
    return periodRisks;
  }

  calculateNetEstimatedRevenue(totalClientPrice: number, revenue: number): number {
    return totalClientPrice + revenue;
  }

  calculateOverrideDifference(periodRisk: any[], riskOverrun: number): number {
    let totalOverridenRisk = 0;
    periodRisk.forEach(period => {
      totalOverridenRisk += +period.riskAmount;
    });
    totalOverridenRisk = +totalOverridenRisk.toFixed(2);
    riskOverrun = riskOverrun ? +riskOverrun.toFixed(2) : 0;
    return totalOverridenRisk - riskOverrun;
  }
}
