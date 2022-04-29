import {
    Injectable,
    EventEmitter
} from '@angular/core';

import {TranslateService} from '@ngx-translate/core';
import { LoggerService } from '../services/logger.service';

import {
    ErroNotificationType,
    HttpError,
    ErrorCode,
    ToastrMessageType
} from '../extensions/http-error.model';

import { ToastrService } from '../services/index';

@Injectable()
export class GlobalErrorLoggingService {

    errorDialogTitle: string;
    errorDialogMessage: string;
    primaryButtton: string;
    secondaryButton: string;
    isLogoutonPrimaryButton: boolean;
    isShowSecondaryButton: boolean;
    notificationType: ErroNotificationType;
    isHandledError = true;

    showErrorDialog: (errorDialogTitle: string, customErrorMessage: string, primaryButtonText: string
        , isLogoutOnPrimaryButtonEvent: boolean, isShowSecondaryButton: boolean, secondaryButtonText: string) => void;

    constructor(private logger: LoggerService,
        private translate: TranslateService,
        private toastrService: ToastrService
    ) {
        this.logger.info('GlobalErrorLoggingService : constructor ');
    }

    public logError(error: any, isLogToConsole: boolean, isSendToServer: boolean): void {
        this.logger.info('GlobalErrorLoggingService : logError ');

        this.notificationType = ErroNotificationType.Dialog;

        if (error instanceof HttpError) { /// This is handled Exception

            this.notificationType = (<HttpError>error).erroNotificationType;

            if (this.notificationType === ErroNotificationType.Dialog) {
                this.translate.get('MESSAGES.Dialog.' + error.code)
                    .subscribe((successResponse) => {
                        this.errorDialogTitle = successResponse.title;
                        this.errorDialogMessage = successResponse.message;
                        this.primaryButtton = successResponse.primaryButton;
                        this.secondaryButton = successResponse.secondaryButton;
                        this.isShowSecondaryButton = JSON.parse(successResponse.isShowSecondaryButton);
                        this.isLogoutonPrimaryButton = JSON.parse(successResponse.isLogoutOnPrimaryButton);
                    }, (errorResponse) => {

                    });
            } else {
                this.notificationType = ErroNotificationType.Toaster;
            }
        } else { /// This is Un-Handled Exception

            this.notificationType = ErroNotificationType.Toaster;
            this.isHandledError = false;
        }

        if (isLogToConsole) {
            if (this.notificationType === ErroNotificationType.Dialog) {
                this.logger.error(this.errorDialogMessage);
            }

            this.logger.error(error);

            if (error.stack !== undefined) {
                this.logger.error(error.stack);
            }
        }

        if (this.notificationType === ErroNotificationType.Dialog) {
            this.showErrorDialog(this.errorDialogTitle, this.errorDialogMessage, this.primaryButtton
                , this.isLogoutonPrimaryButton, this.isShowSecondaryButton, this.secondaryButton);
        } else if (this.notificationType === ErroNotificationType.Toaster) {
            if (this.isHandledError) {
                this.toastrService.showError(error.code);
            } else {
                this.toastrService.showError(ErrorCode.Fatal);
            }
        }
    }

}
