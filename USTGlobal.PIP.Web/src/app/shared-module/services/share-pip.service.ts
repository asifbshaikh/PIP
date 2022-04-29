import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';
import { ISharePIP } from '@shared/domain/ISharePIP';
import { ISharePipVersion } from '@shared/domain/ISharePipVersion';


@Injectable({
  providedIn: 'root'
})

export class SharePipService {
  constructor(private httpClient: HttpClient) { }

  getSharePipListData(projectId: number): Observable<any> {
    return this.httpClient.get<any>(Constants.webApis.getSharePipListData + projectId);
  }

  updateSharePipListData(sharePip: ISharePIP): Observable<any> {
    return this.httpClient.put(Constants.webApis.updateSharePipListData, sharePip);
  }

  deleteSharePipListData(pipSheetId: any, roleId: any, accountId: any, sharedWithUserId: any): Observable<any> {
    return this.httpClient.delete(Constants.webApis.deleteSharePipListData.replace('{pipSheetId}', pipSheetId).replace('{roleId}', roleId)
      .replace('{accountId}', accountId).replace('{sharedWithUserId}', sharedWithUserId));
  }

  getSharePipVersionData(projectId: any) {
    return this.httpClient.get<ISharePipVersion>(Constants.webApis.getSharePipVersionData.replace('{projectId}', projectId));
  }

  saveSharedPipData(sharePip: ISharePIP[]): Observable<ISharePIP[]> {
    return this.httpClient.post<ISharePIP[]>(Constants.webApis.saveSharedPipData, sharePip);
  }
}
