import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';


@Injectable({
  providedIn: 'root'
})
export class AddNewLocationService {

  constructor(private httpClient: HttpClient) { }

  getAllLocations(): Observable<any> {
    return this.httpClient.get(Constants.webApis.getLocations);
  }

  saveNewLocation(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveNewLocation, data);
  }

  getPastLocationVersions(locationId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getPastLocationVersions.replace('{locationId}', locationId));
  }

  deleteRejectedLocation(locationId: any): Observable<any> {
    return this.httpClient.delete(Constants.webApis.deleteRejectedLocation.replace('{locationId}', locationId));
  }

  getInactiveLocationVersion(locationId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getInActiveLocationVersions.replace('{locationId}', locationId));
  }

}
