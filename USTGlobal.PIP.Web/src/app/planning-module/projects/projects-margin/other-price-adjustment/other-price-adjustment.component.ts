import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, FormControl, AbstractControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, SelectItem, MenuItem } from 'primeng/api';
import { DateService } from '@core/services/date.service';
import { OtherpriceadjustmentService } from '@shared/services/other-price-adjustment.service';
import { NotificationService, SharedDataService } from '@global';
import {
  IPeriodOtherPriceAdjustment, IOtherPriceAdjustment, OtherPriceAdjustment,
  ResourceMapper, ProjectPeriod, Mastermapper, Constants
} from '@shared';
import { UtilityService } from '@core';

@Component({
  selector: 'other-price-adjustment',
  templateUrl: './other-price-adjustment.component.html'
})
export class OtherPriceAdjustmentComponent implements OnInit {
  otherPriceAdjustmentForm: FormGroup;
  otherPriceAdjustmentData: OtherPriceAdjustment = {
    otherPriceAdjustmentParent: [], projectMilestone: [], projectPeriod: [], isMonthlyFeeAdjustment: false
  };
  selectedMilestones: SelectItem[];
  projectPeriod: ProjectPeriod[];
  translationData: any;
  pipSheetId: number;
  projectId: number;
  selectedOtherPriceAdjustment: any[] = [];
  periodCols: any[] = [];
  otherPriceAdjustmentCols: any[] = [];
  colSpanSize = 0;
  totalAdjustedRevenue = 0;
  totalFeeAfterAdjustment = 0;
  periodsTotal: number[] = [];
  adjustedRevenuePeriodsTotal: number[] = [];
  contextMenuItems: MenuItem[];
  currentCellIndex = -1;
  currentCellValue = -1;
  currentRowIndex = -1;
  currentRowData: any;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  disableOtherFormControls = false;
  dashboardId: number;
  isPeriods: boolean;
  isSaveClicked = false;
  adjustmentEntryPeriodTotal: number;
  setMonthlyFeePeriodTotal;
  feeBeforeAdjustment: string;
  selRow: any[] = [];
  constructor(
    private dateService: DateService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private otherPriceAdjustmentService: OtherpriceadjustmentService,
    private notificationService: NotificationService,
    private messageService: MessageService,
    private utilityService: UtilityService,
    private sharedDataService: SharedDataService,
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
    this.initializeForm();
    this.translationData = {};
    this.translateService.get('OtherPriceAdjustment').subscribe(data => {
      this.translationData = data;
    });

    // Other_Price_Adjustment columns
    this.translateService.get('OtherPriceAdjustment.OtherPriceAdjustmentColumns').subscribe(cols => {
      this.otherPriceAdjustmentCols = cols;
    });
    this.translateService.get('OtherPriceAdjustment').subscribe(otherPriceAjdustmentConstants => {
      this.feeBeforeAdjustment = otherPriceAjdustmentConstants.FeeBeforeAdjustment;
    });
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.getOtherPriceAdjustmentData();
  }


  initializeForm() {
    this.otherPriceAdjustmentForm = this.fb.group({
      isMonthlyFeeAdjustment: [''],
      otherPriceAdjustmentParent: this.fb.array([]),
    });
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
      this.disableOtherFormControls = true;
      setTimeout(() => {
        this.disableOtherFormControls = true;
        this.otherPriceAdjustmentForm.disable();
      }, 200);
    }
  }

  configureDirtyCheck() {
    this.otherPriceAdjustmentForm.valueChanges.subscribe(() => {
      if (this.otherPriceAdjustmentForm.dirty) {
        this.notificationService.notifyFormDirty(true);
      }
    });
  }

  getOtherPriceAdjustmentData() {
    // post other_Price_Adjustment
    this.otherPriceAdjustmentData = {
      otherPriceAdjustmentParent: [], projectMilestone: [], projectPeriod: [], isMonthlyFeeAdjustment: false
    };
    this.initializeForm();
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }

    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      // get Other_Price_Adjustment
      this.otherPriceAdjustmentService.getOtherPriceAdjustment(this.pipSheetId).subscribe(data => {
        this.getPeriods(data.projectPeriod);

        this.otherPriceAdjustmentData.isMonthlyFeeAdjustment = data.isMonthlyFeeAdjustment;
        // check other_Price_Adjustement data available or fetch default data
        if (data.otherPriceAdjustmentParent.length > 1) {
          this.otherPriceAdjustmentData.otherPriceAdjustmentParent = data.otherPriceAdjustmentParent;
        }
        else {
          this.otherPriceAdjustmentData.otherPriceAdjustmentParent = this.otherPriceAdjustmentService
            .getDefaultOtherPriceAdjustment(this.projectPeriod, this.pipSheetId, data.otherPriceAdjustmentParent[0]);
        }
        this.otherPriceAdjustmentData.projectPeriod = data.projectPeriod;
        this.selectedMilestones = new Mastermapper().getOptionalPhaseComboItems(data.projectMilestone, false);
        if (data.isMonthlyFeeAdjustment) {
          this.bindFormDataSetMonthlyOn();
        }
        else {
          this.bindFormDataSetMonthlyOff();
          this.onCostChange();
        }
        this.pipSheetWorkflowStatus = this.sharedDataService.sharedData.pipSheetWorkflowStatus;
        this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
        this.isDataAvailable = true;
        if (this.isDataAvailable) {
          this.enableDisableForm();
        }
        this.configureDirtyCheck();
        this.isSaveClicked = false;
      });
    }
    else {
      this.isDataAvailable = true;
    }
  }

  bindFormDataSetMonthlyOff() {
    this.otherPriceAdjustmentForm.get('isMonthlyFeeAdjustment').setValue(this.otherPriceAdjustmentData.isMonthlyFeeAdjustment);
    this.otherPriceAdjustmentData.otherPriceAdjustmentParent.forEach((expense, index) => {
      this.otherPriceAdjustmentform.push(this.priceAdjustmentData(expense, index));
    });
  }

  onCheckboxSelect() {
    this.selRow = this.selectedOtherPriceAdjustment.filter(x => x.value.isDeleted === false);
  }
  /* Other_PriceAdjustment_Form Binding */
  get otherPriceAdjustmentform() {
    return this.otherPriceAdjustmentForm.get('otherPriceAdjustmentParent') as FormArray;
  }


  private priceAdjustmentData(otherpriceadjustment: IOtherPriceAdjustment, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      otherPriceAdjustmentId: [otherpriceadjustment.otherPriceAdjustmentId],
      pipSheetId: [this.pipSheetId],
      milestoneId: [otherpriceadjustment.milestoneId],
      selectedMilestoneItem: this.getSelectedMilestoneItem(otherpriceadjustment.milestoneId).value,
      isDeleted: [otherpriceadjustment.isDeleted],
      rowType: [otherpriceadjustment.rowType],
      description: [otherpriceadjustment.description],
      totalRevenue: [otherpriceadjustment.totalRevenue],
      otherPriceAdjustmentPeriodDetail: this.otherPriceAdjustmentPeriodData(otherpriceadjustment.otherPriceAdjustmentPeriodDetail, index)
    },
      {
        validator: this.onDescriptionValidate
      }
    );
    return dataForm;
  }

  private getSelectedMilestoneItem(milestoneId: number) {
    let milestone = this.selectedMilestones.find(item => item.value.id === milestoneId);
    if (!milestone) {
      milestone = Constants.selectComboItem;
    }
    return milestone;
  }

  private otherPriceAdjustmentPeriodData(periodRevenue: IPeriodOtherPriceAdjustment[], index: number): FormArray {
    const periodRevenueData = this.fb.array([]);
    periodRevenue.forEach(period => {
      periodRevenueData.push(this.formulateOtherPriceAdjustmentPeriodForm(period, index));
    });
    return periodRevenueData;
  }

  private formulateOtherPriceAdjustmentPeriodForm(period: IPeriodOtherPriceAdjustment, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      otherPriceAdjustmentId: [period.otherPriceAdjustmentId],
      billingPeriodId: [period.billingPeriodId],
      revenue: [period.revenue]
    });
    return dataForm;
  }

  onSetMonthlyFeeChange(rowIndex: number) {
    if (this.otherPriceAdjustmentForm.value.isMonthlyFeeAdjustment) {

      const adjustmentEntryPeriodWiseCalc = this.otherPriceAdjustmentform.controls[2].get('otherPriceAdjustmentPeriodDetail').
        value[rowIndex].revenue - this.otherPriceAdjustmentform.controls[0].get('otherPriceAdjustmentPeriodDetail').value[rowIndex].revenue;

      this.otherPriceAdjustmentform.controls[1].get('otherPriceAdjustmentPeriodDetail')['controls']
      [rowIndex].get('revenue').setValue(adjustmentEntryPeriodWiseCalc);

      this.periodsTotal[rowIndex] = this.otherPriceAdjustmentform.controls[0].get('otherPriceAdjustmentPeriodDetail').value[rowIndex]
        .revenue + this.otherPriceAdjustmentform.controls[1].get('otherPriceAdjustmentPeriodDetail').value[rowIndex].revenue;

      this.adjustedRevenuePeriodsTotal[rowIndex] = adjustmentEntryPeriodWiseCalc;

      let adjustmentEntryTotal = 0, setMonthlyFeeTotal = 0;
      this.projectPeriod.forEach((period, index) => {
        adjustmentEntryTotal += + this.otherPriceAdjustmentform.controls[1].get('otherPriceAdjustmentPeriodDetail').value[index].revenue;
        setMonthlyFeeTotal += + this.otherPriceAdjustmentform.controls[2].get('otherPriceAdjustmentPeriodDetail').value[index].revenue;
      });
      this.otherPriceAdjustmentform.controls[1].get('totalRevenue').setValue(adjustmentEntryTotal);
      this.otherPriceAdjustmentform.controls[2].get('totalRevenue').setValue(setMonthlyFeeTotal);
      this.totalFeeAfterAdjustment = this.periodsTotal.reduce((a, b) => a + b);
      this.totalAdjustedRevenue = this.adjustedRevenuePeriodsTotal.reduce((a, b) => a + b);
    }
  }
  /* Other_PriceAdjustment_Form Binding end */

  onAddOtherPriceAdjustmentRow() {
    this.otherPriceAdjustmentform.markAsDirty();
    const otherPAdjustmentData = this.otherPriceAdjustmentService.
      addOtherPriceAdjustmentRow(this.pipSheetId, this.otherPriceAdjustmentData.projectPeriod);
    const uId = this.otherPriceAdjustmentform.value.length;
    const controls = this.priceAdjustmentData(otherPAdjustmentData, uId);
    this.otherPriceAdjustmentform.push(controls);
    // sync the collection with the form
    this.otherPriceAdjustmentData.otherPriceAdjustmentParent.push(otherPAdjustmentData);
  }
  get numberOfOPARows() {
    return this.otherPriceAdjustmentform.value.filter(opr => opr.isDeleted === false).length;
  }
  // Remove selected Rows
  onDeleteOtherPriceAdjustmentRow() {
    this.otherPriceAdjustmentform.markAsDirty();
    let index;
    this.selectedOtherPriceAdjustment.forEach((row) => {
      if (row.value.description !== this.feeBeforeAdjustment) {
        index = this.otherPriceAdjustmentform.controls.findIndex(OPARow => row.value.uId === OPARow.value.uId);
        const controls = (<FormGroup>this.otherPriceAdjustmentform.controls[index]).controls;
        controls.isDeleted.setValue(true);
        // should also the sync collection with the same state as that of form
        this.otherPriceAdjustmentData.otherPriceAdjustmentParent[index].isDeleted = true;
      }
    });
    this.selectedOtherPriceAdjustment = [];
    this.onCostChange();
    this.onCheckboxSelect();
  }

  // copy selected rows
  onOtherPriceAdjustmentCopySelected() {
    this.otherPriceAdjustmentform.markAsDirty();
    const copiedOtherPriceAdjustmentData: IOtherPriceAdjustment[] = [];
    let formGroup: FormGroup;
    let uId = this.otherPriceAdjustmentform.value.length;
    this.selectedOtherPriceAdjustment.forEach(row => {
      if (!row.value.isDeleted && row.value.description !== this.feeBeforeAdjustment) {
        copiedOtherPriceAdjustmentData.push(JSON.parse(JSON.stringify(row.value)));
      }
    });
    // sort by uId
    copiedOtherPriceAdjustmentData.sort(this.utilityService.compareValues('uId', 'asc'));
    copiedOtherPriceAdjustmentData.forEach(otherPriceAdjustment => {
      otherPriceAdjustment.otherPriceAdjustmentId = 0;
      otherPriceAdjustment.otherPriceAdjustmentPeriodDetail.forEach(period => {
        period.otherPriceAdjustmentId = 0;
      });
      formGroup = this.priceAdjustmentData(otherPriceAdjustment, uId);
      formGroup.markAsDirty();
      this.otherPriceAdjustmentform.push(formGroup);
      this.otherPriceAdjustmentData.otherPriceAdjustmentParent.push(otherPriceAdjustment);
      uId = uId + 1;
    });
    this.onCostChange();
    this.selectedOtherPriceAdjustment = [];
    this.onCheckboxSelect();
  }

  onOtherPriceAdjustmentMilestoneSelected(index: number, selectedValue) {
    if (this.otherPriceAdjustmentForm.controls.isMonthlyFeeAdjustment.value) {
      const milestone = this.otherPriceAdjustmentForm.get('otherPriceAdjustmentParent')['controls'][index + 1] as FormGroup;
      milestone.controls.milestoneId.setValue(selectedValue.id);
    }
    else {
      const milestone = this.otherPriceAdjustmentForm.get('otherPriceAdjustmentParent')['controls'][index] as FormGroup;
      milestone.controls.milestoneId.setValue(selectedValue.id);
    }
  }

  private getPeriods(periods: ProjectPeriod[]) {
    this.projectPeriod = periods;
    // calculate table footer colspansize
    this.colSpanSize = this.translationData.OtherPriceAdjustmentColumns.length + periods.length + 1;
    if (periods.length > 0) {
      this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
    }
  }

  // calculations Other Price Adjustment
  onCostChange() {
    if (!this.otherPriceAdjustmentForm.value.isMonthlyFeeAdjustment) {
      this.totalAdjustedRevenue = 0;
      this.setTotalAdjustedRevenuePeriodWise();
      const otherPriceAdjustmentDataTotals =
        this.otherPriceAdjustmentService.costCalculations(this.otherPriceAdjustmentform.value, this.projectPeriod);

      this.otherPriceAdjustmentform.controls.forEach((row, index) => {
        const paidRevenue = row.get('totalRevenue') as FormControl;
        paidRevenue.setValue(otherPriceAdjustmentDataTotals.amountPaidTotals[index]);
        const filteredTotalExpense = otherPriceAdjustmentDataTotals.amountPaidTotals.filter(totalExp => {
          return (isNaN(totalExp));
        });
        if (filteredTotalExpense.length > 0) {
          this.otherPriceAdjustmentForm.setErrors({ invalid: true });
        }
        else {
          this.otherPriceAdjustmentForm.setErrors(null);
        }
      });
      this.periodsTotal = otherPriceAdjustmentDataTotals.periodTotals;
      if (this.periodsTotal.length > 0) {
        this.totalFeeAfterAdjustment = this.periodsTotal.reduce((a, b) => a + b);
      }
    }
  }

  setTotalAdjustedRevenuePeriodWise() {
    const totalAdjustedRevenuePeriodTotals =
      this.otherPriceAdjustmentService.calculatetotalAdjustedRevenuePeriodWise(this.otherPriceAdjustmentform.value, this.projectPeriod);

    this.otherPriceAdjustmentform.controls.forEach((row, index) => {
      if (index > 0) {
        const paidRevenue = row.get('totalRevenue') as FormControl;
        paidRevenue.setValue(totalAdjustedRevenuePeriodTotals.amountPaidTotals[index]);
        const filteredTotalExpense = totalAdjustedRevenuePeriodTotals.amountPaidTotals.filter(totalExp => {
          return (isNaN(totalExp));
        });
        if (filteredTotalExpense.length > 0) {
          this.otherPriceAdjustmentForm.setErrors({ invalid: true });
        }
        else {
          this.otherPriceAdjustmentForm.setErrors(null);
        }
      }
    });
    this.adjustedRevenuePeriodsTotal = totalAdjustedRevenuePeriodTotals.periodTotals;
    if (this.adjustedRevenuePeriodsTotal.length > 0) {
      this.totalAdjustedRevenue = this.adjustedRevenuePeriodsTotal.reduce((a, b) => a + b);
    }
  }

  onSave(formData) {
    this.isSaveClicked = true;
    this.otherPriceAdjustmentService.saveOtherPriceAdjustment(formData).subscribe(success => {
      this.otherPriceAdjustmentService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
        this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
        this.getOverrideNotificationStatus();
      });
      // reset selected rows
      this.selectedOtherPriceAdjustment = [];
      this.getOtherPriceAdjustmentData(); // recalled get method :  ideally it should bind the Id's
      this.translateService.get('SuccessMessage.OtherPriceAdjustment').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      this.otherPriceAdjustmentForm.markAsPristine();
      this.notificationService.notifyFormDirty(false);

    }, () => {
      this.translateService.get('ErrorMessage.OtherPriceAdjustment').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
      this.isSaveClicked = false;
    });
  }

  onDescriptionValidate(group: FormGroup) {
    if (group.dirty) {
      const desc = group.get('description');
      if (desc.value === '' && +group.value.totalRevenue !== 0) {
        desc.setErrors({ isDescriptionEmpty: true });
      } else {
        desc.setErrors(null);
      }
    }
  }

  getOtherPriceAdjControls(): AbstractControl[] {
    const otherPrices = this.otherPriceAdjustmentForm.controls.otherPriceAdjustmentParent as FormArray;
    const revenueGroup = otherPrices.controls;
    return revenueGroup;
  }

  onCellClick(isContextEvent: boolean, event?, rowData?, rowIndex?: number) {
    if (!isContextEvent) {
      this.currentCellIndex = +(<string>event.target.id).split('-')[1];
      this.currentRowIndex = rowIndex;
      this.currentRowData = rowData;
    } else {

      const periods: FormArray =
        (<FormArray>this.getOtherPriceAdjControls()[this.currentRowIndex]).controls['otherPriceAdjustmentPeriodDetail'];
      this.currentCellValue = +<FormGroup>periods.controls[this.currentCellIndex].value.revenue;
      if (!isNaN(this.currentCellValue)) {
        periods.controls.forEach((lostRevenueControl, index) => {
          if (index > this.currentCellIndex) {
            (<FormGroup>lostRevenueControl).controls['revenue'].setValue(this.currentCellValue);
            if (this.otherPriceAdjustmentForm.value.isMonthlyFeeAdjustment) {
              this.onSetMonthlyFeeChange(index);
            }
            else {
              this.onCostChange();
            }
          }
        });
        this.scrollToEnd(this.currentRowIndex, (periods.length - 1).toString());
      }
    }
  }
  scrollToEnd(rowIndex: number, length: string) {
    const index = rowIndex.toString() + '-' + length;
    document.getElementById(index).focus();
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

  onSwitchChange() {
    if (this.otherPriceAdjustmentData.isMonthlyFeeAdjustment === this.otherPriceAdjustmentForm.value.isMonthlyFeeAdjustment) {
      if (this.otherPriceAdjustmentData.isMonthlyFeeAdjustment) {
        this.initializeForm();
        this.otherPriceAdjustmentForm.controls.isMonthlyFeeAdjustment.setValue(false);
        this.bindFormDataSetMonthlyOn();
      }
      else {
        this.initializeForm();
        this.otherPriceAdjustmentForm.controls.isMonthlyFeeAdjustment.setValue(false);
        this.bindFormDataSetMonthlyOff();
        this.onCostChange();
      }
    }
    else {
      if (this.otherPriceAdjustmentData.otherPriceAdjustmentParent.length > 1) {
        if (this.otherPriceAdjustmentForm.value.isMonthlyFeeAdjustment) {

          const adjustmentEntryDataForm = this.fb.group({
            uId: 1,
            otherPriceAdjustmentId: 0,
            pipSheetId: [this.pipSheetId],
            milestoneId: -1,
            selectedMilestoneItem: null,
            isDeleted: false,
            rowType: 2,
            description: 'Adjustment Entry',
            totalRevenue: 0,
            otherPriceAdjustmentPeriodDetail: this.createPeriodDataObject(1)
          });

          const setMonthlyFeeDataForm = this.fb.group({
            uId: 2,
            otherPriceAdjustmentId: 0,
            pipSheetId: [this.pipSheetId],
            milestoneId: -1,
            selectedMilestoneItem: null,
            isDeleted: false,
            rowType: 3,
            description: 'Set Monthly Fee',
            totalRevenue: 0,
            otherPriceAdjustmentPeriodDetail: this.createPeriodDataObject(2)
          });

          this.otherPriceAdjustmentform.controls.splice(1, (this.otherPriceAdjustmentform.value.length - 1));

          this.otherPriceAdjustmentform.push(adjustmentEntryDataForm);
          this.otherPriceAdjustmentform.push(setMonthlyFeeDataForm);
          let adjustmentEntryTotal = 0, setMonthlyFeeTotal = 0;
          this.projectPeriod.forEach((period, index) => {

            const adjustmentEntryPeriodWiseCalc = this.otherPriceAdjustmentform.controls[2].get('otherPriceAdjustmentPeriodDetail').
              value[index].revenue - this.otherPriceAdjustmentform.controls[0].get('otherPriceAdjustmentPeriodDetail').value[index].revenue;

            this.otherPriceAdjustmentform.controls[1].get('otherPriceAdjustmentPeriodDetail')['controls']
            [index].get('revenue').setValue(adjustmentEntryPeriodWiseCalc);

            this.periodsTotal[index] = this.otherPriceAdjustmentform.controls[0].get('otherPriceAdjustmentPeriodDetail').value[index]
              .revenue + this.otherPriceAdjustmentform.controls[1].get('otherPriceAdjustmentPeriodDetail').value[index].revenue;

            this.adjustedRevenuePeriodsTotal[index] = adjustmentEntryPeriodWiseCalc;

            adjustmentEntryTotal += this.otherPriceAdjustmentform.controls[1].get('otherPriceAdjustmentPeriodDetail').value[index].revenue;
            setMonthlyFeeTotal += this.otherPriceAdjustmentform.controls[2].get('otherPriceAdjustmentPeriodDetail').value[index].revenue;
          });
          this.otherPriceAdjustmentform.controls[1].get('totalRevenue').setValue(adjustmentEntryTotal);
          this.otherPriceAdjustmentform.controls[2].get('totalRevenue').setValue(setMonthlyFeeTotal);

          this.totalFeeAfterAdjustment = this.periodsTotal.reduce((a, b) => a + b);
          this.totalAdjustedRevenue = this.adjustedRevenuePeriodsTotal.reduce((a, b) => a + b);
        }
        else {
          this.initializeForm();
          this.otherPriceAdjustmentForm.controls.isMonthlyFeeAdjustment.setValue(false);

          const feeBeforeAdjustmentRow = this.fb.group({
            uId: 0,
            otherPriceAdjustmentId: 0,
            pipSheetId: [this.pipSheetId],
            milestoneId: -1,
            selectedMilestoneItem: null,
            isDeleted: false,
            rowType: this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].rowType,
            description: 'Fee Before Adjustment',
            totalRevenue: this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].totalRevenue,
            otherPriceAdjustmentPeriodDetail: this.otherPriceAdjustmentPeriodData(
              this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].otherPriceAdjustmentPeriodDetail, 0)
          });
          this.otherPriceAdjustmentform.push(feeBeforeAdjustmentRow);

          for (let i = 0; i < 2; i++) {
            this.otherPriceAdjustmentform.push(this.createFormObject(i + 1));
          }
          this.onCostChange();
        }
      }
    }
  }

  createPeriodDataObject(uid: number): FormArray {
    const periodRevenueData = this.fb.array([]);
    this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].otherPriceAdjustmentPeriodDetail.forEach(period => {
      const dataForm = this.fb.group({
        uId: uid,
        otherPriceAdjustmentId: 0,
        billingPeriodId: [period.billingPeriodId],
        revenue: 0
      });
      periodRevenueData.push(dataForm);
    });
    return periodRevenueData;
  }

  bindFormDataSetMonthlyOn() {
    this.otherPriceAdjustmentForm.get('isMonthlyFeeAdjustment').setValue(this.otherPriceAdjustmentData.isMonthlyFeeAdjustment);
    const feeBeforeAdjustmentRow = this.fb.group({
      uId: 0,
      otherPriceAdjustmentId: 0,
      pipSheetId: [this.pipSheetId],
      milestoneId: -1,
      selectedMilestoneItem: null,
      isDeleted: false,
      rowType: this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].rowType,
      description: 'Fee Before Adjustment',
      totalRevenue: this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].totalRevenue,
      otherPriceAdjustmentPeriodDetail: this.otherPriceAdjustmentPeriodData(
        this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].otherPriceAdjustmentPeriodDetail, 0)
    });
    this.otherPriceAdjustmentform.push(feeBeforeAdjustmentRow);

    let adjustmentEntryTotal = 0, setMonthlyFeeTotal = 0;

    const adjustmentEntryPeriodDetail = this.fb.array([]);
    this.projectPeriod.forEach((period, index) => {
      const adjustmentEntryRevenueCalc = this.otherPriceAdjustmentData.otherPriceAdjustmentParent[1].
        otherPriceAdjustmentPeriodDetail[index].revenue - this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].
          otherPriceAdjustmentPeriodDetail[index].revenue;

      const dataForm = this.fb.group({
        uId: 1,
        otherPriceAdjustmentId: 0,
        billingPeriodId: [period.billingPeriodId],
        revenue: adjustmentEntryRevenueCalc
      });
      adjustmentEntryPeriodDetail.push(dataForm);

      this.periodsTotal[index] = this.otherPriceAdjustmentData.otherPriceAdjustmentParent[0].otherPriceAdjustmentPeriodDetail[index].revenue
        + adjustmentEntryRevenueCalc;
      this.adjustedRevenuePeriodsTotal[index] = adjustmentEntryRevenueCalc;
      adjustmentEntryTotal += adjustmentEntryRevenueCalc;
      setMonthlyFeeTotal += this.otherPriceAdjustmentData.otherPriceAdjustmentParent[1].
        otherPriceAdjustmentPeriodDetail[index].revenue;
    });

    this.totalFeeAfterAdjustment = this.periodsTotal.reduce((a, b) => a + b);
    this.totalAdjustedRevenue = this.adjustedRevenuePeriodsTotal.reduce((a, b) => a + b);

    const adjustmentEntryRow = this.fb.group({
      uId: 1,
      otherPriceAdjustmentId: 0,
      pipSheetId: [this.pipSheetId],
      milestoneId: -1,
      selectedMilestoneItem: null,
      isDeleted: false,
      rowType: 2,
      description: 'Adjustment Entry',
      totalRevenue: this.otherPriceAdjustmentData.otherPriceAdjustmentParent[1].totalRevenue,
      otherPriceAdjustmentPeriodDetail: adjustmentEntryPeriodDetail
    });
    this.otherPriceAdjustmentform.push(adjustmentEntryRow);

    const setMonthlyFeeDataRow = this.fb.group({
      uId: 2,
      otherPriceAdjustmentId: this.otherPriceAdjustmentData.otherPriceAdjustmentParent[1].otherPriceAdjustmentId,
      pipSheetId: [this.pipSheetId],
      milestoneId: -1,
      selectedMilestoneItem: null,
      isDeleted: false,
      rowType: this.otherPriceAdjustmentData.otherPriceAdjustmentParent[1].rowType,
      description: 'Set Monthly Fee',
      totalRevenue: this.otherPriceAdjustmentData.otherPriceAdjustmentParent[1].totalRevenue,
      otherPriceAdjustmentPeriodDetail: this.otherPriceAdjustmentPeriodData(
        this.otherPriceAdjustmentData.otherPriceAdjustmentParent[1].otherPriceAdjustmentPeriodDetail, 2)
    });
    this.otherPriceAdjustmentform.push(setMonthlyFeeDataRow);
    this.otherPriceAdjustmentform.controls[1].get('totalRevenue').setValue(adjustmentEntryTotal);
    this.otherPriceAdjustmentform.controls[2].get('totalRevenue').setValue(setMonthlyFeeTotal);

    const milestone = this.otherPriceAdjustmentForm.get('otherPriceAdjustmentParent')['controls'][1] as FormGroup;
    milestone.controls.milestoneId.setValue(this.otherPriceAdjustmentData.otherPriceAdjustmentParent[1].milestoneId);
    milestone.controls.selectedMilestoneItem.setValue(this.getSelectedMilestoneItem(this.
      otherPriceAdjustmentData.otherPriceAdjustmentParent[1].milestoneId).value);
  }

  createFormObject(uid: number): FormGroup {
    const formObject = this.fb.group({
      uId: uid,
      otherPriceAdjustmentId: 0,
      pipSheetId: [this.pipSheetId],
      milestoneId: -1,
      selectedMilestoneItem: this.getSelectedMilestoneItem(-1).value,
      isDeleted: false,
      rowType: 3,
      description: '',
      totalRevenue: 0,
      otherPriceAdjustmentPeriodDetail: this.createPeriodDataObject(uid)
    },
      {
        validator: this.onDescriptionValidate
      });
    return formObject;
  }

  onRowUnselect() {
    if (this.selectedOtherPriceAdjustment.find(x => x.value.uId === 0)) {
      this.selectedOtherPriceAdjustment.splice(0, 1);
    }
  }
}

