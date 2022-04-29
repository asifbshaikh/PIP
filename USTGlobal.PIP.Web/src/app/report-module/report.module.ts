import { NgModule } from '@angular/core';

import { SharedModule } from '../shared-module/shared.module';
import { SummaryComponent } from './summary/summary.component';
import { ReportsComponent } from './reports/reports.component';
import { PanelModule } from 'primeng/panel';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { DropdownModule } from 'primeng/dropdown';
import { RadioButtonModule } from 'primeng/radiobutton';
import { CalendarModule } from 'primeng/calendar';
import { MultiSelectModule } from 'primeng/multiselect';
import { TooltipModule } from 'primeng/tooltip';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
    imports: [
        MultiSelectModule,
        SharedModule,
        PanelModule,
        ConfirmDialogModule,
        DropdownModule,
        RadioButtonModule,
        ReactiveFormsModule,
        CalendarModule,
        TooltipModule,
    ],
    exports: [
        ConfirmDialogModule
    ],
    declarations: [
        SummaryComponent,
        ReportsComponent
    ],
    providers: [
        ConfirmationService
    ]
})
export class ReportModule { }
