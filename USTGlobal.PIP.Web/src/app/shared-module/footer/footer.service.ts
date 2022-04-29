import {
    Injectable,
    Inject
} from '@angular/core';

import { Observable } from 'rxjs/Observable';

import {
    LoggerService
} from '@core';

@Injectable()
export class FooterService {

    constructor(
        private logger: LoggerService
    ) {
        this.logger.info('FooterService : constructor ');
    }

    getFooterSupportContactInfo(entityId: number): Observable<any> {

        this.logger.info('FooterService : getFooterSupportContactInfo ');

        return Observable.of('support@test.company');
    }
}
