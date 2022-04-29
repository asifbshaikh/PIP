import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApproverComponent } from './approver/approver.component';
import { SharedModule } from '@shared/shared.module';
import { CardModule } from 'primeng/card';
import { PanelModule } from 'primeng/panel';
import { RouterModule } from '@angular/router';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputSwitchModule } from 'primeng/inputswitch';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { TooltipModule } from 'primeng/tooltip';

@NgModule({
  declarations: [ApproverComponent],
  imports: [
    CommonModule,
    CardModule,
    TooltipModule,
    CommonModule,
    SharedModule,
    PanelModule,
    RouterModule,
    DropdownModule,
    FormsModule,
    TableModule,
    ButtonModule,
    ReactiveFormsModule,
    InputSwitchModule,
    InputTextModule,
    DialogModule,
    CalendarModule,
    CheckboxModule
  ]
})
export class ApproverModule { }
