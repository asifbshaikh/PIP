import { Component } from '@angular/core';
import { LoggerService } from '../services/logger.service';

@Component({
    moduleId: module.id,
    selector: 'app-page-not-found',
    template: `<div class="empty-page">page not found</div>`
})
export class PageNotFoundComponent {

    constructor(
        private logger: LoggerService
    ) {
        this.logger.info('PageNotFoundComponent : constructor ');
    }

}
