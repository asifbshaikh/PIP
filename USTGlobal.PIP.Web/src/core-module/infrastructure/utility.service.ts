import { Injectable } from '@angular/core';
import { LoggerService } from '../services/logger.service';


export class EnvironmentConfig {
    environmentName: string;
    apiTokenUrl: string;
    appUrl: string;
    domain: string;
    scopes: string[];
}

@Injectable()
export class UtilityService {

    features = `width = 800, height = 580, top = 15, left == 15, location=no,directories=no,titlebar=no,status=no,
     toolbar = no, menubar = no, scrollbars = 1, resizable = 1, location = 0`;

    environmentName: string;
    calendarMinDate: Date;
    calendarMaxDate: Date;

    constructor(
        private logger: LoggerService,
        private config: EnvironmentConfig
    ) {
        this.logger.info('UtilityService : constructor ');
        this.environmentName = config.environmentName;
    }

    // First parameter URL is mandatory, other parameters are optional.
    public openInNewWindow = (url: string, target?: string, features?: string, replace?: boolean): Window => {
        this.logger.info('UtilityService : openInNewWindow');

        if (url !== undefined && url !== '') {
            features = (features !== undefined && features !== '') ? features : this.features;
            return window.open(url, target, features, replace);
        }
        else {
            return null;
        }
    }

    public openInNewTab = (url?: string, target?: string): void => {
        this.logger.info('UtilityService : openInNewTab');
        if (url !== undefined && url !== '') {
            window.open(url, target);
        }
    }

    public redirectToURL(href: string) {
        window.location.href = href;
    }

    public hideAppLoadingWidget(): void {
        const appLazyLoadingElement = document.getElementById('appInitloadingWidget');
        if (appLazyLoadingElement) {
            appLazyLoadingElement.style.visibility = 'hidden';
        }
    }

    public showAppLoadingWidget(): void {
        const appLazyLoadingElement = document.getElementById('appInitloadingWidget');
        if (appLazyLoadingElement) {
            appLazyLoadingElement.style.visibility = 'visible';
        }
    }

    public roundToNearestTenth(input: number ): number {
        return (input % 10 <= 5) ? this.roundToLowerTenth(input) : this.roundToUpperTenth(input);
    }

    public roundToLowerTenth(input: number ): number {
        return parseInt((input / 10).toString(), 10) * 10;
    }

    public roundToUpperTenth(input: number ): number {
        return parseInt((input / 10).toString(), 10) * 10 + 10;
    }

    public floor(input: number, decimalPlaces: number ): number {
        return Math.floor(input * parseInt(Math.pow(10, decimalPlaces).toString(), 10)
            / parseInt(Math.pow(10, decimalPlaces).toString(), 10));
    }

    public ceiling(input: number, decimalPlaces: number ): number {
        return Math.ceil(input * parseInt(Math.pow(10, decimalPlaces).toString(), 10))
            / parseInt(Math.pow(10, decimalPlaces).toString(), 10);
    }

    public round(input: number, decimalPlaces: number): number {
        return Math.round(input * parseInt(Math.pow(10, decimalPlaces).toString(), 10))
            / parseInt(Math.pow(10, decimalPlaces).toString(), 10);
    }

    public contains(array: string[], searchTerm: string): boolean {
        for (let i = 0; i < array.length; i++) {
            if (array[i].trim() === searchTerm) {
                return true;
            }
        }
        return false;
    }

    public compareValues(key: string, order: string = 'asc') {
        return function innerSort(a, b) {
          if (!a.hasOwnProperty(key) || !b.hasOwnProperty(key)) {
            return 0;
          }

          const varA = (typeof a[key] === 'string')
            ? a[key].toUpperCase() : a[key];
          const varB = (typeof b[key] === 'string')
            ? b[key].toUpperCase() : b[key];

          let comparison = 0;
          if (varA > varB) {
            comparison = 1;
          } else if (varA < varB) {
            comparison = -1;
          }
          return (
            (order === 'desc') ? (comparison * -1) : comparison
          );
        };
    }

    public setCalendarMinDate() {
        this.calendarMinDate = new Date();
        this.calendarMinDate.setMonth(0, 1);
        this.calendarMinDate.setFullYear(2001);
        return this.calendarMinDate;
    }

    public setCalendarMaxDate() {
        this.calendarMaxDate = new Date();
        this.calendarMaxDate.setMonth(11, 31);
        this.calendarMaxDate.setFullYear(2050);
        return this.calendarMaxDate;
    }

}
