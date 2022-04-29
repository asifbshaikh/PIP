
import { Injectable } from '@angular/core';

import { CookieService as Cookie } from 'ngx-cookie-service';

import { LoggerService } from './logger.service';

import { EnvironmentConfig } from '../infrastructure/utility.service';

 @Injectable()
export class CookieService {

    environmentName: string;
    domain: string;

    constructor(
        private logger: LoggerService,
        private config: EnvironmentConfig,
        private cookie: Cookie
    ) {
        this.logger.info('CookieService : constructor ');
        this.environmentName = config.environmentName;
        this.domain = config.domain;
    }

    public getCookie(cookieName: string): string {
        return this.cookie.get(this.environmentName + cookieName);
    }

    public setCookie(cookieName: string, value: string): void {
        document.cookie = this.environmentName + cookieName + '=' + value + ';domain=.' + this.domain + '; path = /';
    }

    public deleteCookie(cookieName: string): void {
        this.cookie.delete(this.environmentName + cookieName);
    }

    public doesCookieExists(cookieName: string): boolean {
        return this.cookie.check(this.environmentName + cookieName);
    }
}
