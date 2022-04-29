import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';

@Injectable({
  providedIn: 'root'
})
export class ApproverService {

  constructor(private httpClient: HttpClient) { }

  getApproversData(): Observable<any> {
    return this.httpClient.get(Constants.webApis.getApproversData);
  }
}
