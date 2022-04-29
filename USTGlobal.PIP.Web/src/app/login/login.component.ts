 import {
     Component,
     OnInit,
     OnDestroy,
 } from '@angular/core';

import { LoggerService, AuthService } from '@core';

 import {
     UtilityService,
} from '@core';

import { LoginModel } from './login.model';
import { LoginService } from './login.service';
import { environment } from '@env';
import { SharedDataService } from '@global';
import { isNullOrUndefined } from 'util';
import { MsalService, BroadcastService } from '@azure/msal-angular';
import { config } from '../../auth-settings';
import { Logger, CryptoUtils } from 'msal';
import { Subscription } from 'rxjs';
import { Constants } from '@shared';

@Component({
    moduleId: module.id,
    selector: 'app-login',
    templateUrl: 'login.component.html',
    providers: [LoginService]
})
export class LoginComponent implements OnInit, OnDestroy {

    model: LoginModel;
    showLogin = false;
    displayAccessDenied = false;
    displayIsActive = false;
    isUserLoggedIn = false;
    private subscription: Subscription;

    constructor(
        private loginService: LoginService,
        private logger: LoggerService,
        private utilityService: UtilityService,
        public msalService: MsalService,
        private shared: SharedDataService,
        private authService: AuthService,
        private broadcastService: BroadcastService
    ) {
        this.logger.info('LoginComponent : constructor ');
        this.model = new LoginModel();
        this.model.isAuthInitiated = false;
    }

    ngOnInit() {
        this.logger.info('LoginComponent : ngOnInit ');

        this.showLogin = true;

        this.checkoutAccount();

        this.subscription = this.broadcastService.subscribe('msal:loginSuccess', () => {
            this.checkoutAccount();

            if (this.isUserLoggedIn) {
                this.loginService.getUserData().subscribe(userData => {
                    if (isNullOrUndefined(userData)) {
                        this.shared.isRegisteredUSer = false;
                        this.showAccessDeniedDialog();
                    }
                    else if (!userData.isActive) {
                        this.showIsActiveDialog();
                    }
                    else if (userData.role.length === 0) {
                        this.shared.isRegisteredUSer = false;
                        this.showAccessDeniedDialog();
                    }
                    else {
                        this.shared.isRegisteredUSer = true;
                        this.onLoginSuccess();
                    }
                });
            }
          });

          this.msalService.handleRedirectCallback((authError, response) => {
            if (authError) {
             this.logger.log('LoginComponent : Redirect Error: ' + authError.errorMessage);
              return;
            }

            this.logger.log('LoginComponent : Redirect success ');
          });

          this.msalService.setLogger(new Logger((logLevel, message, piiEnabled) => {
            this.logger.log('MSAL Logging: ' + message);
          }, {
            correlationId: CryptoUtils.createNewGuid(),
            piiLoggingEnabled: false
          }));
    }

    ngOnDestroy() {
        this.broadcastService.getMSALSubject().next(1);
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }

    checkoutAccount() {
        this.isUserLoggedIn = !!this.msalService.getAccount();
    }

    login() {
        this.logger.info('LoginComponent : login ');
        const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;

        if (isIE) {
          this.msalService.loginRedirect({ scopes: config.scopes.loginRequest});
        } else {
          this.msalService.loginPopup({ scopes: config.scopes.loginRequest});
        }
    }

    onLoginSuccess() {
        localStorage.setItem(Constants.localStorageKeys.accessToken, localStorage.getItem(Constants.localStorageKeys.msalToken));
        this.utilityService.redirectToURL(environment.appUrl + 'dashboard');
    }

    resetModel() {
        this.logger.info('LoginComponent : resetModel ');
        this.model.emailAddress = '';
        this.model.password = '';
    }

    showAccessDeniedDialog() {
        this.displayAccessDenied = true;
    }

    showIsActiveDialog() {
        this.displayIsActive = true;
    }

    closeAccessDeniedDialog() {
        this.displayAccessDenied = false;
        this.processLogout();
    }

    closeIsActiveDialog() {
        this.displayIsActive = false;
        this.processLogout();
    }

    processLogout() {
        this.authService.logOut();
    }

}
