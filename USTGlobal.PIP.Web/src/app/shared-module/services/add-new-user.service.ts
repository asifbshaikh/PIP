import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';
import { IUserListResult } from '@shared/domain/IUserListResult';

@Injectable({
  providedIn: 'root'
})
export class AddNewUserService {

  constructor(private httpClient: HttpClient) { }

  saveUserData(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveUserData, data);
  }

  getUsers(): Observable<any> {
    return this.httpClient.get(Constants.webApis.getUsers);
  }

  UploadMultipleUserData(formData: FormData): Observable<any> {
    return this.httpClient.post<IUserListResult>(Constants.webApis.uploadMultipleUserData, formData, { responseType: 'json' });
  }
}
