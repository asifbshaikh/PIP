import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IKeyPerformanceIndicators } from '@shared/domain/IKeyPerformanceIndicators';
import { Constants } from '@shared/infrastructure';

@Injectable({
  providedIn: 'root'
})
export class PerformanceIndicatiorsService {

  constructor(private httpClient: HttpClient) { }

  getKeyPerformanceIndicatorsData(pipSheetId: any): Observable<IKeyPerformanceIndicators> {
    return this.httpClient.get<IKeyPerformanceIndicators>(Constants.webApis.getKeyPerformanceIndicators
      .replace('{pipSheetId}', pipSheetId));
  }
}
