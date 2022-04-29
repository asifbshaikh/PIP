import { IAccountId } from './../domain/IAccountId';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IProjectListForAccounts } from '@global/domain/IProjectListForAccount';
import { Constants } from '@shared/infrastructure';
import { IReportKPI } from '@shared/domain/IReportKPI';
import { IGenerateReport } from '@shared/domain/IGenerateReport';
import { ISelectedAccount } from '@shared/domain/ISelectedAccount';
import { ISelectedKPI } from '@shared/domain/ISelectedKPI';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {

  constructor(private httpClient: HttpClient) { }

  getProjectListForAccount(accountDTO: ISelectedAccount[]): Observable<IProjectListForAccounts[]> {
    return this.httpClient.post<IProjectListForAccounts[]>(Constants.webApis.getProjectListForAccount, accountDTO);
  }

  generateReport(report: IGenerateReport): Observable<any> {
    return this.httpClient.post<any>(Constants.webApis.generateProjectSummaryReport, report);
  }

  getReportKPIList(): Observable<ISelectedKPI[]> {
    return this.httpClient.get<ISelectedKPI[]>(Constants.webApis.getReportKPIList);
  }

  getAuthorizedAccounts(): Observable<IAccountId[]> {
    return this.httpClient.get<IAccountId[]>(Constants.webApis.getAuthorizedAccounts);
  }
}
