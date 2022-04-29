import { OverrideNotificationStatus } from './../../../../shared-module/domain/override-notification-status';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IRoleAndAccount } from './../../../../shared-module/domain/IRoleAndAccount';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, FormControl, FormBuilder, FormArray, Validators, ValidationErrors, AbstractControl, FormsModule } from '@angular/forms';
import { DirectexpensesService } from '@shared/services/directexpenses.service';
import { ActivatedRoute } from '@angular/router';
import { ExpensesAndAssetsMapper } from '@shared/mapper/master/expenseandassetmapper';
import { SharedDataService } from '@global';
import { Mastermapper, ResourceMapper, ProjectPeriod, Constants } from '@shared';
import { SelectItem, MessageService, MenuItem } from 'primeng/api';
import { IDefaultLabel } from '@shared/domain/defaultlabel';
import { IAsset } from '@shared/domain/asset';
import { IDirectExpense } from '@shared/domain/directexpense';
import { IDirectExpensesPeriod } from '@shared/domain/directexpensesperiod';
import { DateService } from '@core/services/date.service';
import { UtilityService } from '@core';
import { NotificationService } from '@global';

@Component({
  selector: 'expenses-and-assets',
  templateUrl: './expenses-and-assets.component.html'
})
export class ExpensesAndAssetsComponent implements OnInit {
  projectId: number;
  pipSheetId: number;
  dashboardId: number;
  expenseAndAssetsData: ExpensesAndAssetsMapper;
  selectedMilestones: SelectItem[];
  defaultLabels: IDefaultLabel[];
  periodCols: any[] = [];
  form: FormGroup;
  directExpenseCols: any[] = [];
  basicAssetscols: any[] = [];
  additionalAssetsCols: any[] = [];
  selectedDirectExpense: any[] = [];
  selectedAdditionalAssets: any[] = [];
  totalAssetCost = 0;
  assetMaxDayCharge: number;
  monthlyAssetCost: number[] = [];
  monthlyTotals: number[] = [];
  totalDirectExpenseCost = 0;
  colSpanSize = 0;
  currencyFactor = 0;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  disableOtherFormControls = false;
  // new
  contextMenuItems: MenuItem[];
  isCellActive = false;
  currentCellIndex = -1;
  currentCellValue = -1;
  currentRowIndex = -1;
  currentRowData: any;
  isSaveClicked = false;
  selExpenseRow: any[] = [];
  selAssetsRow: any[] = [];

  constructor(
    private translateService: TranslateService,
    private directExpensesService: DirectexpensesService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private sharedDataService: SharedDataService,
    private dateService: DateService,
    private messageService: MessageService,
    private utilityService: UtilityService,
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
        // this.isCellActive = false;
      }
    }];


    this.assetMaxDayCharge = Constants.assetMaxDayCharge;
    this.initializeForm();

    // fetch asset columns
    this.translateService.get('ExpensesAndAssets.BasicAssets').subscribe(cols => {
      this.basicAssetscols = cols;
    });

    // fetch Additonal assets columns
    this.translateService.get('ExpensesAndAssets.AdditionalSoftwareOrHardware').subscribe(cols => {
      this.additionalAssetsCols = cols;
    });

    // Direct expenses columns
    this.translateService.get('ExpensesAndAssets.ExpensesAndAssetsColumns').subscribe(cols => {
      this.directExpenseCols = cols;
    });

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });

    this.getAssetsAndDirectExpensesData();

    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.currencyFactor = this.sharedDataService.sharedData.currencyDTO.find(c =>
        c.currencyId === this.sharedDataService.sharedData.currencyId).usdToLocal;
    }
  }

  initializeForm() {
    this.form = this.fb.group({
      assetDTO: this.fb.array([]),
      directExpenseDTO: this.fb.array([])
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

  bindFormData() {
    this.currencyFactor = this.sharedDataService.sharedData.currencyDTO.find(c =>
      c.currencyId === this.sharedDataService.sharedData.currencyId).usdToLocal;

    // assets
    if (this.expenseAndAssetsData !== null) {
      this.expenseAndAssetsData.assetDTO.forEach((asset, index) => {
        this.assetsForm.push(this.assetsData(asset, index));
        this.onAssetChange(index);
      });
      this.displayTotalAssetCost();

      // direct expenses
      this.expenseAndAssetsData.directExpenseDTO.forEach((expense, index) => {
        this.directExpensesForm.push(this.directExpensesData(expense, index));
        this.onExpenseChange(index);
      });
      this.displayMonthlyAssetCost();
    }
  }

  getAssetsAndDirectExpensesData() {
    this.translateService.get('ExpensesAndAssets.ExpensesAndAssetsColumns').subscribe(cols => {
      this.directExpenseCols = cols;
    });
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.directExpensesService.
        getDirectExpensesAndAssetsDetails(this.pipSheetId).subscribe(data => {
          this.initializeForm();
          this.expenseAndAssetsData = null;

          if (data.projectPeriodDTO != null) {
            this.getPeriods(data.projectPeriodDTO);

            if (data.assetDTO.length > 0) {
              // already saved data against pipsheet
              this.expenseAndAssetsData = data;
              this.expenseAndAssetsData.assetDTO = this.directExpensesService.getDefaultBasicAssets(this.pipSheetId, data.assetDTO);
            } else {
              this.expenseAndAssetsData = new ExpensesAndAssetsMapper();
              this.expenseAndAssetsData.projectPeriodDTO = data.projectPeriodDTO;
              this.expenseAndAssetsData.assetDTO = this.directExpensesService.getDefaultBasicAssets(this.pipSheetId);
              this.expenseAndAssetsData.directExpenseDTO = this.directExpensesService.
                getDefaultDirectExpenses(data.directExpenseDTO, data.projectPeriodDTO, this.pipSheetId);
              this.expenseAndAssetsData.periodLaborRevenueDTO = data.periodLaborRevenueDTO;
            }
          }
          this.selectedMilestones = new Mastermapper().getOptionalPhaseComboItems(data.projectMilestoneDTO, false);
          this.defaultLabels = this.sharedDataService.sharedData.defaultLabelDTO;
          this.bindFormData();
          this.pipSheetWorkflowStatus = this.sharedDataService.sharedData.pipSheetWorkflowStatus;
          this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);

          this.isDataAvailable = true;
          this.enableDisableForm();
          this.configureDirtyCheck();
          this.isSaveClicked = false;
        });
    }
    else {
      this.isDataAvailable = true;
    }
  }

  get assetsForm() {
    return this.form.get('assetDTO') as FormArray;
  }

  get directExpensesForm() {
    return this.form.get('directExpenseDTO') as FormArray;
  }

  get asset() {
    return this.assetsForm.controls;
  }

  get directExpense() {
    return this.directExpensesForm.controls;
  }

  get numberOfAssetRows() {
    return this.assetsForm.value.filter(asset => asset.isDeleted === false).length;
  }
  get numberOfDirectExpenseRows() {
    return this.directExpensesForm.value.filter(de => de.isDeleted === false).length;
  }

  private assetsData(asset: IAsset, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      projectAssetId: [asset.projectAssetId],
      pipSheetId: [asset.pipSheetId],
      basicAssetId: [asset.basicAssetId],
      description: [asset.description],
      costToProject: [asset.costToProject],
      count: [asset.count],
      amount: [asset.amount],
      isDeleted: [asset.isDeleted],
      createdBy: [1, []],
      updatedBy: [1, []]
    },
      {
        validator: this.descriptionValidator
      }
    );

    return dataForm;
  }

  private directExpensesData(expense: IDirectExpense, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      directExpenseId: [expense.directExpenseId],
      pipSheetId: [expense.pipSheetId],
      milestoneId: [expense.milestoneId],
      selectedMilestoneItem: this.getSelectedMilestoneItem(expense.milestoneId).value,
      label: [expense.label],
      description: [expense.description],
      totalExpense: [expense.totalExpense],
      isDeleted: [expense.isDeleted],
      isReimbursable: [expense.isReimbursable],
      percentRevenue: [expense.percentRevenue > 0 ? expense.percentRevenue : ''],
      createdBy: [1, []],
      updatedBy: [1, []],
      directExpensePeriodDTO: this.directExpensePeriodData(expense.directExpensePeriodDTO, index)
    },
      {
        validator: this.descriptionDirectValidator
      });

    return dataForm;
  }

  private getSelectedMilestoneItem(milestoneId: number) {
    let milestone = this.selectedMilestones.find(item => item.value.id === milestoneId);
    if (!milestone) {
      milestone = Constants.selectComboItem;
    }
    return milestone;
  }

  private directExpensePeriodData(periodExpenses: IDirectExpensesPeriod[], index: number): FormArray {
    const periodExpensesData = this.fb.array([]);
    this.associateBillingPeriodId(periodExpenses);
    periodExpenses.forEach(period => {
      periodExpensesData.push(this.formulateDirectExpensesPeriodForm(period, index));
    });

    return periodExpensesData;
  }

  private formulateDirectExpensesPeriodForm(period: IDirectExpensesPeriod, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      directExpensePeriodDetailId: [period.directExpensePeriodDetailId],
      directExpenseId: [period.directExpenseId],
      billingPeriodId: [period.billingPeriodId],
      expense: [period.expense]
    });
    return dataForm;
  }

  private getPeriods(periods: ProjectPeriod[]) {
    if (this.directExpenseCols.length > 0) {
      this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
      // this.directExpenseCols = this.directExpenseCols.concat(this.periodCols);
      this.colSpanSize = this.periodCols.length + this.directExpenseCols.length + 1;

    }
  }

  private associateBillingPeriodId(periods: IDirectExpensesPeriod[]) {
    periods.forEach((period, index) => {
      period.billingPeriodId = this.expenseAndAssetsData.projectPeriodDTO[index].billingPeriodId;
    });
  }

  onDirectExpensesSelectedRowDelete() {
    this.directExpensesForm.markAsDirty();
    this.selectedDirectExpense.forEach((row, index) => {
      const uId = row.controls.uId;
      // to skip 1st and second row from deletion logic.
      if (uId.value > 1) {
        const isDeleted: FormControl = row.controls.isDeleted;
        const totalExpense: FormControl = row.controls.totalExpense;
        const periodFormArray: FormArray = row.controls.directExpensePeriodDTO;
        isDeleted.setValue(true);
        totalExpense.setValue(0);

        // set period value to zero - for form validation
        for (const group of periodFormArray.controls) {
          group.get('expense').setValue(0);
        }
        // should also the sync collection with the same state as that of form
        this.expenseAndAssetsData.directExpenseDTO[index].isDeleted = true;

        this.selectedDirectExpense = [];
      }
    });
    this.displayMonthlyTotals();
    this.onCheckboxSelect();
  }

  onDirectExpensesAddRow() {
    this.directExpensesForm.markAsDirty();
    const expense = this.directExpensesService.
      addDirectExpenseRow(this.pipSheetId, this.expenseAndAssetsData.projectPeriodDTO);

    const uId = this.expenseAndAssetsData.directExpenseDTO.length + 1;
    this.directExpensesForm.push(this.directExpensesData(expense, uId));

    // sync the collection with the form
    this.expenseAndAssetsData.directExpenseDTO.push(expense);
  }

  onDirectExpensesCopySelected() {
    this.directExpensesForm.markAsDirty();
    let uId = this.expenseAndAssetsData.directExpenseDTO.length + 1;
    const composedExpenseData: IDirectExpense[] = [];
    let expenseData: IDirectExpense;
    let formGroup: FormGroup;
    this.selectedDirectExpense.forEach(expense => {
      if (expense.value.uId > 1) {
        expenseData = JSON.parse(JSON.stringify(expense.value));
        composedExpenseData.push(expenseData);
      }
    });
    // sort selected items by uId
    composedExpenseData.sort(this.utilityService.compareValues('uId', 'asc'));
    // update ID's and form
    composedExpenseData.forEach(composedExpense => {
      composedExpense.directExpenseId = 0;
      composedExpense.directExpensePeriodDTO.forEach(period => {
        period.directExpensePeriodDetailId = 0;
        period.directExpenseId = 0;
      });
      formGroup = this.directExpensesData(composedExpense, uId);
      formGroup.markAsDirty();
      this.directExpensesForm.push(formGroup);
      // sync the collection with the form
      this.expenseAndAssetsData.directExpenseDTO.push(composedExpense);
      uId = uId + 1;
    });
    this.displayMonthlyTotals();
    this.selectedDirectExpense = [];
    this.onCheckboxSelect();
  }

  onAdditionalAssetAddRow() {
    this.assetsForm.markAsDirty();
    const asset = this.directExpensesService.
      addAdditionalAssetRow(this.pipSheetId);
    const uId = this.expenseAndAssetsData.assetDTO.length + 1;

    this.assetsForm.push(this.assetsData(asset, uId));

    // sync the collection with the form
    this.expenseAndAssetsData.assetDTO.push(asset);
  }

  onAdditionalAssetsSelectedRowDelete() {
    this.assetsForm.markAsDirty();
    this.selectedAdditionalAssets.forEach((row) => {
      if (row.value.basicAssetId === null) {
        const index = this.assetsForm.controls.findIndex(val => val.value.uId === row.value.uId);
        this.assetsForm.removeAt(index);

        // should also the sync collection with the same state as that of form
        this.expenseAndAssetsData.assetDTO.splice(index, 1);
      }
    });
    this.selectedAdditionalAssets = [];
    this.displayTotalAssetCost();
    this.onCheckboxSelect();
  }

  onAdditionalAssetsCopySelected() {
    this.assetsForm.markAsDirty();
    const copiedAssetData: IAsset[] = [];
    let asset: IAsset;
    let formGroup: FormGroup;
    let uId = 0;
    this.selectedAdditionalAssets.forEach(copiedAsset => {


      // this code needs  refactoring..
      if (copiedAsset.value.basicAssetId === null) {
        if (this.expenseAndAssetsData.assetDTO[this.expenseAndAssetsData.assetDTO.length - 1].hasOwnProperty('uId')) {
          uId = this.expenseAndAssetsData.assetDTO[this.expenseAndAssetsData.assetDTO.length - 1].uId + 1;
        } else {
          uId = this.expenseAndAssetsData.assetDTO.length + 1;
        }
        asset = JSON.parse(JSON.stringify(copiedAsset.value));
        copiedAssetData.push(asset);
      }
    });
    // sort by uid ascending order
    copiedAssetData.sort(this.utilityService.compareValues('uId', 'asc'));
    copiedAssetData.forEach(sortedAsset => {
      sortedAsset.projectAssetId = 0;
      formGroup = this.assetsData(sortedAsset, uId);
      formGroup.markAsDirty();
      this.assetsForm.push(formGroup);
      this.expenseAndAssetsData.assetDTO.push(formGroup.value);
      uId = uId + 1;
    });
    this.displayTotalAssetCost();
    this.selectedAdditionalAssets = [];
    this.onCheckboxSelect();
  }

  onSave(formData) {
    this.isSaveClicked = true;
    this.directExpensesService.saveDirectExpenses(formData).subscribe(success => {
      this.directExpensesService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
        this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
      });
      // Set selectedAdditionalAssets to empty
      this.selectedAdditionalAssets = [];
      this.selectedDirectExpense = [];
      this.getAssetsAndDirectExpensesData(); // recalled get method :  ideally it should bind the Id's
      this.translateService.get('SuccessMessage.ExpensesAndAssets').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      this.form.markAsPristine();
      this.notificationService.notifyFormDirty(false);
      this.getOverrideNotificationStatus();
    }, () => {
      this.translateService.get('ErrorMessage.ExpensesAndAssets').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
      this.isSaveClicked = false;
    });
  }

  onAssetChange(rowIndex: number) {
    const asset = this.asset[rowIndex] as FormGroup;
    asset.controls.amount.setValue(this.directExpensesService.calculateAssetAmount(asset.value));
  }

  displayTotalAssetCost() {
    this.totalAssetCost = this.directExpensesService.calculateTotalAssetCost(this.assetsForm.value);
  }

  onExpenseChange(rowIndex: number) {
    const directExpense = this.directExpense[rowIndex] as FormGroup;
    directExpense.controls.totalExpense.setValue(
      this.directExpensesService.calculateTotalExpense(directExpense.value));
  }

  displayMonthlyAssetCost() {
    const perMonthCost = this.directExpensesService
      .calculatePerMonthAssetCost(this.directExpensesForm.value, this.totalAssetCost, this.assetMaxDayCharge, this.currencyFactor);

    perMonthCost.forEach((perMonth, index) => {
      this.monthlyAssetCost[index] = perMonth;
    });

    this.displayMonthlyTotals();
  }

  displayMonthlyTotals() {
    const directDTO = [];
    Object.assign(directDTO, this.directExpensesForm.value);
    const perMonthAssetAndExpense = this.directExpensesService
      .calculatePerMonthAssetAndExpense(directDTO, this.monthlyAssetCost);

    perMonthAssetAndExpense.forEach((perMonth, index) => {
      this.monthlyTotals[index] = perMonth;
    });

    this.totalDirectExpenseCost = this.directExpensesService
      .calculateTotalAssetAndExpense(this.monthlyTotals);
  }

  onMilestoneSelected(index: number, selectedValue) {
    const milestone = this.directExpense[index] as FormGroup;
    milestone.controls.milestoneId.setValue(selectedValue.id);
  }

  // validator: ideally should be in a separate file

  descriptionValidator(assetForm: FormGroup) {
    if (assetForm.dirty) {
      const amount = assetForm.get('amount');
      const description = assetForm.get('description');
      if (amount.value > 0 && description.value === '') {
        description.setErrors({ isDescriptionEmpty: true });
      } else {
        description.setErrors(null);
      }
    }
  }

  descriptionDirectValidator(group: FormGroup) {
    if (group.dirty) {
      const desc = group.get('description');
      if (desc.value === '' && (+group.value.totalExpense !== 0 || +group.value.percentRevenue > 0)) {
        desc.setErrors({ isDescriptionEmpty: true });
      } else {
        desc.setErrors(null);
      }
    }
  }

  onCellClick(isContextEvent: boolean, rowData?, event?, rowIndex?: number) {
    if (!isContextEvent) {
      this.currentCellIndex = +(<string>event.target.id).split('a')[1];
      this.isCellActive = (this.currentCellIndex >= 0) ? true : false;
      this.currentRowIndex = rowIndex;
      this.currentRowData = rowData;
    } else {
      const periods = (<FormArray>this.directExpense[this.currentRowIndex]).controls['directExpensePeriodDTO'];
      this.currentCellValue = +<FormGroup>periods.value[this.currentCellIndex].expense;

      periods.controls.forEach((expenseControl, index) => {
        if (index > this.currentCellIndex) {
          (<FormGroup>expenseControl).controls['expense'].setValue(this.currentCellValue);
        }
      });

      this.onExpenseChange(this.currentRowIndex);
      this.displayMonthlyTotals();
      this.isCellActive = false;
      this.scrollToEnd(this.currentRowIndex, (periods.length - 1).toString());
    }
  }

  scrollToEnd(rowIndex: number, length: string) {
    const index = rowIndex.toString() + 'a' + length;
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

  onPercentRevenueChange(rowIndex: number, percentRevenue: string) {

    const directExpensePeriodData = this.directExpense[rowIndex].get('directExpensePeriodDTO');
    if (+percentRevenue > 0) {
      const percentValue = +percentRevenue / 100;

      directExpensePeriodData.value.forEach((period, periodIndex) => {
        const periodValue = this.expenseAndAssetsData.periodLaborRevenueDTO[periodIndex].revenue * percentValue;
        directExpensePeriodData['controls'][periodIndex].get('expense').setValue(+ periodValue.toFixed(2));
      });
      this.displayMonthlyTotals();
      this.onExpenseChange(rowIndex);
    }
    else {
      if (this.expenseAndAssetsData.directExpenseDTO[rowIndex].percentRevenue > 0) {
        directExpensePeriodData.value.forEach((period, periodIndex) => {
          directExpensePeriodData['controls'][periodIndex].get('expense').setValue(0);
        });
      }
      else {
        const getDirectExpensePeriodRowData = this.expenseAndAssetsData.directExpenseDTO[rowIndex].directExpensePeriodDTO;
        directExpensePeriodData.value.forEach((period, periodIndex) => {
          directExpensePeriodData['controls'][periodIndex].get('expense').setValue(getDirectExpensePeriodRowData[periodIndex].expense);
        });
      }
      this.displayMonthlyTotals();
      this.onExpenseChange(rowIndex);
    }
  }

  onCheckboxSelect() {
    this.selExpenseRow = this.selectedDirectExpense.filter(x => x.value.isDeleted === false);
    this.selAssetsRow = this.selectedAdditionalAssets.filter(x => x.value.isDeleted === false);
  }

  onDirectExpenseRowUnselect() {
    if (this.selectedDirectExpense.find(x => x.value.uId === 0 || x.value.uId === 1)) {
      this.selectedDirectExpense.splice(0, 2);
    }
  }

  onAdditionalAssetRowUnselect() {
    if (this.selectedAdditionalAssets.find(x => x.value.uId > 6)) {
      this.selectedAdditionalAssets.splice(0, 7);
    }
  }
}
