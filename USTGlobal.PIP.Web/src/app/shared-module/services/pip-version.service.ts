import { IPipMainVersion } from './../domain/IPipMainVersion';
import { IPipCheckIn } from './../domain/IPipCheckIn';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPipVersion } from '@shared/domain/IPipVersion';
import { Constants } from '@shared/infrastructure/constants';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class PipVersionService {

  constructor(private httpClient: HttpClient) { }

  getPipVersions(projectId: string): Observable<IPipMainVersion> {
    return this.httpClient.get<IPipMainVersion>(Constants.webApis.getAllPipVersions.replace('{projectId}', projectId));
  }

  deletePipSheet(pipSheetId: any, projectId: any): Observable<any> {
    return this.httpClient.delete<IPipVersion>(Constants.webApis.deletePipSheet
      .replace('{pipSheetId}', pipSheetId).replace('{projectId}', projectId));
  }

  updatePIPSheetCheckIn(pipCheckIn: IPipCheckIn): Observable<number> {
    return this.httpClient.put<number>(Constants.webApis.updatePIPSheetCheckIn, pipCheckIn);
  }
}
