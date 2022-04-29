import { ICheckRole } from './../../shared-module/domain/ICheckRole';
import { isNullOrUndefined } from 'util';
import { SharedDataService } from './shared-data.service';
import { Injectable } from '@angular/core';
import {
    CanActivate,
    Router,
    ActivatedRouteSnapshot,
    RouterStateSnapshot,
    ActivatedRoute
} from '@angular/router';

import { LoggerService, AuthService } from '@core';
import { IPipSheetWFStatusAndAccountSpecificRole } from '@shared/domain/IPipSheetWFStatusAndAccountSpecificRole';

@Injectable()
export class AuthGuardService implements CanActivate {
    private role: IPipSheetWFStatusAndAccountSpecificRole;

    constructor(
        private router: Router,
        private logger: LoggerService,
        private authService: AuthService,
        private sharedDataService: SharedDataService,
    ) {
        this.logger.info('AuthGuard : constructor ');
    }


    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        let isUserAuthorised = false;
        const userRoles = this.sharedDataService.sharedData ? this.sharedDataService.sharedData.userRoleAccountDTO.roleAndAccountDTO
            : undefined;

        if (this.authService.isUserLoggedIn() && !isNullOrUndefined(userRoles)) {
            const checkRole: ICheckRole = this.sharedDataService.getUserSpecificRoles(userRoles);
            const pageRequest = route.routeConfig.path.split('/');
            const urlRequest = pageRequest[0];
            const pipsheetId = route.params.pipSheetId;
            const accountId = route.params.accountId;
            const projectId = route.params.projectId;

            switch (urlRequest) {
                case 'administration':
                    if (pageRequest.length === 1) {
                        isUserAuthorised = checkRole.isAdmin || checkRole.isFinanaceApprover;
                    } else if ((pageRequest[1] === 'userroles' || pageRequest[1] === 'defineFinancePoc') &&
                        !checkRole.isAdmin && checkRole.isFinanaceApprover) {
                        isUserAuthorised = true;
                    } else {
                        isUserAuthorised = checkRole.isAdmin ? true : false;
                    }

                    break;

                case 'approver':
                    isUserAuthorised = checkRole.isFinanaceApprover || checkRole.isReviewer;
                    break;

                case 'reports':
                    isUserAuthorised = checkRole.isFinanaceApprover || this.sharedDataService.sharedData.hasAccountLevelEditorAccess;
                    break;

                case ((isNullOrUndefined(pipsheetId) && isNullOrUndefined(accountId)) ? 'projects' : '-'):
                case ((isNullOrUndefined(pipsheetId) && isNullOrUndefined(accountId)) ? 'samples' : '-'):
                    if (!isNullOrUndefined(projectId)) {
                        const data = await this.authService.isUserAuthorised(0, 0, +projectId);

                        if ((!data.isDummy && urlRequest === 'projects') || (data.isDummy && urlRequest === 'samples')) {
                            isUserAuthorised = data.canNavigate;
                        } else {
                            isUserAuthorised = false;
                        }


                    } else {
                        isUserAuthorised = this.sharedDataService.sharedData.hasAccountLevelAccess ||
                            this.sharedDataService.sharedData.hasSharePipAccess;
                    }
                    break;
                default:
                    if (!isNullOrUndefined(pipsheetId) && !isNullOrUndefined(accountId) && +pipsheetId > 0 && +accountId > 0) {
                        const data = await this.authService.isUserAuthorised(+pipsheetId, +accountId, 0);
                        if ((!data.isDummy && urlRequest === 'projects') || (data.isDummy && urlRequest === 'samples')) {
                            isUserAuthorised = data.canNavigate;
                        } else {
                            isUserAuthorised = false;
                        }

                    } else {
                        isUserAuthorised = true;
                    }
                    break;
            }
            if (!isUserAuthorised) {
                this.router.navigate(['/unauthorised']);
                return false;
            }
        } else {
            this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
            return false;
        }
        return true;
    }
}
