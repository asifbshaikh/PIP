import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { Injectable } from '@angular/core';
import 'rxjs/add/operator/toPromise';
import { Constants } from '../infrastructure/constants';
import { HttpClient } from '@angular/common/http';
import { AuthService, LoggerService } from '@core';
import { ISharedData } from '@global';
import { isNullOrUndefined } from 'util';
import { ICheckRole } from '@shared/domain/ICheckRole';
import { environment } from '@env';

@Injectable()
export class SharedDataService {

  public sharedData: ISharedData;
  public isDisableUIElements: boolean;
  public userId: any;
  public showProjectMilestone: boolean;
  public roleAndAccount: IRoleAndAccount[];
  isRegisteredUSer = true;
  public isDummy: boolean;

  constructor(
    private logger: LoggerService,
    private authService: AuthService,
    private httpClient: HttpClient
  ) {
    this.logger.info('SharedDataService : constructor ');
  }

  populateCommonData(pipSheetId: any): Promise<any> {

    this.logger.info('SharedDataService : populateCommonData ');

    if (!this.authService.isUserLoggedIn()) {
      return;
    }

    if (this.isRegisteredUSer) {
      const promise = this.httpClient.get<ISharedData>(Constants.webApis.getSharedData.replace('{pipSheetId}', pipSheetId))
      .toPromise();

      promise.then(
        successResponse => {
          this.logger.info('SharedDataService : populateCommonData : successResponse ' + successResponse);
          this.sharedData = successResponse;
        })
        .catch(
          errorResponse => {
            this.logger.info('****** SharedDataService : populateCommonData : server returned error');
            this.logger.info('errorResponse : ' + errorResponse);
          });
      return promise;
    }
  }

  getUserSpecificRoles(roleAndAccount: IRoleAndAccount[]): ICheckRole {
    const checkRole: ICheckRole = {
      isAdmin: false, isEditor: false, isFinanaceApprover: false,
      isReviewer: false, isReadOnly: false
    };

    roleAndAccount.forEach(roleAcc => {

      if (roleAcc.roleName === 'Admin') {
        checkRole.isAdmin = true;
      }
      else if (roleAcc.roleName === 'Editor') {
        checkRole.isEditor = true;
      }
      else if (roleAcc.roleName === 'Finance Approver') {
        checkRole.isFinanaceApprover = true;
      }
      else if (roleAcc.roleName === 'Reviewer') {
        checkRole.isReviewer = true;
      }
      else if (roleAcc.roleName === 'Readonly') {
        checkRole.isReadOnly = true;
      }
    });

    return checkRole;
  }


}

