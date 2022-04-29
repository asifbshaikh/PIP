import { IPipSheetWFStatusAndAccountSpecificRole } from './../domain/IPipSheetWFStatusAndAccountSpecificRole';
import { IPipSheetWorkflowStatus } from './../domain/IPipSheetWorkflowStatus';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';
import { IHeader1 } from './../domain/IHeader1';
import { ICurrency } from './../domain/ICurrency';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared';
import { IEbitda } from '@shared/domain/IEbitda';

@Injectable({
  providedIn: 'root'
})

export class EbitdaService {
  constructor(private httpClient: HttpClient) {
  }

  saveEbitdaData(ebitdaList: IEbitda[]): Observable<number> {
    return this.httpClient.put<number>(Constants.webApis.saveEbitdaData, ebitdaList);
  }

  getEbitdaData(pipSheetId: any): Observable<IEbitda[]> {
    return this.httpClient.get<IEbitda[]>(Constants.webApis.getEbitdaData.replace('{pipSheetId}', pipSheetId));
  }

  getCurrencyConversionData(pipSheetId: any): Observable<ICurrency> {
    return this.httpClient.get<ICurrency>(Constants.webApis.getCurrencyConversionData.replace('{pipSheetId}', pipSheetId));
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }

  getWorkflowStatusAccountRole(pipSheetId: any, accountId: any):
    Observable<IPipSheetWFStatusAndAccountSpecificRole> {
      const projectId = 0;
      return this.httpClient.get<IPipSheetWFStatusAndAccountSpecificRole>(Constants.webApis.getWorkflowStatusAccountRole
        .replace('{pipSheetId}', pipSheetId).replace('{accountId}', accountId).replace('{projectId}', projectId.toString()));
  }
}
