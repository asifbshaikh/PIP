import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IUserRole } from '@shared/domain/IUserRole';
import { ISharedPipRole } from '@shared/domain/ISharedPipRole';
import { Constants } from '@shared';

@Injectable({
  providedIn: 'root'
})
export class UserRoleService {

  constructor(private httpClient: HttpClient) { }

  getUsersRoles(accountId: any) {
    return this.httpClient.get<IUserRole[]>(Constants.webApis.getUserRoles.replace('{accountId}', accountId));
  }

  getAllUsersAndAssociatedRoles() {
    return this.httpClient.get<IUserRole[]>(Constants.webApis.getAllUsersAndAssociatedRoles);
  }

  saveUserRoles(userRole: IUserRole): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveUserRoles, userRole);
  }

  saveSharedPipRole(sharedPipRole: ISharedPipRole): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveSharedPipRole, sharedPipRole);
  }
}
