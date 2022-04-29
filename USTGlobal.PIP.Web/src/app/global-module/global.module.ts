import {
    NgModule,
    APP_INITIALIZER,
    ErrorHandler,
    ModuleWithProviders,
    SkipSelf,
    Optional
} from '@angular/core';

import {
    AuthGuardService,
    NotificationService,
    SharedData,
    SharedDataService
} from '@global';

export function sharedDataServiceFactory(service: SharedDataService) {
    return () => service.populateCommonData(0);
}

@NgModule({
    imports: [

    ],
    declarations: [
    ],
    providers: [
        AuthGuardService,
        SharedDataService,
        NotificationService,
        {
            provide: APP_INITIALIZER,
            useFactory: sharedDataServiceFactory,
            deps: [SharedDataService],
            multi: true
        }
    ],
    exports: [
    ]
})

export class GlobalModule {

    // Prevent global module to be injected multiple times
    constructor( @Optional() @SkipSelf() parentModule: GlobalModule) {
        if (parentModule) {
            throw new Error(
                'GlobalModule is already loaded. Import it in the AppModule only');
        }
    }

    static forRoot(): ModuleWithProviders {
        return {
            ngModule: GlobalModule,
            providers: [
            ]
        };
    }
}
