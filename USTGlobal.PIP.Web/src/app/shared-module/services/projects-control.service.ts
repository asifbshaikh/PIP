import { IPipSheetWorkflowStatus } from './../domain/IPipSheetWorkflowStatus';
import { Http } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ILocation } from '@shared/domain/locations';
import { IProjectControl } from '@shared/domain/IProjectControl';
import { Constants } from '@shared';
import { SharedDataService } from '@global';
import { IProjectMilestoneGroup } from '@shared/domain/IProjectMilestoneGroup';
import { ICurrency, IProjectMilestone } from '@shared/domain';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';

@Injectable({
  providedIn: 'root'
})

export class ProjectControlService {
  constructor(
    private httpClient: HttpClient,
    private sharedDataService: SharedDataService
  ) {
  }
  getLocations(projectId: any, pipSheetId: any): Observable<ILocation[]> {
    return this.httpClient.get<ILocation[]>(Constants.webApis.getAllLocations.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }

  getProjectControlData(pipSheetId: any): Observable<IProjectControl> {
    return this.httpClient.get<IProjectControl>(Constants.webApis.getAllProjectControlData.replace('{pipSheetId}', pipSheetId));
  }

  saveProjectControlData(pipSheet: IProjectControl): Observable<IProjectControl> {
    return this.httpClient.post<IProjectControl>(Constants.webApis.saveProjectControlData, JSON.stringify(pipSheet));
  }

  getProjectMilestoneGroup(pipSheetId: number): Observable<IProjectMilestoneGroup[]> {
    return this.httpClient.get<IProjectMilestoneGroup[]>(Constants.webApis.getAllProjectMilestonesGroups + pipSheetId);
  }

  getCurrencyConversionData(pipSheetId: any): Observable<ICurrency> {
    return this.httpClient.get<ICurrency>(Constants.webApis.getCurrencyConversionData.replace('{pipSheetId}', pipSheetId));
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }
}
