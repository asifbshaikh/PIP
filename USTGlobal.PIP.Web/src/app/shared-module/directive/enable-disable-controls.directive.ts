import {
    Directive,
    ElementRef,
    HostListener,
    Input,
    Renderer,
    AfterViewInit
} from '@angular/core';

import { LoggerService } from '@core';
import {
    NotificationService,
    SharedDataService,
    SharedData
} from '../../global-module/index';

declare var $: any;

@Directive({
    selector: '[pipEnableDisable]'
})
export class EnableDisableControlsDirective implements AfterViewInit {

    parentElement: ElementRef;
    modelHeader: string;
    reset: string;

    constructor(
        private el: ElementRef,
        private notificationService: NotificationService,
        private logger: LoggerService,
    ) {
        this.logger.info('EnableDisableControls : constructor ');
        this.parentElement = this.el;
    }

    ngAfterViewInit() {
        this.logger.info('EnableDisableControls : ngAfterViewInit ');
        this.notificationService.onProjectsListClickNotification.subscribe(() => {
            this.enableDisable(this.parentElement.nativeElement);
        });
    }

    private enableDisable(currentElement: any): void {
        this.logger.info('EnableDisableControls : enableDisable ');
    }
}
