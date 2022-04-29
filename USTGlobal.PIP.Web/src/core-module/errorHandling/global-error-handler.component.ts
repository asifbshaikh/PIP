import {
    ErrorHandler,
    Injectable,
    Inject
} from '@angular/core';

import { LoggerService } from '../services/logger.service';

import {
    AuthService
} from '../extensions/index';

import {
    Constants,
    ConfigurationSettings,
    UtilityService,
    EnvironmentConfig
} from '../infrastructure/index';

import { SpinnerService } from '../spinner/spinner.service';

import { GlobalErrorLoggingService } from './global-error-logging.service';

export class LoggingErrorHandlerOptions {
    isRethrowError: boolean;
    isUnwrapError: boolean;
    isLogErrorToConsole: boolean;
    isSendErrorToServer: boolean;
    isShowErrorDialog: boolean;
}

@Injectable()
export class GlobalErrorHandlerComponent implements ErrorHandler {

    private options: LoggingErrorHandlerOptions;

    constructor(
        private globalErrorLoggingService: GlobalErrorLoggingService,
        options: LoggingErrorHandlerOptions,
        private logger: LoggerService,
        private spinner: SpinnerService,
        private utilityService: UtilityService,
        private authService: AuthService,
        private config: EnvironmentConfig
    ) {
        this.logger.info('ErrorHandler : constructor');
        this.options = options;
    }

    public handleError(error: any): void {

        const url: string = this.config.appUrl + '?' + Constants.queryString.SessionExpired;

        try {
            this.logger.info('ErrorHandler : handleError()');

            if (error && error.status === 0) {
                this.logger.error('!!!!!! API is down !!!!!!');
            }
            else if (error && error.status === 401) { // token expired/invalid
                localStorage.clear();
                this.logger.error(error);
                this.utilityService.redirectToURL(url);
                return;
            }

            this.spinner.stop();
            this.logger.error(error);

        } catch (loggingError) {
            this.logger.error('Error in global error handler service. Original error was - ');
            this.logger.error(error);
            this.utilityService.redirectToURL(url);
            return;
        }

        if (this.options.isRethrowError) {
            throw (error);
        }
    }

    private findOriginalError(error: any): any {
        this.logger.info('ErrorHandler : findOriginalError()');

        while (error && error.originalError) {
            error = error.originalError;
        }

        return (error);
    }
}
