import { Constants } from './../../core-module/infrastructure/constants';
import {
    NgModule,
    Optional,
    SkipSelf,
    ModuleWithProviders
} from '@angular/core';

import { SharedModule } from '@shared/shared.module';
import { MasterComponent } from './master/master.component';
import { MultiSelectModule } from 'primeng/multiselect';
import { TableModule } from 'primeng/table';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { PanelModule } from 'primeng/panel';
import { StepsModule } from 'primeng/steps';
import { ProjectsComponent } from './projects/projects.component';
import { ProjectsStaffComponent } from './projects/projects-staff/projects-staff.component';
import { ProjectsMarginComponent } from './projects/projects-margin/projects-margin.component';
import { ProjectsSummaryComponent } from './projects/projects-summary/projects-summary.component';
import { InputTextModule } from 'primeng/inputtext';
import { ProjectsHeaderComponent } from './projects/projects-staff/projects-header/projects-header.component';
import { ProjectsControlComponent } from './projects/projects-staff/projects-control/projects-control.component';
import { ReactiveFormsModule } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { TooltipModule } from 'primeng/tooltip';
import { GlobalModule } from '@global/global.module';
import { ResourcePlanningComponent } from './projects/projects-staff/resource-planning/resource-planning.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ProjectsEstimateComponent } from './projects/projects-estimate/projects-estimate.component';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { CarouselModule } from 'primeng/carousel';
import { DateService } from '@core/services/date.service';
import { LaborPricingComponent } from './projects/projects-margin/labor-pricing/labor-pricing.component';
import { PipVersionComponent } from './projects/pip-versions/pip-version.component';
import { EbitdaComponent } from './projects/projects-margin/ebitda/ebitda.component';
import { VacationAbsensesComponent } from './projects/projects-margin/vacation-absenses/vacation-absenses.component';
import { PriceAdjustmentComponent } from './projects/projects-margin/price-adjustment/price-adjustment.component';
import { ExpensesAndAssetsComponent } from './projects/projects-margin/expenses-and-assets/expenses-and-assets.component';
import { PartnerCostRevenueComponent } from './projects/projects-margin/partner-cost-revenue/partner-cost-revenue.component';
import { FixedBidAndMarginCalcComponent } from './projects/projects-margin/fixed-bid-and-margin-calc/fixed-bid-and-margin-calc.component';
import { OtherPriceAdjustmentComponent } from './projects/projects-margin/other-price-adjustment/other-price-adjustment.component';
import { ReimbursementAndSalesComponent } from './projects/projects-margin/reimbursement-and-sales/reimbursement-and-sales.component';
import { RiskManagementComponent } from './projects/projects-margin/risk-management/risk-management.component';
import { SummaryComponent } from './projects/projects-summary/summary/summary.component';
import { GpmOmittingComponent } from './projects/projects-summary/gpm-omitting/gpm-omitting.component';
import { InvestmentViewComponent } from './projects/projects-summary/investment-view/investment-view.component';
import { EffortSummaryComponent } from './projects/projects-summary/effort-summary/effort-summary.component';
import { BillingComponent } from './projects/projects-summary/billing/billing.component';
import { PlForecastComponent } from './projects/projects-summary/pl-forecast/pl-forecast.component';
import { YOYComparisonComponent } from './projects/projects-summary/yoy-comparison/yoy-comparison.component';
import { CapitalChargesComponent } from './projects/projects-margin/capital-charges/capital-charges.component';
import { TotalClientPriceComponent } from './projects/projects-margin/total-client-price/total-client-price.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CheckboxModule } from 'primeng/checkbox';
import { ClipboardModule } from 'ngx-clipboard';
import { DatePipe } from '@angular/common';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ContextMenuModule } from 'primeng/contextmenu';
import { CardModule } from 'primeng/card';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { DialogService } from 'primeng/api';
import { CheckOutDialogComponent } from './projects/CheckOutDialog/CheckOutDialog.component';
import { OverrideNotificationDialogComponent } from '@shared/override-notification-dialog/override-notification-dialog.component';
import { SharePipComponent } from './projects/share-pip/share-pip.component';
import { SharedPipListComponent } from './projects/shared-pip-list/shared-pip-list.component';
import {TabViewModule} from 'primeng/tabview';
import { ReplicateComponent } from './projects/replicate/replicate.component';
import { DialogModule } from 'primeng/dialog';
import {MessagesModule} from 'primeng/messages';
import {MessageModule} from 'primeng/message';
import { InfographicsComponent } from './projects/projects-summary/infographics/infographics.component';

 import { RevisedSummaryComponent } from './projects/projects-summary/revised-summary/revised-summary.component';
import { ResourceAnalysisComponent } from './projects/projects-summary/resource-analysis/resource-analysis.component';
import { ChartsModule } from 'ng2-charts';
import { LocationWiseDetailsComponent } from './projects/projects-summary/location-wise-details/location-wise-details.component';
 import { PerformanceIndicatorsComponent } from './projects/projects-summary/performance-indicators/performance-indicators.component';
import { TotalDealFinancialsComponent } from './projects/projects-summary/total-deal-financials/total-deal-financials.component';
import {RadioButtonModule} from 'primeng/radiobutton';

@NgModule({
    imports: [
        SharedModule,
        MultiSelectModule,
        TableModule,
        DropdownModule,
        ButtonModule,
        PanelModule,
        StepsModule,
        InputTextModule,
        ReactiveFormsModule,
        CalendarModule,
        TooltipModule,
        GlobalModule,
        NgbModule,
        ScrollPanelModule,
        CarouselModule,
        ConfirmDialogModule,
        InputTextareaModule,
        CheckboxModule,
        ClipboardModule,
        OverlayPanelModule,
        ContextMenuModule,
        CardModule,
        DynamicDialogModule,
        TabViewModule,
        DialogModule,
        MessagesModule,
        MessageModule,
        ChartsModule,
        RadioButtonModule
    ],
    exports: [
        ConfirmDialogModule,
        DynamicDialogModule
    ],
    declarations: [
        MasterComponent,
        ProjectsComponent,
        ProjectsStaffComponent,
        ProjectsMarginComponent,
        ProjectsSummaryComponent,
        ProjectsHeaderComponent,
        ProjectsControlComponent,
        ResourcePlanningComponent,
        ProjectsEstimateComponent,
        LaborPricingComponent,
        PipVersionComponent,
        EbitdaComponent,
        VacationAbsensesComponent,
        PriceAdjustmentComponent,
        ExpensesAndAssetsComponent,
        PartnerCostRevenueComponent,
        FixedBidAndMarginCalcComponent,
        OtherPriceAdjustmentComponent,
        ReimbursementAndSalesComponent,
        RiskManagementComponent,
        SummaryComponent,
        GpmOmittingComponent,
        InvestmentViewComponent,
        EffortSummaryComponent,
        BillingComponent,
        PlForecastComponent,
        YOYComparisonComponent,
        CapitalChargesComponent,
        TotalClientPriceComponent,
        CheckOutDialogComponent,
        OverrideNotificationDialogComponent,
        SharePipComponent,
        SharedPipListComponent,
        ReplicateComponent,
        InfographicsComponent,
        RevisedSummaryComponent,
        ResourceAnalysisComponent,
        LocationWiseDetailsComponent,
        PerformanceIndicatorsComponent,
        TotalDealFinancialsComponent

    ],
    entryComponents: [
        CheckOutDialogComponent,
        OverrideNotificationDialogComponent
    ],
    providers: [
        DateService,
        ConfirmationService,
        DatePipe,
        DialogService

    ]
})

export class PlanningModule {

}
