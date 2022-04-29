import { HttpClient } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { LoggerService } from '../services';
import { EnvironmentConfig } from '../infrastructure';
import { MsalService } from '@azure/msal-angular';
import { Constants } from '@global/infrastructure/constants';
import { JwtHelperService } from '@auth0/angular-jwt';
import { isNullOrUndefined } from 'util';
import { IPipSheetWFStatusAndAccountSpecificRole } from '@shared/domain/IPipSheetWFStatusAndAccountSpecificRole';

@Injectable()
export class AuthService {

    private get msalService() { return this._injector.get(MsalService); }

    constructor(
        private logger: LoggerService,
        private _injector: Injector,
        private envConfig: EnvironmentConfig,
        private httpClient: HttpClient) {
        this.logger.info('AuthService : constructor ');

    }

    isUserLoggedIn(): boolean {
        if (this.getTokenExpiry()) {
            localStorage.clear();
            sessionStorage.clear();
            return false;
        }
        else {
            return !!this.msalService.getAccount();
        }
        // return !!this.msalService.getAccount();
    }

    logOut(): void {
        this.msalService.logout();
        localStorage.clear();
        sessionStorage.clear();
        // this.utilityService.redirectToURL(environment.appUrl);
    }

    refreshToken() {
        const epochDiffInMins = this.getEpochDiffInMinutes();
        if (epochDiffInMins > 10) {
            return false;
        }
        const request = { scopes: this.envConfig.scopes };
        return this.msalService.acquireTokenSilent(request)
            .then(token => {
                localStorage.setItem(Constants.localStorageKeys.accessToken, token.accessToken);
            }).catch(error => {
                // fallback to interaction when silent call fails
                return this.msalService.acquireTokenPopup(request)
                    .then(tokenResponse => {
                        localStorage.setItem(Constants.localStorageKeys.accessToken, tokenResponse.accessToken);
                        return tokenResponse;
                    }).catch(err => {
                        this.logger.info(err);
                    });
            });
    }

    getEpochDiffInMinutes(): number {
        const jwtHelper = new JwtHelperService();
        // TODO confirm if we need idToken only or something else
        const token = localStorage.getItem(Constants.localStorageKeys.accessToken);

        const decodedToken = jwtHelper.decodeToken(token);
        const tokenExpiresIn = decodedToken['exp'];
        const currentEpochTime = Math.floor(new Date().getTime() / 1000.0);
        return Math.floor((tokenExpiresIn - currentEpochTime) / 60); // Epoch time difference in minutes
    }

    getTokenExpiry(): boolean {
        const token = localStorage.getItem(Constants.localStorageKeys.accessToken);
        if (!isNullOrUndefined(token)) {
            const jwtHelper = new JwtHelperService();
            return jwtHelper.isTokenExpired(token);
        }
        else {
            return false;
        }
    }

    isUserAuthorised(pipSheetId: any, accountId: any, projectId?: any) {
      return this.httpClient.get<IPipSheetWFStatusAndAccountSpecificRole>(Constants.webApis.getWorkflowStatusAccountRole
        .replace('{pipSheetId}', pipSheetId).replace('{accountId}', accountId).replace('{projectId}', projectId.toString())).toPromise();
    }
}
