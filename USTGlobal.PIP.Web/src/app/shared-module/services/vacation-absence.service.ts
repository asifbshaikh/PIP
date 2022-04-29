import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared';
import { IVacationAbsence } from '@shared/domain/IVacationAbsence';
import { IPeriodLostRevenue } from '@shared/domain';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';

@Injectable({
  providedIn: 'root'
})

export class VacationAbsenceService {
  constructor(
    private httpClient: HttpClient,
  ) {
  }

  getVacationAbsence(pipSheetId: number): Observable<IVacationAbsence> {
    return this.httpClient.get<IVacationAbsence>(Constants.webApis.getVacationAbsence + pipSheetId);
  }

  saveVacationAbsence(vacationAbsenceData: IVacationAbsence): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveVacationAbsence, vacationAbsenceData);
  }

  calculateTotalLostRevenue(periodArray: any, lostRevenue: number, isOverride: boolean): any {
    let totalLostRevenue = 0, overrideDifference = 0;
    periodArray.forEach(period => {
      if (period.lostRevenue) {
        totalLostRevenue += parseFloat(period.lostRevenue);
      }
    });
    totalLostRevenue = totalLostRevenue;
    overrideDifference = Math.abs(Math.abs(lostRevenue) - Math.abs(totalLostRevenue));
    return { totalLostRevenue, overrideDifference };
  }

  calculatePeriodWiseLostRevenue(periodArray: any, totalRevenue: number, lostRevenue: number) {
    const periodWiseLostRevenue = new Array(periodArray.length);
    periodWiseLostRevenue.fill(0);
    let periodLostRevenue = 0;
    let totalLostRevenue = 0;
    periodArray.forEach((period, index) => {
      periodLostRevenue = (lostRevenue * period.revenue) / totalRevenue;
      periodWiseLostRevenue[index] = periodLostRevenue;
    });
    totalLostRevenue = periodWiseLostRevenue.reduce((a, b) => a + b, 0);
    return { totalLostRevenue, periodWiseLostRevenue };
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }
}
