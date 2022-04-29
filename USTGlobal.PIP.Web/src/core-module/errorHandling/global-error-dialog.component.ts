import {
    Component,
    ViewChild,
    ViewEncapsulation,
    Output,
    EventEmitter
} from '@angular/core';

import { Router } from '@angular/router';
import { LoggerService } from '../services/logger.service';
import { GlobalErrorLoggingService } from './global-error-logging.service';

@Component({
    moduleId: module.id,
    selector: 'app-error-dialog',
    templateUrl: 'global-error-dialog.component.html',
    encapsulation: ViewEncapsulation.None
})
export class GlobalErrorDialogComponent {

    displayModal = false;
    dialogTitle: string;
    message: string;
    isShowSecondaryButton: boolean;
    primaryButtonText: string;
    secondaryButtonText: string;
    isLogoutOnPrimaryButtonEvent: boolean;

    constructor(
        private router: Router,
        private logger: LoggerService,
        private globalErrorLoggingService: GlobalErrorLoggingService
    ) {
        this.logger.info('GlobalErrorDialogComponent : ngOnInit ');
        globalErrorLoggingService.showErrorDialog = this.showErrorDialog.bind(this);
    }


    showErrorDialog(errorDialogTitle: string, customErrorMessage: string, primaryButtonText: string
        , isLogoutOnPrimaryButtonEvent: boolean, isShowSecondaryButton: boolean, secondaryButtonText: string) {
        this.logger.info('GlobalErrorDialogComponent : showErrorDialog ');
        this.dialogTitle = errorDialogTitle;
        this.message = customErrorMessage;
        this.isShowSecondaryButton = isShowSecondaryButton;
        this.primaryButtonText = primaryButtonText;
        this.secondaryButtonText = secondaryButtonText;
        this.isLogoutOnPrimaryButtonEvent = isLogoutOnPrimaryButtonEvent;
        this.displayModal = true;
    }

    logout() {
        this.logger.info('GlobalErrorDialogComponent : logout ');
    }

    initDialogData() {
        this.logger.info('GlobalErrorDialogComponent : initDialogData ');
    }
}
