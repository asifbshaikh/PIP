import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, AbstractControl, Validators } from '@angular/forms';
import { RiskManagementService } from '@shared/services/risk-management.service';
import { ActivatedRoute } from '@angular/router';
import { IRiskManagement } from '@shared/domain/IRiskManagement';
import { ICalulcatedValue } from '@shared/domain/ICalculatedValue';
import { IRisk } from '@shared/domain/IRisk';
import { ResourceMapper, ProjectPeriod, Constants } from '@shared';
import { DateService } from '@core/services/date.service';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, MenuItem } from 'primeng/api';
import { SharedDataService, NotificationService } from '@global';
import { isNullOrUndefined } from 'util';


@Component({
  selector: 'risk-management',
  templateUrl: './risk-management.component.html'
})
export class RiskManagementComponent implements OnInit {
  form: FormGroup;
  pipSheetId: number;
  projectId: number;
  dashboardId: number;
  periodCols: any[] = [];
  riskManagementCols: any[] = [];
  riskManagementArray: any[] = [];
  riskManagementDetails: IRiskManagement = {
    calculatedValue: null,
    projectPeriod: [],
    riskManagement: null
  };
  contextMenuItems: MenuItem[];
  currentCellIndex = -1;
  currentCellValue = -1;
  currentRowIndex = -1;
  currentRowData: any;
  feesAtRiski159 = 0;
  netEstimatedRevenue = 0;
  costContingencyRiskAmount = 0;
  overrideDifference = 0;
  message: string;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  isValid: Boolean = true;
  pShowToolTip: string;

  constructor(private riskManagementService: RiskManagementService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private sharedDataService: SharedDataService,
    private messageService: MessageService,
    private dateService: DateService,
    private notificationService: NotificationService,
    private userWorkflowService: UserWorkflowService
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
      this.projectId = parseInt(data.get(Constants.uiRoutes.routeParams.projectId), 10);
      this.dashboardId = parseInt(data.get(Constants.uiRoutes.routeParams.dashboardId), 10);
    });
  }

  ngOnInit() {
    this.contextMenuItems = [{
      label: 'REPEAT >>',
      command: (event: any) => {
        this.onCellClick(true);
      }
    }];
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.initializeForm();
    this.getRiskManagementData();
  }

  enableDisableForm() {
    let flag = false;
    if (this.dashboardId === 3) {     // To be opened in readonly mode
      flag = true;
    }
    else {
      flag = this.userWorkflowService.isFormDisabled(this.checkRole, this.workflowFlag, this.loggedInUserId, this.dashboardId);
    }
    if (flag) {
      setTimeout(() => {
        this.form.disable();
      }, 200);
    }
  }

  configureDirtyCheck() {
    this.form.valueChanges.subscribe(() => {
      if (this.form.dirty) {
        this.notificationService.notifyFormDirty(true);
      }
    });
  }

  private getRiskManagementData() {
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.riskManagementService.getRiskmanagementDetails(this.pipSheetId).subscribe(data => {
        this.riskManagementDetails = data;
        this.getPeriods(data.projectPeriod);
        this.buildForm(this.riskManagementDetails.calculatedValue, this.riskManagementDetails.riskManagement);

        if (this.riskManagementDetails.riskManagement.riskManagementId > 0) {
          if (this.riskManagementDetails.riskManagement.costContingencyRisk === 0) {
            this.calculateCostContingencyWhenAmount(undefined);
            this.calculateCostContingencyWhenPercent(undefined);
          } else {
            if (this.riskManagementDetails.riskManagement.isContingencyPercent) {
              this.calculateCostContingencyWhenPercent(this.riskManagementDetails.riskManagement.costContingencyPercent);
            } else {
              this.calculateCostContingencyWhenAmount(this.riskManagementDetails.riskManagement.costContingencyRisk);
            }
          }
        } else {
          if (this.deliveryTypeId !== 5) {
            this.calculateCostContingencyWhenAmount(undefined);
          } else {
            this.calculateCostContingencyWhenPercent(undefined);
          }
        }
        this.pipSheetWorkflowStatus = this.sharedDataService.sharedData.pipSheetWorkflowStatus;
        this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
        this.isDataAvailable = true;
        this.enableDisableForm();
        this.configureDirtyCheck();
      });
    }
    else {
      this.isDataAvailable = true;
    }
  }
  buildForm(calculatedValue: ICalulcatedValue, riskManagement: IRisk) {
    this.form = this.fb.group({
      calculatedValue: this.fb.group({
        pipSheetId: this.pipSheetId,
        stdOverheadAmount: calculatedValue.stdOverheadAmount,
        totalDirectExpense: calculatedValue.totalDirectExpense
      }),
      riskManagement: this.fb.group({
        riskManagementId: riskManagement.riskManagementId,
        pipSheetId: this.pipSheetId,
        isContingencyPercent: riskManagement.isContingencyPercent,
        costContingencyRisk: riskManagement.costContingencyRisk,
        feesAtRisk: [riskManagement.feesAtRisk, [Validators.pattern(Constants.regExType.percentageWithDecimalPrecisionTwo)]],
        isFixedBid: riskManagement.isFixedBid,
        costContingencyPercent: riskManagement.costContingencyPercent,
        fixBidRiskAmount: riskManagement.fixBidRiskAmount,
        totalAssesedRiskOverrun: riskManagement.totalAssesedRiskOverrun,
        projectDeliveryTypeID: riskManagement.projectDeliveryTypeID,
        isOverride: riskManagement.isOverride,
        fixedBidPercent: 0,
        totalAssessedPercent: 0,
        revenue: 0,
        riskCostSubTotal: riskManagement.riskCostSubTotal,
        riskManagementPeriodDetail: this.computeRiskManagementPeriodDetails()
      }),
    });
    this.feesAtRiski159 = this.riskManagementService.calculateFeesAtRiski159(riskManagement.feesAtRisk);
  }

  initializeForm() {
    this.form = this.fb.group({
      calculatedValue: this.fb.group({
        pipSheetId: this.pipSheetId,
        stdOverheadAmount: 0,
        totalDirectExpense: 0
      }),
      riskManagement: this.fb.group({
        riskManagementId: 0,
        pipSheetId: this.pipSheetId,
        isContingencyPercent: false,
        costContingencyRisk: 0,
        costContingencyPercent: 0,
        feesAtRisk: 0,
        isFixedBid: 0,
        fixedBidPercent: 0,
        fixBidRiskAmount: 0,
        totalAssessedPercent: 0,
        projectDeliveryTypeID: 0,
        totalAssesedRiskOverrun: 0,
        isOverride: 0,
        revenue: 0,
        riskCostSubTotal: 0,
        riskManagementPeriodDetail: this.fb.array([])
      }),
    });
  }


  get calculatedValueForm() {
    return this.form.get('calculatedValue') as FormGroup;
  }

  get riskManagementForm() {
    return this.form.get('riskManagement') as FormGroup;
  }

  get deliveryTypeId() {
    if (!isNullOrUndefined(this.riskManagementDetails.riskManagement.projectDeliveryTypeID)) {
      return this.sharedDataService.sharedData.projectDeliveryTypeDTO
        .find(dt => dt.projectDeliveryTypeId === this.riskManagementDetails.riskManagement.projectDeliveryTypeID).projectDeliveryTypeId;
    }
    else {
      return 0;
    }
  }

  get riskManagementControls() {
    return this.riskManagementForm.controls;
  }

  get calculatedValueControls() {
    return this.calculatedValueForm.controls;
  }

  get FIXED_BID_PERCENT() {
    return 10;
  }

  get M155_CostSubTotal() {
    return +this.riskManagementControls.riskCostSubTotal.value;
  }

  computeRiskManagementPeriodDetails() {

    const riskPeriods = this.fb.array([]);

    // if data exists
    if (this.riskManagementDetails.riskManagement.riskManagementPeriodDetail.length > 0) {
      const riskperiod = this.riskManagementDetails.riskManagement.riskManagementPeriodDetail;
      riskperiod.forEach(period => {
        riskPeriods.push(this.fb.group({
          riskManagementId: period.riskManagementId,
          billingPeriodId: period.billingPeriodId,
          riskAmount: period.riskAmount
        }));
      });
    } else { //  if not exists
      this.riskManagementDetails.projectPeriod.forEach(period => {
        riskPeriods.push(this.fb.group({
          riskManagementId: [0],
          billingPeriodId: period.billingPeriodId,
          riskAmount: [0]
        }));
      });
    }

    return riskPeriods;
  }

  onSave(formData) {
    this.riskManagementService.saveRiskManagementData(formData).subscribe(success => {
      this.riskManagementService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
        this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
      });
      this.getRiskManagementData();
      this.getOverrideNotificationStatus();
      this.translateService.get('SuccessMessage.RiskManagement').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      this.form.markAsPristine();
      this.notificationService.notifyFormDirty(false);

    }, () => {
      this.translateService.get('ErrorMessage.RiskManagement').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
    });

  }

  private getPeriods(periods: ProjectPeriod[]) {
    this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
  }

  onAmountChange(amount: any) {
    amount = (<string>amount).trim() === '' ? undefined : +amount;
    this.calculateCostContingencyWhenAmount(amount);
  }

  onPercentChange(percent: any) {
    let isContingencyPercent = false;
    percent = (<string>percent).trim() === '' ? undefined : +percent;

    // previous
    if (!isNullOrUndefined(percent)) {
      isContingencyPercent = true;
    } else {
      isContingencyPercent = false;
    }
    this.riskManagementControls.isContingencyPercent.setValue(isContingencyPercent);
    this.calculateCostContingencyWhenPercent(percent);
  }

  onFeesAtRiskPercentChange(percent: any) {
    this.calculateFeesAtRisk(+percent);
  }

  calculateCostContingencyWhenAmount(amount: number) {
    const amountControl = this.riskManagementControls.costContingencyRisk;
    const percentControl = this.riskManagementControls.costContingencyPercent;
    this.riskManagementControls.isContingencyPercent.setValue(false);

    if (!isNullOrUndefined(amount)) {
      setTimeout(() => percentControl.disable());
    } else {
      amount = 0;
      setTimeout(() => percentControl.enable());
    }


    if (this.deliveryTypeId !== 5) {  // Fixed Bid (Milestone based)
      // amountControl.setValue(amount);
      this.costContingencyRiskAmount = amount;
      setTimeout(() => amountControl.enable());
      this.riskManagementControls.totalAssesedRiskOverrun.setValue(Math.abs(amount));
    } else {
      const h156 = this.riskManagementService.calculateH156(this.FIXED_BID_PERCENT, this.M155_CostSubTotal);
      this.riskManagementControls.fixedBidPercent.setValue(this.FIXED_BID_PERCENT);
      this.riskManagementControls.totalAssessedPercent.setValue(10);
      this.riskManagementControls.fixBidRiskAmount.setValue(h156);
      this.costContingencyRiskAmount = amount;
      this.riskManagementControls.totalAssesedRiskOverrun.setValue(Math.abs(h156 + amount));
    }
    this.calculateMonthlyRisk();
  }

  calculateCostContingencyWhenPercent(percent: number) {
    const amountControl = this.riskManagementControls.costContingencyRisk;
    if (!isNullOrUndefined(percent)) {
      setTimeout(() => amountControl.disable());
    } else {
      if (this.riskManagementControls.isContingencyPercent.value) {
        percent = this.derivePercentFromAmount(amountControl.value);
        setTimeout(() => amountControl.disable());
        this.riskManagementControls.costContingencyPercent.setValue(+percent.toFixed(2));
      } else {
        percent = 0;
        setTimeout(() => amountControl.enable());
      }
    }

    if (this.deliveryTypeId !== 5) {
      this.riskManagementControls.totalAssessedPercent.setValue(percent);
      const totalRiskOverrun = this.riskManagementService.calculateTotalRiskOverrun(percent, this.M155_CostSubTotal);
      this.riskManagementControls.totalAssesedRiskOverrun.setValue(totalRiskOverrun);
      this.riskManagementControls.costContingencyRisk.setValue(totalRiskOverrun === 0 ? null : totalRiskOverrun.toFixed(2));
      this.costContingencyRiskAmount = totalRiskOverrun;
    } else {
      this.riskManagementControls.fixedBidPercent.setValue(this.FIXED_BID_PERCENT); // default
      this.riskManagementControls.totalAssessedPercent.setValue(percent + 10);
      const h156 = this.riskManagementService.calculateH156(this.FIXED_BID_PERCENT, this.M155_CostSubTotal);
      const h157 = this.riskManagementService.calculateH157(percent, this.M155_CostSubTotal);
      this.riskManagementControls.fixBidRiskAmount.setValue(h156);
      this.riskManagementControls.costContingencyRisk.setValue(h157 === 0 ? null : h157);
      this.costContingencyRiskAmount = h157;
      this.riskManagementControls.totalAssesedRiskOverrun.setValue(Math.abs(h156 + h157));

    }
    this.calculateMonthlyRisk();
  }

  calculateFeesAtRisk(percent: number) {
    const feesAtRiskPercent = percent;
    if (this.form.controls.riskManagement.get('feesAtRisk').invalid) {
      this.translateService.get('MESSAGES.Tooltip.PercentError').subscribe((perMsg) => {
        this.pShowToolTip = perMsg['message'];
      });
    }
    else {
      this.pShowToolTip = '';
    }
    let revenue = 0;
    this.feesAtRiski159 = this.riskManagementService.calculateFeesAtRiski159(feesAtRiskPercent);
    revenue = this.riskManagementService.calculateRevenue(this.feesAtRiski159, 1); // 1 should be replaced totalClient price;
    this.riskManagementControls.revenue.setValue(revenue);
    this.calculateNetEstimatedRevenue();
  }

  calculateMonthlyRisk() {
    const totalAssesedRiskOverrun = +this.riskManagementControls.totalAssesedRiskOverrun.value;
    const totalCappedCost = this.riskManagementDetails.calculatedValue.totalCappedCost;
    const isOverride = this.riskManagementControls.isOverride.value;
    const periodRiskData = this.riskManagementService
      .calculatePeriodWiseRisk(this.riskManagementDetails.projectPeriod, totalAssesedRiskOverrun, totalCappedCost);
    this.riskManagementDetails.projectPeriod.forEach((period, index) => {
      const periodControl = (<FormArray>this.riskManagementControls.riskManagementPeriodDetail).controls[index] as FormGroup;
      if (!isOverride) {
        setTimeout(() => periodControl.disable());
        periodControl.controls['riskAmount'].setValue(isNaN(periodRiskData[index]) ? 0 : +periodRiskData[index].toFixed(2));
      } else {
        setTimeout(() => periodControl.enable());
      }
    });

    this.onOverrideValidate();
  }

  calculateNetEstimatedRevenue() {
    const totalClientPrice = 0; //  should come from calculated form
    const revenue = this.riskManagementControls.revenue.value;
    this.netEstimatedRevenue = this.riskManagementService.calculateNetEstimatedRevenue(totalClientPrice, revenue);
  }

  isOverrideChanged() {
    if (this.riskManagementControls.isOverride.value) {
      this.riskManagementDetails.projectPeriod.forEach((period, index) => {
        const periodControl = (<FormArray>this.riskManagementControls.riskManagementPeriodDetail).controls[index] as FormGroup;
        periodControl.controls['riskAmount'].setValue(0);
      });
    }
    this.calculateMonthlyRisk();
  }

  private derivePercentFromAmount(amount: number) {
    return Math.abs((amount * 100) / this.M155_CostSubTotal);
  }

  onOverrideValidate() {
    const override = this.riskManagementControls.isOverride.value;
    if (!isNullOrUndefined(override) && override) {
      const riskOverrun = this.riskManagementControls.totalAssesedRiskOverrun.value;
      this.overrideDifference = 0;
      this.overrideDifference =
        this.riskManagementService.calculateOverrideDifference(this.riskManagementControls.riskManagementPeriodDetail.value, riskOverrun);
      if (this.overrideDifference === 0) {
        this.riskManagementControls.totalAssesedRiskOverrun.setErrors(null);
        this.message = '';
        this.isValid = true;
      } else {
        this.riskManagementControls.totalAssesedRiskOverrun.setErrors({ riskOverrun: true });
        if (this.overrideDifference < 0) {
          this.message = `Override ignored as sum is ${Math.abs(this.overrideDifference).toFixed(2)} too low.`;
          this.isValid = false;
        } else if (this.overrideDifference > 0) {
          this.message = `Override ignored as sum is ${Math.abs(this.overrideDifference).toFixed(2)} too high.`;
          this.isValid = false;
        } else {
          this.message = '';
          this.isValid = true;
        }
      }
    } else {
      this.overrideDifference = 0;
      this.riskManagementControls.totalAssesedRiskOverrun.setErrors(null);
      this.message = '';
      this.isValid = true;
    }
  }

  roundOff(value, target) {
    if (value) {
      this.riskManagementControls[target].setValue(Math.round(value));
    }
  }

  getRiskManagementControls(): AbstractControl[] {
    const riskAmounts = this.form.controls.riskManagement as FormArray;
    const riskAmountGroup = riskAmounts.controls;
    return riskAmountGroup;
  }

  onCellClick(isContextEvent: boolean, event?, rowData?, rowIndex?: number) {
    if (!isContextEvent) {
      this.currentCellIndex = +event.target.id;
      this.currentRowIndex = rowIndex;
      this.currentRowData = rowData;
    } else {
      const periods: FormArray = (<FormArray>this.getRiskManagementControls()['riskManagementPeriodDetail']);
      this.currentCellValue = +<FormGroup>periods.controls[this.currentCellIndex].value.riskAmount;
      periods.controls.forEach((lostRevenueControl, index) => {
        if (index > this.currentCellIndex) {
          (<FormGroup>lostRevenueControl).controls['riskAmount'].setValue(this.currentCellValue);
        }
      });
      this.onOverrideValidate();
      this.scrollToEnd((periods.length - 1).toString());
    }
  }
  scrollToEnd(length: string) {
    document.getElementById(length).focus();
  }

  getOverrideNotificationStatus() {
    this.userWorkflowService.getOverrideNotificationStatus(+this.pipSheetId).subscribe(item => {
      const overrideNotification = item;
      if (overrideNotification.clientPrice || overrideNotification.riskManagement
        || overrideNotification.vacationAbsence || overrideNotification.ebitdaStdOverhead) {
        this.notificationService.showNotificationDialog(this.pipSheetId);
      }
    });
  }
}
