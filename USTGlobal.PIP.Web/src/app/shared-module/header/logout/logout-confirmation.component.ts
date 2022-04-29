import {
    Component,
    ViewChild,
    ViewEncapsulation,
    Output,
    EventEmitter,
    ChangeDetectionStrategy,
    ElementRef,
    ChangeDetectorRef
} from '@angular/core';

import { Router } from '@angular/router';
import { LoggerService } from '@core';

@Component({
    moduleId: module.id,
    changeDetection: ChangeDetectionStrategy.OnPush,
    selector: 'app-logout-confirmation',
    templateUrl: 'logout-confirmation.component.html',
    encapsulation: ViewEncapsulation.None
})
export class LogoutConfirmationComponent {

    @Output() logoutConfirmation = new EventEmitter<boolean>();

    displayModal = false;

    constructor(
        private router: Router,
        private logger: LoggerService,
        private cdRef: ChangeDetectorRef
    ) {
        this.logger.info('LogoutConfirmationComponent : ngOnInit ');
    }

    showConfirmation() {
        this.logger.info('LogoutConfirmationComponent : showConfirmation ');
        this.displayModal = true;
        this.cdRef.detectChanges();
    }

    closeAndInstructLogoutToParent() {
        this.logger.info('LogoutConfirmationComponent : closeAndInstructLogoutToParent ');
        this.displayModal = false;
        this.logoutConfirmation.emit(true);
    }

    dismissModal() {
    }
}
