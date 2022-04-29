import {
    Component,
    OnInit,
    ChangeDetectionStrategy,
    ChangeDetectorRef
} from '@angular/core';

import {
    UtilityService,
    LoggerService
} from '@core';

import { SharedDataService } from '@global';

import { FooterService } from './footer.service';

@Component({
    moduleId: module.id,
    changeDetection: ChangeDetectionStrategy.OnPush,
    selector: 'app-footer',
    templateUrl: 'footer.component.html',
    providers: [FooterService]
})
export class FooterComponent implements OnInit {
    model = '';

    constructor(
        private logger: LoggerService,
        private utilityService: UtilityService,
        private changeRef: ChangeDetectorRef,
        private footerService: FooterService,
        private sharedDataService: SharedDataService
    ) {
        this.logger.info('FooterComponent : constructor ');
    }

    ngOnInit() {
        this.logger.info('FooterComponent : ngOnInit');
    }

}
