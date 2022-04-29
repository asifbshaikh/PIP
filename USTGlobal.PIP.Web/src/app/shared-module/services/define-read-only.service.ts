import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';

@Injectable({
  providedIn: 'root'
})
export class DefineReadOnlyService {

  constructor(private httpClient: HttpClient) { }

  getReadOnlyData(): Observable<any> {
    return this.httpClient.get(Constants.webApis.getReadOnlyUsers);
  }

  getUsers(): Observable<any> {
    return this.httpClient.get(Constants.webApis.getUsers);
  }

  deleteReadOnlyRole(userId: any): Observable<any> {
    return this.httpClient.delete(Constants.webApis.deleteReadOnlyRole.replace('{userId}', userId));
  }

  assignAdminRole(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.assignReadOnlyRole, data);
  }
}
