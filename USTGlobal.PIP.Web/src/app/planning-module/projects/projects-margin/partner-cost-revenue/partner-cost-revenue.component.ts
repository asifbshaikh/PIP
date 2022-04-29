import { OverrideNotificationStatus } from './../../../../shared-module/domain/override-notification-status';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { PartnerCostRevenueService } from '@shared/services/partner-cost-revenue.service';
import { NotificationService, SharedDataService } from '@global';
import { MessageService, SelectItem, MenuItem } from 'primeng/api';
import { Subscription } from 'rxjs';
import { ICurrency, IPartnerCostRevenue, IPartnerCost, IPartnerRevenue, ProjectPeriod, Mastermapper, Constants, Milestone } from '@shared';
import { TableHeaderCheckbox } from 'primeng/table';
import { ResourceMapper } from '@shared/mapper/master/resourcemapper';
import { DateService } from '@core/services/date.service';
import { IPeriodPartnerCost } from '@shared/domain/IPeriodPartnerCost';
import { IPeriodPartnerRevenue } from '@shared/domain/IPeriodPartnerRevenue';
import { startWith, pairwise } from 'rxjs/operators';
import { isNullOrUndefined } from 'util';
import { UtilityService } from '@core';

@Component({
  selector: 'partner-cost-revenue',
  templateUrl: './partner-cost-revenue.component.html',
})
export class PartnerCostRevenueComponent implements OnInit {

  partnerCostRevenueform: FormGroup;
  partnerCostRevenueData: IPartnerCostRevenue = { partnerCost: [], partnerRevenue: [], projectMilestone: [], projectPeriod: [] };
  selectedMilestones: SelectItem[];
  projectPeriod: ProjectPeriod[];
  translationData: any;
  currencyData: ICurrency;
  subscriptions: Subscription[] = [];
  pipSheetId: number;
  projectId: number;
  selectedPartnerCost: any[] = [];
  selectedPartnerRevenue: any[] = [];
  periodCols: any[] = [];
  costCols: any[] = [];
  revenueCols: any[] = [];
  colSpanSize = 0;
  partnerCostPeriodsTotal: number[] = [];
  partnerRevenuePeriodsTotal: number[] = [];
  paidAmountTotal = 0;
  revenueAmountTotal = 0;
  disableOtherFormControls = false;
  display = false;
  // Partner cost
  pcContextMenuItems: MenuItem[];
  pcIsCellActive = false;
  pcCurrentCellIndex = -1;
  pcCurrentCellValue = -1;
  pcCurrentRowIndex = -1;
  pcCurrentRowData: any;
  selCostRow: any[] = [];

  // partner revenue
  contextMenuItems: MenuItem[];
  isCellActive = false;
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
  dashboardId: number;
  isSaveClicked = false;
  selRevenueRow: any[] = [];

  constructor(
    private dateService: DateService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private partnerCostRevenueService: PartnerCostRevenueService,
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
    this.pcContextMenuItems = [{
      label: 'REPEAT >>',
      command: (event: any) => {
        this.onPartnerCostCellClick(true);
      }
    }];

    this.contextMenuItems = [{
      label: 'REPEAT >>',
      command: (event: any) => {
        this.onCellClick(true);
      }
    }];

    this.initializeForm();
    this.translationData = {};
    this.translateService.get('PartnerCostRevenue').subscribe(data => {
      this.translationData = data;
    });

    // cost columns
    this.translateService.get('PartnerCostRevenue.partnerCostColumns').subscribe(data => {
      this.costCols = data;
    });

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });

    this.getPartnerRevenueCostData();
  }

  initializeForm() {
    this.partnerCostRevenueform = this.fb.group({
      partnerCost: this.fb.array([]),
      partnerCostTotal: this.fb.array([]),
      partnerRevenue: this.fb.array([]),
      partnerRevenueTotal: this.fb.array([])
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
        this.partnerCostRevenueform.disable();
      }, 200);
    }
  }

  configureDirtyCheck() {
    this.partnerCostRevenueform.valueChanges.subscribe(() => {
      if (this.partnerCostRevenueform.dirty) {
        this.notificationService.notifyFormDirty(true);
      }
    });
  }

  getPartnerRevenueCostData() {
    this.partnerCostRevenueData = { partnerCost: [], partnerRevenue: [], projectMilestone: [], projectPeriod: [] };
    this.partnerCostPeriodsTotal = [];
    this.partnerRevenuePeriodsTotal = [];
    this.initializeForm();
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
    // Get Partner cost/revenue
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.partnerCostRevenueService.getPartnerCostRevenue(this.pipSheetId)
        .subscribe(data => {
          this.getPeriods(data.projectPeriod);
          // check partner cost data available or fetch default data
          if (data.partnerCost.length > 0) {
            this.partnerCostRevenueData.partnerCost = data.partnerCost;
          } else {
            this.partnerCostRevenueData.partnerCost = this.partnerCostRevenueService
              .getDefaultPartnerCost(this.projectPeriod, this.pipSheetId);
          }

          // check partner revenue data available else fetch default data
          if (data.partnerRevenue.length > 0) {
            this.partnerCostRevenueData.partnerRevenue = data.partnerRevenue;
          } else {
            this.partnerCostRevenueData.partnerRevenue = this.partnerCostRevenueService
              .getDefaultPartnerRevenue(this.projectPeriod, this.pipSheetId);
          }
          this.partnerCostRevenueData.projectPeriod = data.projectPeriod;

          this.selectedMilestones = new Mastermapper().getOptionalPhaseComboItems(data.projectMilestone, false);
          this.bindFormData();
          this.onCostChange();
          this.onRevenueChange();
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

  bindFormData() {

    // Partner cost
    this.partnerCostRevenueData.partnerCost.forEach((data, index) => {
      this.partnerCostForm.push(this.partnerCostData(data, index));
    });

    // Partner Revenue
    this.partnerCostRevenueData.partnerRevenue.forEach((data, index) => {
      this.partnerRevenueForm.push(this.partnerRevenueData(data, index));
    });
  }

  /* partner cost form binding start */
  get partnerCostForm() {
    return this.partnerCostRevenueform.get('partnerCost') as FormArray;
  }

  private partnerCostData(partnerCost: IPartnerCost, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      partnerCostId: [partnerCost.partnerCostId],
      pipSheetId: [this.pipSheetId],
      milestoneId: [partnerCost.milestoneId],
      selectedMilestoneItem: this.getSelectedMilestoneItem(partnerCost.milestoneId).value,
      isDeleted: [partnerCost.isDeleted],
      description: [partnerCost.description],
      paidAmount: [partnerCost.paidAmount],
      setMargin: [partnerCost.setMargin ? partnerCost.setMargin : false],
      marginPercent: [partnerCost.marginPercent],
      partnerCostPeriodDetail: this.partnerCostPeriodData(partnerCost.partnerCostPeriodDetail, index)
    },
      { validator: this.onCostDescriptionValidate }
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

  private partnerCostPeriodData(periodCost: IPeriodPartnerCost[], index: number): FormArray {
    const periodCostData = this.fb.array([]);
    periodCost.forEach(period => {
      periodCostData.push(this.formulatePartnerCostPeriodForm(period, index));
    });
    return periodCostData;
  }

  private formulatePartnerCostPeriodForm(period: IPeriodPartnerCost, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      partnerCostId: [period.partnerCostId],
      billingPeriodId: [period.billingPeriodId],
      cost: [period.cost]
    });
    return dataForm;
  }

  /* partner cost form binding end */

  /* partner Revenue form binding start */
  get partnerRevenueForm() {
    return this.partnerCostRevenueform.get('partnerRevenue') as FormArray;
  }

  private partnerRevenueData(partnerRevenue: IPartnerRevenue, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      partnerRevenueId: [partnerRevenue.partnerRevenueId],
      pipSheetId: [this.pipSheetId],
      milestoneId: [partnerRevenue.milestoneId],
      selectedMilestoneItem: this.getSelectedMilestoneItem(partnerRevenue.milestoneId).value,
      isDeleted: [partnerRevenue.isDeleted],
      description: [partnerRevenue.description],
      revenueAmount: [partnerRevenue.revenueAmount],
      setMargin: [partnerRevenue.setMargin ? partnerRevenue.setMargin : false],
      partnerCostUId: [partnerRevenue.partnerCostUId],
      marginPercent: [partnerRevenue.marginPercent],
      partnerRevenuePeriodDetail: this.partnerRevenuePeriodData(partnerRevenue.partnerRevenuePeriodDetail, index)
    },
      { validator: this.onRevenueDescriptionValidate }
    );

    return dataForm;
  }

  private partnerRevenuePeriodData(periodRevenue: IPeriodPartnerRevenue[], index: number): FormArray {
    const periodRevenueData = this.fb.array([]);
    periodRevenue.forEach(period => {
      periodRevenueData.push(this.formulatePartnerRevenuePeriodForm(period, index));
    });

    return periodRevenueData;
  }

  private formulatePartnerRevenuePeriodForm(period: IPeriodPartnerRevenue, index: number): FormGroup {
    const revenue = +period.revenue;
    const dataForm = this.fb.group({
      uId: [index],
      partnerRevenueId: [period.partnerRevenueId],
      billingPeriodId: [period.billingPeriodId],
      revenue: revenue !== 0 ? revenue.toFixed(2) : revenue
    });
    return dataForm;
  }

  get numberOfPCRows() {
    return this.partnerCostForm.value.filter(pr => pr.isDeleted === false).length;
  }
  /* partner cost form binding end */

  get numberOfPRRows() {
    return this.partnerRevenueForm.value.filter(pr => pr.isDeleted === false && !pr.setMargin).length;
  }

  onAddPartnerCostRow() {
    this.partnerCostForm.markAsDirty();
    const costData = this.partnerCostRevenueService.
      addPartnerCostRow(this.pipSheetId, this.partnerCostRevenueData.projectPeriod);
    const uId = this.partnerCostRevenueData.partnerCost.length;
    const controls = this.partnerCostData(costData, uId);
    this.partnerCostForm.push(controls);
    // sync the collection with the form
    this.partnerCostRevenueData.partnerCost.push(costData);
  }

  onAddPartnerRevenueRow() {
    this.partnerRevenueForm.markAsDirty();
    const revenueData = this.partnerCostRevenueService.
      addPartnerRevenueRow(this.pipSheetId, this.partnerCostRevenueData.projectPeriod);
    const uId = this.partnerCostRevenueData.partnerRevenue.length;
    const controls = this.partnerRevenueData(revenueData, uId);
    this.partnerRevenueForm.push(controls);
    // sync the collection with the form
    this.partnerCostRevenueData.partnerRevenue.push(revenueData);
  }

  // Remove selected Rows from partner Cost grid
  onCostRowDelete() {
    const hasSetMargin = this.selectedPartnerCost.find(row => row.value.setMargin === true);
    if (hasSetMargin) {
      this.display = true;
    }
    else {
      this.onDeletePartnerCostRow();
    }
  }

  onDeletePartnerCostRow() {
    let index;
    let revenueRowIndex;
    this.partnerCostForm.markAsDirty();
    this.selectedPartnerCost.forEach((row) => {
      index = this.partnerCostForm.controls.findIndex(costRow => row.value.uId === costRow.value.uId);
      const controls = (<FormGroup>this.partnerCostForm.controls[index]).controls;
      const periods: FormArray = controls.partnerCostPeriodDetail as FormArray;
      this.resetPartnerCostPeriods(periods);
      controls.isDeleted.setValue(true);
      controls.marginPercent.reset();
      controls.description.reset();
      if (row.value.setMargin) {
        revenueRowIndex = (<IPartnerRevenue[]>this.partnerRevenueForm.value).
          findIndex(revenueRow => revenueRow.partnerCostUId === row.value.uId);

        const reveuneControls = (<FormGroup>this.partnerRevenueForm.controls[revenueRowIndex]).controls;
        const revenuePeriods: FormArray = reveuneControls.partnerRevenuePeriodDetail as FormArray;
        this.resetPartnerRevenuePeriods(revenuePeriods);
        reveuneControls.isDeleted.setValue(true);
        this.partnerCostRevenueData.partnerRevenue[revenueRowIndex].isDeleted = true;
        this.onRevenueChange();
      }
      // should also the sync collection with the same state as that of form
      this.partnerCostRevenueData.partnerCost[index].isDeleted = true;
    });
    this.selectedPartnerCost = [];
    this.onCheckboxSelect();
    this.onCostChange();
  }

  // on partner cost copy selected
  onPartnerCostCopySelected() {
    this.partnerCostForm.markAsDirty();
    const copiedPartnerCostData: IPartnerCost[] = [];
    let formGroup: FormGroup;
    let uId = this.partnerCostRevenueData.partnerCost.length;
    this.selectedPartnerCost.forEach(row => {

      copiedPartnerCostData.push(JSON.parse(JSON.stringify(row.value)));
    });
    copiedPartnerCostData.sort(this.utilityService.compareValues('uId', 'asc'));
    copiedPartnerCostData.forEach(partnerCost => {
      partnerCost.partnerCostId = 0;
      partnerCost.partnerCostPeriodDetail.forEach(period => {
        period.partnerCostId = 0;
      });
      formGroup = this.partnerCostData(partnerCost, uId);
      formGroup.markAsDirty();
      this.partnerCostForm.push(formGroup);
      // sync the collection with the form
      this.partnerCostRevenueData.partnerCost.push(partnerCost);
      if (formGroup.value.setMargin) {
        this.onSetMarginChange(formGroup, { checked: true });
      }
      uId = uId + 1;
    });
    this.onCostChange();
    this.selectedPartnerCost = [];
    this.onCheckboxSelect();
  }

  resetPartnerCostPeriods(periods: FormArray) {
    periods.controls.forEach(period => {
      const cost = period.get('cost');
      cost.setValue(0);
    });
  }

  onDeletePartnerRevenueRow() {
    this.partnerRevenueForm.markAsDirty();

    let index;
    this.selectedPartnerRevenue.forEach((row) => {
      index = this.partnerRevenueForm.controls.findIndex(revenueRow => (row.value.uId === revenueRow.value.uId) && !row.value.setMargin);
      if (index !== -1) {
        const controls = (<FormGroup>this.partnerRevenueForm.controls[index]).controls;
        const periods: FormArray = controls.partnerRevenuePeriodDetail as FormArray;
        this.resetPartnerRevenuePeriods(periods);
        controls.isDeleted.setValue(true);
        // should also the sync collection with the same state as that of form
        this.partnerCostRevenueData.partnerRevenue[index].isDeleted = true;
      }
    });
    this.selectedPartnerRevenue = [];
    this.onRevenueChange();
    this.onCheckboxSelect();
  }

  // on partner cost copy selected
  onPartnerRevenueCopySelected() {
    this.partnerRevenueForm.markAsDirty();
    const copiedPartnerRevenueData: IPartnerRevenue[] = [];
    let formGroup: FormGroup;
    let uId = this.partnerCostRevenueData.partnerRevenue.length;
    this.selectedPartnerRevenue.forEach(row => {
      if (!row.value.setMargin) {
        copiedPartnerRevenueData.push(JSON.parse(JSON.stringify(row.value)));
      }
    });
    copiedPartnerRevenueData.sort(this.utilityService.compareValues('uId', 'asc'));
    copiedPartnerRevenueData.forEach(partnerRevenue => {
      partnerRevenue.partnerRevenueId = 0;
      partnerRevenue.partnerRevenuePeriodDetail.forEach(period => {
        period.partnerRevenueId = 0;
      });
      formGroup = this.partnerRevenueData(partnerRevenue, uId);
      formGroup.markAsDirty();
      this.partnerRevenueForm.push(formGroup);
      this.partnerCostRevenueData.partnerRevenue.push(partnerRevenue);
      uId = uId + 1;
    });
    this.onRevenueChange();
    this.selectedPartnerRevenue = [];
    this.onCheckboxSelect();
  }

  onCheckboxSelect() {
    this.selCostRow = this.selectedPartnerCost.filter(x => x.value.isDeleted === false);
    this.selRevenueRow = this.selectedPartnerRevenue.filter(x => x.value.isDeleted === false && x.value.setMargin === false);
  }

  resetPartnerRevenuePeriods(periods: FormArray) {
    periods.controls.forEach(period => {
      const revenue = period.get('revenue');
      revenue.setValue(0);
    });
  }

  onCostMilestoneSelected(index: number, selectedValue) {
    const milestone = this.partnerCostRevenueform.get('partnerCost')['controls'][index] as FormGroup;
    milestone.controls.milestoneId.setValue(selectedValue.id);
  }

  onRevenueMilestoneSelected(index: number, selectedValue) {
    const milestone = this.partnerCostRevenueform.get('partnerRevenue')['controls'][index] as FormGroup;
    milestone.controls.milestoneId.setValue(selectedValue.id);
  }

  private getPeriods(periods: ProjectPeriod[]) {
    this.projectPeriod = periods;
    // calculate table footer colspansize
    this.colSpanSize = this.translationData.partnerCostColumns.length + periods.length + 1;
    if (periods.length > 0) {
      this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
    }
  }

  onSave(formData) {
    this.isSaveClicked = true;
    this.partnerCostRevenueService.savePartnerCostRevenue(formData).subscribe(success => {
      this.partnerCostRevenueService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        this.notificationService.notifyTotalClientPriceExists(isNullOrUndefined(headerInfo.header1.totalClientPrice) ? null
          : headerInfo.header1.totalClientPrice.toString());
        this.notificationService.notifyPercentEbitdaExists(isNullOrUndefined(headerInfo.headerEbitda) ? null :
          headerInfo.headerEbitda.projectEBITDAPercent.toString());
      });
      this.selectedPartnerCost = [];
      this.selectedPartnerRevenue = [];
      this.getPartnerRevenueCostData();
      this.translateService.get('SuccessMessage.PartnerCostAndRevenue').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      this.partnerCostRevenueform.markAsPristine();
      this.notificationService.notifyFormDirty(false);
      this.getOverrideNotificationStatus();
    }, () => {
      this.translateService.get('ErrorMessage.PartnerCostAndRevenue').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
      this.isSaveClicked = false;
    });
  }

  onCostChange() {
    const costDataTotals = this.partnerCostRevenueService.costCalculations(this.partnerCostForm.value, this.projectPeriod);

    this.partnerCostForm.controls.forEach((row, index) => {
      const paidAmount = row.get('paidAmount') as FormControl;
      paidAmount.setValue(costDataTotals.amountPaidTotals[index]);
    });
    const filteredTotalPartnerCost = costDataTotals.amountPaidTotals.filter(totalCost => {
      return (isNaN(totalCost));
    });
    if (filteredTotalPartnerCost.length > 0) {
      this.partnerCostForm.setErrors({ invalid: true });
    }
    else {
      this.partnerCostForm.setErrors(null);
    }
    this.partnerCostPeriodsTotal = costDataTotals.periodTotals;
    if (this.partnerCostPeriodsTotal.length > 0) {
      this.paidAmountTotal = this.partnerCostPeriodsTotal.reduce((a, b) => a + b);
    }
  }

  onRevenueChange() {
    const revenueDataTotals = this.partnerCostRevenueService.revenueCalculations(this.partnerRevenueForm.value, this.projectPeriod);

    this.partnerRevenueForm.controls.forEach((row, index) => {
      const revenueAmount = row.get('revenueAmount') as FormControl;
      revenueAmount.setValue(revenueDataTotals.revenueAmountTotals[index]);
    });
    this.partnerRevenuePeriodsTotal = revenueDataTotals.periodTotals;
    if (this.partnerRevenuePeriodsTotal.length > 0) {
      this.revenueAmountTotal = this.partnerRevenuePeriodsTotal.reduce((a, b) => a + b);
    }
  }

  onCostDescriptionValidate(group: FormGroup) {
    if (group.dirty) {
      const desc = group.get('description');
      if (desc.value === '' && (+group.value.paidAmount !== 0 || +group.value.setMargin)) {
        desc.setErrors({ isDescriptionEmpty: true });
      } else {
        desc.setErrors(null);
      }
    }
  }

  onRevenueDescriptionValidate(group: FormGroup) {
    if (group.dirty) {
      const desc = group.get('description');
      if (desc.value === '' && +group.value.revenueAmount > 0) {
        desc.setErrors({ isDescriptionEmpty: true });
      } else {
        desc.setErrors(null);
      }
    }
  }

  onPartnerCostCellClick(isContextEvent: boolean, rowData?, event?, rowIndex?: number) {

    if (!isContextEvent) {
      this.pcCurrentCellIndex = +(<string>event.target.id).split('a')[1];
      this.pcIsCellActive = (this.pcCurrentCellIndex >= 0) ? true : false;
      this.pcCurrentRowIndex = rowIndex;
      this.pcCurrentRowData = rowData;
    } else {
      const periods = (<FormArray>this.partnerCostForm.controls[this.pcCurrentRowIndex]).controls['partnerCostPeriodDetail'];
      this.pcCurrentCellValue = +<FormGroup>periods.value[this.pcCurrentCellIndex].cost;

      periods.controls.forEach((expenseControl, index) => {
        if (index > this.pcCurrentCellIndex) {
          (<FormGroup>expenseControl).controls['cost'].setValue(this.pcCurrentCellValue);
        }
      });
      this.onCostChange();
      this.onPartnerCostValueChange(this.pcCurrentRowData);
      this.pcIsCellActive = false;
      this.pcScrollToEnd(this.pcCurrentRowIndex, (periods.length - 1).toString());
    }
  }

  pcScrollToEnd(rowIndex: number, length: string) {
    const index = rowIndex.toString() + 'a' + length;
    document.getElementById(index).focus();
  }

  onCellClick(isContextEvent: boolean, rowData?, event?, rowIndex?: number) {
    if (!isContextEvent) {
      this.currentCellIndex = +(<string>event.target.id).split('b')[1];
      this.isCellActive = (this.currentCellIndex >= 0) ? true : false;
      this.currentRowIndex = rowIndex;
      this.currentRowData = rowData;
    } else {
      const periods = (<FormArray>this.partnerRevenueForm.controls[this.currentRowIndex]).controls['partnerRevenuePeriodDetail'];
      this.currentCellValue = +<FormGroup>periods.value[this.currentCellIndex].revenue;

      periods.controls.forEach((expenseControl, index) => {
        if (index > this.currentCellIndex) {
          (<FormGroup>expenseControl).controls['revenue'].setValue(this.currentCellValue);
        }
      });

      this.onRevenueChange();
      this.isCellActive = false;
      this.scrollToEnd(this.currentRowIndex, (periods.length - 1).toString());

    }
  }
  scrollToEnd(rowIndex: number, length: string) {
    const index = rowIndex.toString() + 'b' + length;
    document.getElementById(index).focus();
  }

  getOverrideNotificationStatus() {
    let overrideNotification: OverrideNotificationStatus;
    this.userWorkflowService.getOverrideNotificationStatus(+this.pipSheetId).subscribe(item => {
      overrideNotification = item;
      if (overrideNotification.clientPrice || overrideNotification.riskManagement
        || overrideNotification.vacationAbsence || overrideNotification.ebitdaStdOverhead) {
        this.notificationService.showNotificationDialog(this.pipSheetId);
      }
    });
  }

  onSetMarginChange(rowData, event) {
    const copiedRow = this.partnerRevenueForm.value.find(pr =>
      (!isNullOrUndefined(pr.partnerCostUId) && pr.partnerCostUId === rowData.value.uId));
    const uId = this.partnerRevenueForm.value.length;
    this.onCostDescriptionValidate(rowData);
    if (!isNullOrUndefined(copiedRow) && copiedRow.isDeleted) {
      this.partnerRevenueForm.controls[copiedRow.uId].get('isDeleted').setValue(false);
    }
    else {

      if (event.checked && (isNullOrUndefined(copiedRow))) {    // Copy new Revenue Row
        const row: IPartnerRevenue = {
          partnerRevenueId: 0,
          isDeleted: rowData.value.isDeleted,
          description: rowData.value.description,
          marginPercent: rowData.value.marginPercent,
          milestoneId: rowData.value.milestoneId,
          revenueAmount: rowData.value.paidAmount,
          setMargin: rowData.value.setMargin,
          partnerCostUId: rowData.value.uId,
          partnerRevenuePeriodDetail: this.copiedPeriods(rowData.value, uId),
          pipSheetId: this.pipSheetId,
          uId: uId
        };
        this.partnerRevenueForm.push(this.partnerRevenueData(row, uId));
        this.partnerCostRevenueData.partnerRevenue.push(row);
      }
      else if (event.checked && copiedRow.partnerCostUId) {    // update existing Revenue Row
        this.onPartnerCostValueChange(rowData);
      }

      else {                                                // Reset Revenue Row
        const revenueRowIndex = this.partnerRevenueForm.value.findIndex
          (revenueRow => revenueRow.partnerCostUId === copiedRow.partnerCostUId);
        this.partnerCostForm.controls[rowData.value.uId].get('marginPercent').setValue(0);
        this.partnerRevenueForm.controls[revenueRowIndex].get('marginPercent').setValue(0);
        this.partnerRevenueForm.controls[revenueRowIndex].get('revenueAmount').setValue(0);
        this.partnerRevenueForm.controls[revenueRowIndex].get('partnerRevenuePeriodDetail').
          setValue(this.copiedPeriods(rowData.value, revenueRowIndex));
        this.partnerRevenueForm.controls[revenueRowIndex].get('setMargin').setValue(event.checked);
      }
    }
    this.onRevenueChange();
    this.onCheckboxSelect();
  }

  private copiedPeriods(rowData, uId) {
    const periodRevenueData = [];
    rowData.partnerCostPeriodDetail.forEach(period => {
      const revenueCost = period.cost / (1 - (rowData.marginPercent / 100));
      let dataForm;
      if (rowData.setMargin && +rowData.marginPercent !== 100) {
        dataForm = {
          uId: uId,
          partnerRevenueId: 0,
          billingPeriodId: period.billingPeriodId,
          revenue: isNaN(revenueCost) ? 0 : +revenueCost.toFixed(2)
        };
      }
      else {
        dataForm = {
          uId: uId,
          partnerRevenueId: 0,
          billingPeriodId: period.billingPeriodId,
          revenue: 0
        };
      }
      periodRevenueData.push(dataForm);
      return dataForm;
    });
    return periodRevenueData;
  }

  onPartnerCostValueChange(rowData) {
    const copiedRow = this.partnerRevenueForm.value.find(pr => pr.partnerCostUId === rowData.value.uId);
    const revenueRowIndex = this.partnerRevenueForm.value.findIndex(revenueRow => revenueRow.partnerCostUId === copiedRow.partnerCostUId);
    if (rowData.value.setMargin) {
      this.partnerRevenueForm.controls[revenueRowIndex].get('setMargin').setValue(rowData.value.setMargin);
      this.partnerRevenueForm.controls[revenueRowIndex].get('selectedMilestoneItem').setValue(rowData.value.selectedMilestoneItem);
      this.partnerRevenueForm.controls[revenueRowIndex].get('milestoneId').setValue(rowData.value.selectedMilestoneItem.id);
      this.partnerRevenueForm.controls[revenueRowIndex].get('description').setValue(rowData.value.description);
      if (rowData.value.marginPercent > 0) {
        this.partnerRevenueForm.controls[revenueRowIndex].get('marginPercent').setValue(rowData.value.marginPercent);
      }
      else {
        this.partnerRevenueForm.controls[revenueRowIndex].get('marginPercent').setValue(0);
        this.partnerCostForm.controls[rowData.value.uId].get('marginPercent').setErrors({ 'invalid': true });
      }
      this.partnerRevenueForm.controls[revenueRowIndex].get('partnerRevenuePeriodDetail').setValue(this.copiedPeriods(rowData.value,
        revenueRowIndex));
    }
    this.onRevenueChange();
  }
}
