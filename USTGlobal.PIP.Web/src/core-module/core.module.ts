import {
    NgModule,
    ErrorHandler,
    ModuleWithProviders,
    SkipSelf,
    Optional
} from '@angular/core';

import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {
    HttpModule,
    Http,
    BrowserXhr,
    RequestOptions,
    XHRBackend
} from '@angular/http';

import { LoggerModule, NgxLoggerLevel } from 'ngx-logger';
import { CookieService } from 'ngx-cookie-service';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

import {
    LoggingErrorHandlerOptions,
    GlobalErrorHandlerComponent,
    GlobalErrorLoggingService,
    GlobalErrorDialogComponent
} from './errorHandling/index';

import {
    CustomBrowserXhr,
    AuthService
} from './extensions/index';

import {
    ConfigurationSettings,
    EnvironmentConfig,
    UtilityService,
    ValidationService
} from './infrastructure/index';

import { HttpClientModule } from '@angular/common/http';

import { LoggerService } from './services/logger.service';

import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

import { SpinnerService } from './spinner/spinner.service';
import { ToastrService } from './services/toastr.service';
import { BrowserInfoService } from './services/browser-info.service';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        BrowserAnimationsModule,
        HttpModule,
        LoggerModule,
        DialogModule,
        ToastModule,
        HttpClientModule
    ],
    declarations: [
        GlobalErrorDialogComponent,
        PageNotFoundComponent
    ],
    exports: [
        GlobalErrorDialogComponent,
        PageNotFoundComponent
    ],
    providers: [
        LoggerService,
        MessageService,
        ToastrService,
        CookieService,
        UtilityService,
        ValidationService,
        AuthService,
        SpinnerService,
        BrowserInfoService,
        GlobalErrorLoggingService,
        {
            provide: LoggingErrorHandlerOptions,
            useValue: {
                isRethrowError: ConfigurationSettings.isRethrowErrorInsideGlobalErrorHandler,
                isUnwrapError: ConfigurationSettings.isUnwrapErrorInsideGlobalErrorHandler,
                isLogErrorToConsole: ConfigurationSettings.islogErrorToConsoleInsideGlobalErrorHandler,
                isSendErrorToServer: ConfigurationSettings.isSendErrorToServerInsideGlobalErrorHandler,
                isShowErrorDialog: ConfigurationSettings.isShowErrorDialogInsideGlobalErrorHandler
            }
        },
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandlerComponent
        },
        {
            provide: BrowserXhr,
            useClass: CustomBrowserXhr
        }
    ]
})

export class CoreModule {

    // Prevent core module to be injected multiple times
    constructor( @Optional() @SkipSelf() parentModule: CoreModule) {
        if (parentModule) {
            throw new Error(
                'CoreModule is already loaded. Import it in the AppModule only');
        }
    }

    static forRoot(config: EnvironmentConfig): ModuleWithProviders {
        return {
            ngModule: CoreModule,
            providers: [
                { provide: EnvironmentConfig, useValue: config }
            ]
        };
    }

}
