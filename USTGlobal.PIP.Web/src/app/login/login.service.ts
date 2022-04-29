import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import {
    LoggerService
} from '@core';

import { Constants } from '@shared';
import { IUser } from '@global';

@Injectable()
export class LoginService {

    constructor(
        private httpClient: HttpClient,
        private logger: LoggerService
    ) {
        this.logger.info('LoginService : constructor ');
    }

    logOn(request: any): Observable<any> {
        this.logger.info('LoginService : logOn ');
        return this.httpClient.post(`${Constants.webApis.login}`, request);
    }

    getUserData(): Observable<any> {
        this.logger.info('LoginService : getUserData ');
        return this.httpClient.get<IUser>(`${Constants.webApis.getUserData}`);
    }

}
