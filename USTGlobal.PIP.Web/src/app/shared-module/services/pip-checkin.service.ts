import { ICheckOutPipVersion } from './../domain/ICheckOutPipVersion';
import { IPipCheckInProject } from '../domain/IPipCheckInProject';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure/constants';


@Injectable({
  providedIn: 'root'
})

export class PipCheckinService {
  constructor(
    private httpClient: HttpClient
  ) { }

  getProjects(accountId: any): Observable<IPipCheckInProject[]> {
    return this.httpClient.get<IPipCheckInProject[]>(Constants.webApis.getProjects.replace('{accountId}', accountId));
  }

  getVersions(projectId: any): Observable<ICheckOutPipVersion[]> {
    return this.httpClient.get<ICheckOutPipVersion[]>(Constants.webApis.getCheckedOutVersions.replace('{projectId}', projectId));
  }

  SaveCheckedInVersions(selectedVersions: ICheckOutPipVersion[]): Observable<ICheckOutPipVersion[]> {
    return this.httpClient.post<ICheckOutPipVersion[]>(Constants.webApis.saveCheckedInVersions, selectedVersions);
  }
}
