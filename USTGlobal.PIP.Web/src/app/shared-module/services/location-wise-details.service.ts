import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ILocationWiseDetailsSummary } from '@shared/domain/ILocationWiseDetailsSummary';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';

@Injectable({
  providedIn: 'root'
})
export class LocationWiseDetailsService {

  constructor(private httpClient: HttpClient) { }

  getLocationWiseDetails(pipSheetId: any): Observable<ILocationWiseDetailsSummary> {
    return this.httpClient.get<ILocationWiseDetailsSummary>(Constants.webApis.getLocationWiseDetails.replace('{pipSheetId}', pipSheetId));
  }
}
