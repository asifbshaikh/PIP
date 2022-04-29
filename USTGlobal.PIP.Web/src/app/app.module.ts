import { BrowserModule } from '@angular/platform-browser';
import { NgModule, DoBootstrap, ApplicationRef } from '@angular/core';
import { ServiceWorkerModule } from '@angular/service-worker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from '@core/core.module';
import { GlobalModule } from '@global/global.module';
import { SharedModule } from '@shared/shared.module';
import { routing } from './app.routing';
import { PlanningModule } from './planning-module/planning.module';
import { AdministrationModule } from './administration-module/administration.module';
import { DashboardModule } from './dashboard-module/dashboard.module';
import { ReportModule } from './report-module/report.module';
import { LoginComponent } from './login/index';
import { AppComponent } from './app.component';
import { environment } from '../environments/environment';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import {CardModule} from 'primeng/card';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { NgIdleKeepaliveModule } from '@ng-idle/keepalive';
import { ApproverModule } from './approver-module/approver.module';

import { MsalModule, MsalService, MSAL_CONFIG, MSAL_CONFIG_ANGULAR, MsalInterceptor } from '@azure/msal-angular';
import { MSALAngularConfigFactory, MSALConfigFactory, config } from '../auth-settings';
import { MsalComponent } from './msal.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { HttpClientInterceptor } from '@core/extensions/http-interceptor.service';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    MsalComponent
  ],
  imports: [
    MsalModule,
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    PerfectScrollbarModule,
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production }),
    CoreModule.forRoot(
      {
        environmentName: environment.environmentName
        , apiTokenUrl: ''
        , appUrl: environment.appUrl
        , domain: environment.domain
        , scopes : config.scopes.loginRequest
      }),
    GlobalModule.forRoot(),
    SharedModule,
    routing,
    PlanningModule,
    AdministrationModule,
    DashboardModule,
    ReportModule,
    FormsModule,
    DropdownModule,
    CardModule,
    ApproverModule,
    NgIdleKeepaliveModule.forRoot()
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpClientInterceptor,
      multi: true
    },
    {
      provide: MSAL_CONFIG,
      useFactory: MSALConfigFactory
    },
    {
      provide: MSAL_CONFIG_ANGULAR,
      useFactory: MSALAngularConfigFactory
    },
    MsalService
  ],
  entryComponents: [AppComponent]
})

export class AppModule implements DoBootstrap {
  constructor() {
    console.log('APP Module Constructor!');
  }

  ngDoBootstrap(ref: ApplicationRef) {
    if (window !== window.parent && !window.opener) {
      console.log('Bootstrap: MSAL');
      ref.bootstrap(MsalComponent);
    }
    else {
      console.log('Bootstrap: App');
      ref.bootstrap(AppComponent);
    }
  }

}
