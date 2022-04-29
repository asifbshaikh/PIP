import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdministrationComponent } from './administration/administration.component';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { RouterModule } from '@angular/router';
import { MastersComponent } from './masters/masters.component';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import {TabViewModule} from 'primeng/tabview';
import { ButtonModule } from 'primeng/button';
import { LocationMasterComponent } from './masters/admin-masters/location-master/location-master.component';
import { InputSwitchModule } from 'primeng/inputswitch';
import { InputTextModule } from 'primeng/inputtext';
import { ContractingEntityComponent } from './masters/admin-masters/contracting-entity/contracting-entity.component';
import { ContractorMarkupComponent } from './masters/admin-masters/contractor-markup/contractor-markup.component';
import { HolidaysComponent } from './masters/admin-masters/holidays/holidays.component';
import { ServicePortfolioGroupComponent } from './masters/admin-masters/service-portfolio-group/service-portfolio-group.component';
import { ProjectDeliveryTypeComponent } from './masters/admin-masters/project-delivery-type/project-delivery-type.component';
import { SharedModule } from '@shared/shared.module';
import { EbitdaSeatCostComponent } from './masters/admin-masters/ebitda-seat-cost/ebitda-seat-cost.component';
import { MilestoneComponent } from './masters/admin-masters/milestone/milestone.component';
import { DirectExpensesComponent } from './masters/admin-masters/direct-expenses/direct-expenses.component';
import { BasicAssetsComponent } from './masters/admin-masters/basic-assets/basic-assets.component';
import { DialogModule } from 'primeng/dialog';
import { AddNewLocationComponent } from './masters/admin-masters/location-master/add-new-location/add-new-location.component';
import { CalendarModule } from 'primeng/calendar';
import { DefineFinancePocComponent } from './administration/define-finance-poc/define-finance-poc.component';
import { DefineAdminComponent } from './administration/define-admin/define-admin.component';
import { UserRolesComponent } from './userroles/userroles.component';
import { CheckboxModule } from 'primeng/checkbox';
import { AddNewUserComponent } from './administration/add-new-user/add-new-user.component';
import { TooltipModule } from 'primeng/tooltip';
import { DefineReadOnlyComponent } from './administration/define-read-only/define-read-only.component';
import { AccountRolesComponent } from './account-roles/account-roles.component';
import { TabMenuModule } from 'primeng/tabmenu';
import { PipCheckInComponent } from './administration/pip-check-in/pip-check-in.component';
import {FileUploadModule} from 'primeng/fileupload';

@NgModule({
  declarations: [
    AdministrationComponent,
    MastersComponent,
    LocationMasterComponent,
    ContractingEntityComponent,
    ContractorMarkupComponent,
    HolidaysComponent,
    ServicePortfolioGroupComponent,
    ProjectDeliveryTypeComponent,
    EbitdaSeatCostComponent,
    MilestoneComponent,
    DirectExpensesComponent,
    BasicAssetsComponent,
    AddNewLocationComponent,
    DefineFinancePocComponent,
    DefineAdminComponent,
    UserRolesComponent,
    AddNewUserComponent,
    DefineReadOnlyComponent,
    AccountRolesComponent,
    PipCheckInComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    SharedModule,
    PanelModule,
    CardModule,
    RouterModule,
    DropdownModule,
    FormsModule,
    TableModule,
    TabViewModule,
    ButtonModule,
    ReactiveFormsModule,
    InputSwitchModule,
    InputTextModule,
    DialogModule,
    CalendarModule,
    CheckboxModule,
    TooltipModule,
    TabMenuModule,
    FileUploadModule
  ]
})
export class AdministrationModule { }
