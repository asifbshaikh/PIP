import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';

import { LoggerService } from './logger.service';

@Injectable()
export class ToastrService {

    constructor(
        private logger: LoggerService,
        private translate: TranslateService,
        private messageService: MessageService
        ) {}

    showSuccess(toastrCode) {
        this.messageService.add({severity: 'success', summary: 'Success Message', detail: this.getMessage(toastrCode) });
    }

    showInfo(toastrCode) {
        this.messageService.add({severity: 'info', summary: 'Info Message', detail: this.getMessage(toastrCode)});
    }

    showWarn(toastrCode) {
        this.messageService.add({severity: 'warn', summary: 'Warn Message', detail: this.getMessage(toastrCode)});
    }

    showError(toastrCode) {
        this.messageService.add({severity: 'error', summary: 'Error Message', detail: this.getMessage(toastrCode)});
    }

    getMessage(toastrCode) {
        let message = '';

        this.translate.get('MESSAGES.Toastr.' + toastrCode)
                .subscribe((successResponse) => {
                    this.logger.info('ToastrMessageHelperService : getFormattedToast : Success');
                    message = successResponse;
                }, (errorResponse) => {
                    this.logger.info('ToastrMessageHelperService : getFormattedToastrMessage : Error');
                });

        return message;
    }
}
