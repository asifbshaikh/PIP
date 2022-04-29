import {
    NgModule,
    ErrorHandler
} from '@angular/core';

import { Http } from '@angular/http';
import { HttpClient } from '@angular/common/http';

import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { PanelMenuModule } from 'primeng/panelmenu';
import { TooltipModule } from 'primeng/tooltip';
import { TextEllipsisComponent } from './text-ellipsis/text-ellipsis.component';

import {
    Routes,
    RouterModule
} from '@angular/router';

// plugins

import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';

import { NgbCarouselModule } from '@ng-bootstrap/ng-bootstrap';

import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import {
    RestrictInputDirective,
    EnableDisableControlsDirective
} from './directive/index';

import { ConfigurationSettings } from './infrastructure/index';

import {
    LogoutConfirmationComponent,
    HeaderComponent
} from './header/index';

import { FooterComponent } from './footer/footer.component';
import { SpinnerComponent } from './spinner/spinner.component';

import {
    DatexPipe,
    EllipsisPipe,
    SafeHtmlPipe,
    SplitPipe,
    NegativeValuePipe,
    CurrencyConversionPipe
} from './pipes/index';
import { NavmenuComponent } from './navmenu/navmenu.component';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { BreadcrumbService } from './breadcrumb/breadcrumb.service';
import { DropdownModule } from 'primeng/dropdown';
import { StepWizardModule } from './stepWizard/stepWizard.module';
import { Header1Component } from './header1/header1.component';
import { GlobalModule } from '@global/global.module';
import * as $ from 'jquery';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReactiveFormsModule } from '@angular/forms';
declare var resourcesVersion: any;
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { InputSwitchModule } from 'primeng/inputswitch';

import { CarouselModule } from 'primeng/carousel';
import { TabMenuModule } from 'primeng/tabmenu';
import { ConfirmationService } from 'primeng/api';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { DialogComponent } from './dialog/dialog.component';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { TableModule } from 'primeng/table';
import { FileUploadModule } from 'primeng/fileupload';
import { UnauthorisedComponent } from './unauthorised/unauthorised.component';

export function createTranslateLoader(http: HttpClient) {
    return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        FormsModule,
        RouterModule,
        TieredMenuModule,
        PanelMenuModule,
        StepWizardModule,
        ButtonModule,
        DialogModule,
        ConfirmDialogModule,
        MenuModule,
        InputSwitchModule,
        ToastModule,
        NgbModule,
        NgbCarouselModule,
        CarouselModule,
        TabMenuModule,
        TooltipModule,
        OverlayPanelModule,
        TableModule,
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: (createTranslateLoader),
                deps: [HttpClient]
            }
        }),
        DropdownModule,
        ReactiveFormsModule,
        MessageModule,
        MessagesModule
    ],
    declarations: [
        // pipes
        DatexPipe,
        EllipsisPipe,
        SafeHtmlPipe,
        SplitPipe,
        NegativeValuePipe,
        CurrencyConversionPipe,

        // directives
        RestrictInputDirective,
        EnableDisableControlsDirective,

        // components
        SpinnerComponent,
        HeaderComponent,
        LogoutConfirmationComponent,
        FooterComponent,
        NavmenuComponent,
        BreadcrumbComponent,
        Header1Component,
        DialogComponent,
        TextEllipsisComponent,
        UnauthorisedComponent,
    ],
    providers: [
        BreadcrumbService,
        ConfirmationService
    ],
    exports: [
        // Angular modules
        BrowserModule,
        BrowserAnimationsModule,
        FormsModule,
        RouterModule,
        StepWizardModule,
        ButtonModule,
        GlobalModule,
        MenuModule,
        InputSwitchModule,

        // plugins
        DialogModule,
        ConfirmDialogModule,
        ToastModule,
        NgbCarouselModule,
        TranslateModule,

        // pipes
        DatexPipe,
        EllipsisPipe,
        SafeHtmlPipe,
        SplitPipe,
        CurrencyConversionPipe,
        NegativeValuePipe,

        // directives
        RestrictInputDirective,
        EnableDisableControlsDirective,

        // shared components
        SpinnerComponent,
        HeaderComponent,
        LogoutConfirmationComponent,
        FooterComponent,
        NavmenuComponent,
        BreadcrumbComponent,
        Header1Component,
        DialogComponent,
        TextEllipsisComponent
    ]
})

export class SharedModule { }
