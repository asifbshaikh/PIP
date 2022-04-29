import { Milestone } from './../../../../shared-module/infrastructure/configuration-settings';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { SelectItem, MessageService, MenuItem } from 'primeng/api';
import { FormGroup, FormBuilder, FormArray, FormControl, Validators, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute } from '@angular/router';
import { SharedDataService, NotificationService } from '@global';
import { DateService, UtilityService } from '@core';
import { ReimbursementSalesService } from '@shared/services/reimbursement-sales.service';
import { IReimbursementAndSales } from '@shared/domain/IReimbursementAndSales';
import { ProjectPeriod, ResourceMapper, Mastermapper, Constants } from '@shared';
import { IReimbursement } from '@shared/domain/IReimbursement';
import { IReimbursementPeriod } from '@shared/domain/IReimbursementPeriod';
import { ISalesDiscount } from '@shared/domain/ISalesDiscount';
import { ISalesDiscountPeriod } from '@shared/domain/ISalesDiscountPeriod';
import { pairwise, startWith } from 'rxjs/operators';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'reimbursement-and-sales',
  templateUrl: './reimbursement-and-sales.component.html'
})
export class ReimbursementAndSalesComponent implements OnInit {
  pipSheetId: number;
  projectId: number;
  dashboardId: number;
  selectedMilestones: SelectItem[];
  periodCols: any[] = [];
  form: FormGroup;
  reimbursementCols: any[] = [];
  salesDiscountCols: any[] = [];
  selectedReimbursement: any[] = [];
  selectedSalesDiscount: any[] = [];

  reimbursementAndDiscountData: IReimbursementAndSales;
  totalReimburesment = 0;
  totalDiscount = 0;
  reimbursedPeriodTotals = [];
  salesDiscountPeriodTotals = [];
  reimbursColSpanSize = 0;
  colSpanSize = 0;
  reimburmentContextMenuItems: MenuItem[];
  salesContextMenuItems: MenuItem[];
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
  isSaveClicked = false;
  pPercentOfDirectExpenseShowToolTip: string;
  selReimbRow: any[] = [];
  selSalesRow: any[] = [];

  constructor(
    private translateService: TranslateService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private sharedDataService: SharedDataService,
    private dateService: DateService,
    private messageService: MessageService,
    private reimbursementSalesService: ReimbursementSalesService,
    private cdRef: ChangeDetectorRef,
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
    this.reimburmentContextMenuItems = [{
      label: 'REPEAT >>',
      command: (event: any) => {
        this.onRreimbursementCellClick(true);
      }
    }];
    this.salesContextMenuItems = [{
      label: 'REPEAT >>',
      command: (event: any) => {
        this.onSalesCellClick(true);
      }
    }];
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.initialize();
    this.getReimbursementAndSalesDiscountData();
  }

  initialize() {
    // default data
    this.reimbursementAndDiscountData = {
      projectMilestones: [],
      projectPeriods: [],
      reimbursements: [],
      salesDiscounts: []
    };

    // form
    this.form = this.fb.group({
      reimbursements: this.fb.array([]),
      salesDiscounts: this.fb.array([])
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

  getColumns(path: string) {
    return this.translateService.get(path);
  }

  get reimbursementForm() {
    return this.form.get('reimbursements') as FormArray;
  }

  get salesDiscountForm() {
    return this.form.get('salesDiscounts') as FormArray;
  }

  get reimbursement() {
    return this.reimbursementForm.controls;
  }

  get salesDiscount() {
    return this.salesDiscountForm.controls;
  }

  get numberOfReimbursementRows() {
    return this.reimbursementForm.value.filter(reimb => reimb.isDeleted === false).length;
  }

  get numberOfSalesRows() {
    return this.salesDiscountForm.value.filter(sales => sales.isDeleted === false).length;
  }
  getReimbursementAndSalesDiscountData() {
    this.reimbursementAndDiscountData = null;
    this.initialize();
    this.reimbursedPeriodTotals = [];
    this.salesDiscountPeriodTotals = [];
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.reimbursementSalesService
        .getReimbursementAndSalesDetails(this.pipSheetId).subscribe(data => {

          //  Reimbursement columns
          this.getColumns('ReimbursementAndSales.ReimbursementColumns').subscribe(cols => {
            this.reimbursementCols = cols;
          });

          //  Sales and Discount columns
          this.getColumns('ReimbursementAndSales.SalesDiscountColumns').subscribe(cols => {
            this.salesDiscountCols = cols;
          });

          this.getPeriods(data.projectPeriods);

          // this.reimbursementCols = this.reimbursementCols.concat(this.periodCols);
          // this.salesDiscountCols = this.salesDiscountCols.concat(this.periodCols);
          this.reimbursColSpanSize = this.reimbursementCols.length;

          this.reimbursementAndDiscountData.projectMilestones = data.projectMilestones;
          this.reimbursementAndDiscountData.projectPeriods = data.projectPeriods;

          // reimbursement
          if (data.reimbursements.length > 0) {
            const length = data.reimbursements.length;
            if (length === 1) {
              this.reimbursementAndDiscountData.reimbursements = this.reimbursementSalesService
                .getDefaultReimbursements(data.reimbursements, data.projectPeriods, this.pipSheetId, 1);
            }
            else {
              this.reimbursementAndDiscountData.reimbursements = data.reimbursements;
            }
          } else {
            this.reimbursementAndDiscountData.reimbursements = this.reimbursementSalesService
              .getDefaultReimbursements(data.reimbursements, data.projectPeriods, this.pipSheetId, 2);
          }

          // sales discount
          if (data.salesDiscounts.length > 0) {
            // already saved data against pipsheet
            this.reimbursementAndDiscountData.salesDiscounts = data.salesDiscounts;
          }
          else {
            this.reimbursementAndDiscountData.salesDiscounts = this.reimbursementSalesService
              .getDefaultSalesDiscount(data.salesDiscounts, data.projectPeriods, this.pipSheetId);
          }

          this.colSpanSize = this.reimbursementCols.length + this.periodCols.length + 1;
          this.selectedMilestones = new Mastermapper().getOptionalPhaseComboItems(data.projectMilestones, false);
          this.bindFormData();
          this.onExpenseChange();
          this.onDiscountChange();
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

  private getPeriods(periods: ProjectPeriod[]) {
    this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
  }

  bindFormData() {

    //  reimbursement form
    this.reimbursementAndDiscountData.reimbursements.forEach((reimbursement, index) => {
      this.reimbursementForm.push(this.reimbursementData(reimbursement, index));
    });

    // this.reimbursementForm.push(this.composeTotalReimbursementForm());

    // Sales and Discount Form
    this.reimbursementAndDiscountData.salesDiscounts.forEach((discount, index) => {
      this.salesDiscountForm.push(this.salesDiscountData(discount, index));
    });

    //  need to handle here sales discount total data
  }

  private getSelectedMilestoneItem(milestoneId: number) {
    let milestone = this.selectedMilestones.find(item => item.value.id === milestoneId);
    if (!milestone) {
      milestone = Constants.selectComboItem;
    }
    return milestone;
  }

  private reimbursementData(reimbursement: IReimbursement, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      reimbursementId: [reimbursement.reimbursementId],
      pipSheetId: [reimbursement.pipSheetId],
      milestoneId: [reimbursement.milestoneId],
      selectedMilestoneItem: this.getSelectedMilestoneItem(reimbursement.milestoneId).value,
      description: [reimbursement.description],
      reimbursedExpense: [reimbursement.reimbursedExpense],
      isDeleted: [reimbursement.isDeleted],
      isDirectExpenseReimbursable: [reimbursement.isDirectExpenseReimbursable],
      directExpensePercent: [reimbursement.isDirectExpenseReimbursable &&
        (reimbursement.directExpensePercent === 100 || reimbursement.directExpensePercent == null) ? 100 :
        ((reimbursement.isDirectExpenseReimbursable && reimbursement.directExpensePercent !== 100)
          ? reimbursement.directExpensePercent : ''), [this.directExpensePercentValidate.bind(this)]],
      isDirectExpenseMilestone: [reimbursement.isDirectExpenseMilestone],
      createdBy: [1],
      updatedBy: [1],
      reimbursementPeriods: this.reimbursementPeriodData(reimbursement.reimbursementPeriods, index)
    },
      { validator: this.onReimbursementdescriptionValidate }
    );
    return dataForm;
  }

  public directExpensePercentValidate(control: AbstractControl): { [key: string]: any } | null {
    if (control.dirty) {
      const valid = (control.value === '') || (control.value === null) || (+control.value === 0) ? false : true;
      !valid ? this.isSaveClicked = true : this.isSaveClicked = false;
      return valid
        ? null
        : { invalidNumber: { valid: false, value: control.value } };
    }
  }

  private reimbursementPeriodData(periods: IReimbursementPeriod[], index: number): FormArray {
    const periodExpensesData = this.fb.array([]);
    this.associateBillingPeriodId(periods);
    periods.forEach(period => {
      periodExpensesData.push(this.formulateReimbursementPeriodForm(period, index));
    });

    return periodExpensesData;
  }

  private associateBillingPeriodId(periods: IReimbursementPeriod[]) {
    periods.forEach((period, index) => {
      period.billingPeriodId = this.reimbursementAndDiscountData.projectPeriods[index].billingPeriodId;
    });
  }

  private formulateReimbursementPeriodForm(period: IReimbursementPeriod, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      reimbursementId: [period.reimbursementId],
      billingPeriodId: [period.billingPeriodId],
      expense: [period.expense]
    });
    return dataForm;
  }

  onReimbursementMilestoneSelected(index: number, selectedValue) {
    const milestone = this.reimbursement[index] as FormGroup;
    milestone.controls.milestoneId.setValue(selectedValue.id);
  }

  onReimbursementAddRow() {
    this.reimbursementForm.markAsDirty();
    const expense = this.reimbursementSalesService.addReimbursementrow(
      this.pipSheetId, this.reimbursementAndDiscountData.projectPeriods);

    const uId = this.reimbursementAndDiscountData.reimbursements.length + 1;
    const control = this.reimbursementData(expense, uId);
    this.reimbursementForm.push(control);
    // sync the collection with the form
    this.reimbursementAndDiscountData.reimbursements.push(expense);
  }

  onReimbursementSelectedRowDelete() {
    this.reimbursementForm.markAsDirty();
    this.selectedReimbursement.forEach((row, index) => {
      if (!row.value.isDirectExpenseReimbursable) {
        const isDeleted: FormControl = row.controls.isDeleted;
        const periods: FormArray = row.controls.reimbursementPeriods;
        isDeleted.setValue(true);
        this.resetReimbursementPeriods(periods);
        // should also the sync collection with the same state as that of form
        this.reimbursementAndDiscountData.reimbursements[index].isDeleted = true;
      }
    });
    this.selectedReimbursement = [];
    this.onExpenseChange();
    this.onCheckboxSelect();
  }

  onReimbursementCopySelected() {
    this.reimbursementForm.markAsDirty();
    const copiedReimbursementData: IReimbursement[] = [];
    let formGroup: FormGroup;
    let uId = this.reimbursementAndDiscountData.reimbursements.length + 1;
    this.selectedReimbursement.forEach(row => {
      if (!row.value.isDirectExpenseReimbursable) {
        copiedReimbursementData.push(JSON.parse(JSON.stringify(row.value)));
      }
    });
    copiedReimbursementData.sort(this.utilityService.compareValues('uId', 'asc'));
    copiedReimbursementData.forEach(reimbursement => {
      reimbursement.reimbursementId = 0;
      reimbursement.reimbursementPeriods.forEach(period => {
        period.reimbursementId = 0;
      });
      formGroup = this.reimbursementData(reimbursement, uId);
      formGroup.markAsDirty();
      this.reimbursementForm.push(formGroup);
      this.reimbursementAndDiscountData.reimbursements.push(reimbursement);
      uId = uId + 1;
    });
    this.onExpenseChange();
    this.selectedReimbursement = [];
    this.onCheckboxSelect();
  }

  resetReimbursementPeriods(periods: FormArray) {
    periods.controls.forEach(period => {
      const expense = period.get('expense');
      expense.setValue(0);
    });
  }

  // on expense change

  onExpenseChange() {
    const expenseCalculatedTotals = this.reimbursementSalesService
      .calculateReimbursedExpense(this.reimbursementForm.value, this.reimbursementAndDiscountData.projectPeriods);

    this.reimbursementForm.controls.forEach((row, index) => {
      const reimbursedExpense = row.get('reimbursedExpense') as FormControl;
      reimbursedExpense.setValue(expenseCalculatedTotals.expenseTotals[index]);
    });
    this.reimbursedPeriodTotals = expenseCalculatedTotals.periodTotals;
    if (this.reimbursedPeriodTotals.length > 0) {
      this.totalReimburesment = this.reimbursedPeriodTotals.reduce((a, b) => a + b);
    }
  }

  onReimbursementdescriptionValidate(group: FormGroup) {
    if (group.dirty) {
      const desc = group.get('description');
      if (desc.value === '' && +group.value.reimbursedExpense !== 0) {
        desc.setErrors({ isDescriptionEmpty: true });
      } else {
        desc.setErrors(null);
      }
    }
  }

  // repeat period value logic
  getReimbursementControls(): AbstractControl[] {
    const expenses = this.form.controls.reimbursements as FormArray;
    const expenseGroup = expenses.controls;
    return expenseGroup;
  }

  onRreimbursementCellClick(isContextEvent: boolean, event?, rowData?, rowIndex?: number) {
    if (!isContextEvent) {
      this.currentCellIndex = +(<string>event.target.id).split('a')[1];
      this.currentRowIndex = rowIndex;
      this.currentRowData = rowData;
    } else {
      const periods: FormArray = (<FormArray>this.getReimbursementControls()[this.currentRowIndex]).controls['reimbursementPeriods'];
      this.currentCellValue = +<FormGroup>periods.controls[this.currentCellIndex].value.expense;
      periods.controls.forEach((expenseControl, index) => {
        if (index > this.currentCellIndex) {
          (<FormGroup>expenseControl).controls['expense'].setValue(this.currentCellValue);
        }
      });
      this.onExpenseChange();
      this.rScrollToEnd(this.currentRowIndex, (periods.length - 1).toString());
    }
  }

  rScrollToEnd(rowIndex: number, length: string) {
    const index = rowIndex.toString() + 'a' + length;
    document.getElementById(index).focus();
  }

  // SALES DISCOUNT SECTION

  private salesDiscountData(discount: ISalesDiscount, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      salesDiscountId: [discount.salesDiscountId],
      pipSheetId: [discount.pipSheetId],
      milestoneId: [discount.milestoneId],
      selectedMilestoneItem: this.getSelectedMilestoneItem(discount.milestoneId).value,
      description: [discount.description],
      discount: [discount.discount],
      isDeleted: [discount.isDeleted],
      createdBy: [1],
      updatedBy: [1],
      salesDiscountPeriods: this.discountPeriodData(discount.salesDiscountPeriods, index)
    },
      { validator: this.onSalesDiscountValidate });
    return dataForm;
  }

  private discountPeriodData(periods: ISalesDiscountPeriod[], index: number): FormArray {
    const periodDiscountData = this.fb.array([]);
    this.associateSalesDiscountBillingPeriodId(periods);
    periods.forEach(period => {
      periodDiscountData.push(this.formulateSalesDiscountPeriodForm(period, index));
    });

    return periodDiscountData;
  }

  private associateSalesDiscountBillingPeriodId(periods: ISalesDiscountPeriod[]) {
    periods.forEach((period, index) => {
      period.billingPeriodId = this.reimbursementAndDiscountData.projectPeriods[index].billingPeriodId;
    });
  }

  private formulateSalesDiscountPeriodForm(period: ISalesDiscountPeriod, index: number): FormGroup {
    const dataForm = this.fb.group({
      uId: [index],
      salesDiscountId: [period.salesDiscountId],
      billingPeriodId: [period.billingPeriodId],
      discount: [period.discount]
    });
    return dataForm;
  }

  // To be rectified
  private composeTotalSalesDiscountForm(): FormGroup {

    const totalFormGroup = this.fb.group({
      totalReimbursment: 0
    });

    this.reimbursementAndDiscountData.projectPeriods.forEach((period, index) => {
      totalFormGroup.addControl(index.toString(), new FormControl(0));
    });

    return totalFormGroup;
  }

  onSalesDiscountMilestoneSelected(index: number, selectedValue) {
    const milestone = this.salesDiscount[index] as FormGroup;
    milestone.controls.milestoneId.setValue(selectedValue.id);
  }

  onSalesDiscountAddRow() {
    this.salesDiscountForm.markAsDirty();
    // const data = this.reimbursementSalesService.addSalesDiscountRow(this.pipSheetId,
    //   this.reimbursementAndDiscountData.projectPeriods);

    const data = this.reimbursementSalesService.addSalesDiscountRow(
      this.pipSheetId, this.reimbursementAndDiscountData.projectPeriods);

    const uId = this.reimbursementAndDiscountData.salesDiscounts.length + 1;
    const control = this.salesDiscountData(data, uId);
    this.salesDiscountForm.push(control);
    // sync the collection with the form
    this.reimbursementAndDiscountData.salesDiscounts.push(data);
  }

  onSalesDiscountSelectedRowDelete() {
    this.salesDiscountForm.markAsDirty();
    this.selectedSalesDiscount.forEach((row, index) => {
      const isDeleted: FormControl = row.controls.isDeleted;
      const periods: FormArray = row.controls.salesDiscountPeriods;
      isDeleted.setValue(true);
      this.resetSalesDiscountPeriods(periods);

      // should also the sync collection with the same state as that of form
      this.reimbursementAndDiscountData.salesDiscounts[index].isDeleted = true;
    });
    this.selectedSalesDiscount = [];
    this.onDiscountChange();
    this.onCheckboxSelect();
  }

  onSalesDiscountCopySelected() {
    this.salesDiscountForm.markAsDirty();
    const copiedSalesData: ISalesDiscount[] = [];
    let formGroup: FormGroup;
    let uId = this.reimbursementAndDiscountData.salesDiscounts.length + 1;
    this.selectedSalesDiscount.forEach(row => {
      copiedSalesData.push(JSON.parse(JSON.stringify(row.value)));
    });
    copiedSalesData.sort(this.utilityService.compareValues('uId', 'asc'));
    copiedSalesData.forEach(sales => {
      sales.salesDiscountId = 0;
      sales.salesDiscountPeriods.forEach(period => {
        period.salesDiscountId = 0;
      });
      formGroup = this.salesDiscountData(sales, uId);
      formGroup.markAsDirty();
      this.salesDiscountForm.push(formGroup);
      this.reimbursementAndDiscountData.salesDiscounts.push(sales);
      uId = uId + 1;
    });
    this.onDiscountChange();
    this.selectedSalesDiscount = [];
    this.onCheckboxSelect();
  }

  private resetSalesDiscountPeriods(periods: FormArray) {
    periods.controls.forEach(period => {
      const expense = period.get('discount');
      expense.setValue(0);
    });
  }

  onCheckboxSelect() {
    this.selReimbRow = this.selectedReimbursement.filter(x => x.value.isDeleted === false && x.value.isDirectExpenseReimbursable === false);
    this.selSalesRow = this.selectedSalesDiscount.filter(x => x.value.isDeleted === false);
  }

  onSave(formData) {
    this.isSaveClicked = true;
    this.reimbursementSalesService.saveReimbursementAndSalesDetails(formData).subscribe(success => {
      this.reimbursementSalesService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
        this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
        this.getOverrideNotificationStatus();
      });
      // empty selected arrays
      this.selectedReimbursement = [];
      this.selectedSalesDiscount = [];
      this.getReimbursementAndSalesDiscountData(); // recalled get method :  ideally it should bind the Id's
      this.translateService.get('SuccessMessage.ReimbursementAndSales').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      this.form.markAsPristine();
      this.notificationService.notifyFormDirty(false);
    }, () => {
      this.translateService.get('ErrorMessage.ReimbursementAndSales').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
      this.isSaveClicked = false;
    });
  }

  onSalesDiscountValidate(group: FormGroup) {
    if (group.dirty) {
      const desc = group.get('description');
      if (desc.value === '' && +group.value.discount > 0) {

        desc.setErrors({ isDescriptionEmpty: true });
      } else {
        desc.setErrors(null);
      }
    }
  }

  // Sales and discount calculation :
  onDiscountChange() {
    const calculatedDiscountTotals = this.reimbursementSalesService
      .calculateDiscount(this.salesDiscountForm.value, this.reimbursementAndDiscountData.projectPeriods);

    this.salesDiscountForm.controls.forEach((row, index) => {
      const discount = row.get('discount') as FormControl;
      discount.setValue(calculatedDiscountTotals.discountTotals[index]);
    });
    this.salesDiscountPeriodTotals = calculatedDiscountTotals.periodTotals;
    if (this.salesDiscountPeriodTotals.length > 0) {
      this.totalDiscount = this.salesDiscountPeriodTotals.reduce((a, b) => a + b);
    }
  }

  // repeat period value logic
  getSalesControls(): AbstractControl[] {
    const sales = this.form.controls.salesDiscounts as FormArray;
    const salesGroup = sales.controls;
    return salesGroup;
  }

  onSalesCellClick(isContextEvent: boolean, event?, rowData?, rowIndex?: number) {
    if (!isContextEvent) {
      this.currentCellIndex = +(<string>event.target.id).split('b')[1];
      this.currentRowIndex = rowIndex;
      this.currentRowData = rowData;
    } else {
      const periods: FormArray = (<FormArray>this.getSalesControls()[this.currentRowIndex]).controls['salesDiscountPeriods'];
      this.currentCellValue = +<FormGroup>periods.controls[this.currentCellIndex].value.discount;
      periods.controls.forEach((salesControl, index) => {
        if (index > this.currentCellIndex) {
          (<FormGroup>salesControl).controls['discount'].setValue(this.currentCellValue);
        }
      });
      this.onDiscountChange();
      this.scrollToEnd(this.currentRowIndex, (periods.length - 1).toString());
    }
  }

  scrollToEnd(rowIndex: number, length: string) {
    const index = rowIndex.toString() + 'b' + length;
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

  onPercentDirectExpenseChange(rowIndex: number, directExpensePercent: string) {
    const reimbursementPeriodData = this.reimbursement[rowIndex].get('reimbursementPeriods');

    const percentValue = +directExpensePercent / 100;

    reimbursementPeriodData.value.forEach((period, periodIndex) => {
      const periodValue = (this.reimbursementAndDiscountData.reimbursements[rowIndex].reimbursementPeriods[periodIndex].expense
        / (this.reimbursementAndDiscountData.reimbursements[rowIndex].directExpensePercent / 100))
        * percentValue;
      reimbursementPeriodData['controls'][periodIndex].get('expense').setValue(+ periodValue.toFixed(2));
    });

    this.onExpenseChange();
  }

  getOverrideNotificationErrorMsg(rowIndex) {
    const reimbursementControls = this.reimbursement[rowIndex];
    if (reimbursementControls.value.directExpensePercent === '' || reimbursementControls.value.directExpensePercent === null ||
      reimbursementControls.value.directExpensePercent === '0') {
      this.translateService.get('ReimbursementAndSales.PercentOfDirectExpense').subscribe(msg => {
        this.pPercentOfDirectExpenseShowToolTip = msg;
      });
    } else {
      this.pPercentOfDirectExpenseShowToolTip = '';
    }
  }
}



