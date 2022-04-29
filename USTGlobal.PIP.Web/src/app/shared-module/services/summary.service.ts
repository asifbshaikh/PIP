import { ISubmitPipSheetReturnType } from './../domain/ISubmitPipSheetReturnType';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';
import { SharedDataService } from '@global';
import { ProjectPeriod, ISubmitPipSheet, IplForcast } from '@shared/domain';
import { IPipVersionSummaryDetail } from '@shared/domain/IPipVersionSummaryDetail';
import { IcashFlowParentDTO } from '@shared/domain/IcashFlowParentDTO';

@Injectable({
  providedIn: 'root'
})

export class SummaryService {
  constructor(private httpClient: HttpClient, private sharedDataService: SharedDataService) {
  }

  getSummaryData(pipSheetId: any): Observable<any> {
    return this.httpClient.get<any>(Constants.webApis.getSummaryData.replace('{pipSheetId}', pipSheetId));
  }

  getGrossProfitData(pipSheetId: any): Observable<any> {
    return this.httpClient.get<any>(Constants.webApis.getGrossProfit.replace('{pipSheetId}', pipSheetId));
  }

  getInvestmentData(pipSheetId: any): Observable<any> {
    return this.httpClient.get<any>(Constants.webApis.getInvestment.replace('{pipSheetId}', pipSheetId));
  }

  saveInvestmentPercent(saveDataObject): Observable<any> {
    return this.httpClient.post<any>(Constants.webApis.saveInvestment, saveDataObject);
  }

  getEffortSummaryData(pipSheetId: any): Observable<any> {
    return this.httpClient.get<any>(Constants.webApis.getEffortSummary.replace('{pipSheetId}', pipSheetId)
    );
  }

  getBillingSchedule(pipSheetId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getBillingSchedule.replace('{pipSheetId}', pipSheetId));
  }

  getTotalDealFinancialData(pipSheetId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getTotalDealFinancials.replace('{pipSheetId}', pipSheetId));
  }

  getDefaultBillingSchedule(periods: ProjectPeriod[], pipSheetId: number, totalClientPrice: IcashFlowParentDTO[]): IcashFlowParentDTO[] {
    for (let i = 0; i < 4; i++) {
      totalClientPrice.push({
        cashFlowDTO: [],
        clientPriceId: 0,
        descriptionId: 0,
        isOverrideUpdated: false,
        pipSheetId: 0,
        totalPrice: 0,
        uId: 0,
      });
    }
    return totalClientPrice;
  }

  getPLForecast(pipSheetId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getPLForecast.replace('{pipSheetId}', pipSheetId));
  }

  getYearComparison(pipSheetId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getYearComparison.replace('{pipSheetId}', pipSheetId));
  }

  submitPipSheet(pipSheet: ISubmitPipSheet): Observable<ISubmitPipSheetReturnType> {
    return this.httpClient.post<ISubmitPipSheetReturnType>(Constants.webApis.submitPipSheet, JSON.stringify(pipSheet));
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }

  getPipVersionDetailsOnSummary(pipSheetId: any): Observable<IPipVersionSummaryDetail> {
    return this.httpClient.get<IPipVersionSummaryDetail>(Constants.webApis.getPipVersionSummaryDetails.replace('{pipSheetId}', pipSheetId));
  }

  getPIPSheetStatus(pipSheet: ISubmitPipSheet): Observable<ISubmitPipSheetReturnType> {
    return this.httpClient.post<ISubmitPipSheetReturnType>(Constants.webApis.getPipSheetStatus, JSON.stringify(pipSheet));
  }
  savePlForeCastData(plforecast: IplForcast, pipSheetId: any): Observable<IplForcast> {
    return this.httpClient.post<IplForcast>(Constants.webApis.savePlForeCastData.replace('{pipSheetId}', pipSheetId), plforecast);
  }
}
