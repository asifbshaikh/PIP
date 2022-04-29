import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';
import { HttpClient } from '@angular/common/http';
import { IProject } from '@shared/domain';

@Injectable({
  providedIn: 'root'
})
export class ReplicateService {

  constructor(private httpClient: HttpClient) { }

  createReplicatePipSheet(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.createReplicatePipSheet, data);
  }
  getProjectListBasedOnAccountId(accountId: any): Observable<IProject[]> {
    return this.httpClient.get<IProject[]>(Constants.webApis.getProjectListBasedOnAccountId.replace('{accountId}', accountId));
  }
}
