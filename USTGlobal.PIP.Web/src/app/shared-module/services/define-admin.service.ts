import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';

@Injectable({
  providedIn: 'root'
})
export class DefineAdminService {

  constructor(private httpClient: HttpClient) { }

  getAdminData(accountId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getAdmins.replace('{accountId}', accountId));
  }

  getUsers(): Observable<any> {
    return this.httpClient.get(Constants.webApis.getUsers);
  }

  deleteUserRole(userId: any, accountId: any): Observable<any> {
    const fromAdminScreen = 'true';
    return this.httpClient.delete(Constants.webApis.deleteUserRole
      .replace('{userId}', userId).replace('{accountId}', accountId).replace('{fromAdminScreen}', fromAdminScreen));
  }

  assignAdminRole(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.assignAdminRole, data);
  }
}
