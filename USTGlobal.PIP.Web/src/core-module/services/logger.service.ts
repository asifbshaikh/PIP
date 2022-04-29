import { Injectable } from '@angular/core';

import { NGXLogger, CustomNGXLoggerService, NgxLoggerLevel } from 'ngx-logger';
import { environment } from '@env';
import * as moment from 'moment';
import { BrowserInfoService } from './browser-info.service';
// import { Constants } from '@core/infrastructure';

@Injectable()
export class LoggerService {
    private logger: NGXLogger;
    private logQueue: string[] = [];
    private noOfLastLogsSendToServer = 100;

    constructor(customLogger: CustomNGXLoggerService, private browserInfo: BrowserInfoService) {
        if (environment.environmentName !== 'Dev') {
            this.logger = customLogger.create(
                {
                    serverLoggingUrl: environment.serverLoggingUrl,
                    level: environment.logLevel,
                    serverLogLevel: environment.serverLogLevel,
                });
        }
        else {
            this.logger = customLogger.create(
                {
                    level: environment.logLevel,
                    serverLogLevel: environment.serverLogLevel,
                });
        }
    }

    private enqueueLog(message: string, logLevel: string) {
        if (environment.serverLogLevel !== NgxLoggerLevel.OFF) {
            const logLenght = this.logQueue.push(`${moment().format()} ${logLevel} ${message}`);
            if (logLenght > this.noOfLastLogsSendToServer * 2) {
                this.logQueue = this.logQueue.slice(logLenght - this.noOfLastLogsSendToServer, logLenght);
            }
        }
    }

    private getLastNLogs(): string[] {
        const logLenght = this.logQueue.length;
        return this.logQueue.slice(
            logLenght > this.noOfLastLogsSendToServer ? (logLenght - this.noOfLastLogsSendToServer) : 0,
            logLenght);
    }

    private getLoggenInUser() {
        return JSON.parse(JSON.stringify(localStorage.getItem('userId')));
    }

    trace(message: string) {
        this.enqueueLog(message, 'TRACE');
        this.logger.trace(message);
    }

    debug(message: string) {
        this.enqueueLog(message, 'DEBUG');
        this.logger.debug(message);
    }

    info(message: string) {
        this.enqueueLog(message, 'INFO');
        this.logger.info(message);
    }

    log(message: string) {
        this.enqueueLog(message, 'LOG');
        this.logger.log(message);
    }

    warn(message: string) {
        this.enqueueLog(message, 'WARN');
        this.logger.warn(message);
    }

    // Note - logger.error/logger.fatal to be called only when we want log to post to server
    error(message: string) {
        this.enqueueLog(message, 'ERROR');
        // systemInfo, userIdentification
        this.logger.error(message, this.getLoggenInUser(), this.getLastNLogs(), this.browserInfo.getBrowserInfo());
    }


    fatal(message: string) {
        this.enqueueLog(message, 'FATAL');
        this.logger.fatal(message, this.getLoggenInUser(), this.getLastNLogs(), this.browserInfo.getBrowserInfo());
    }
}
