import { OverrideNotificationStatus } from './../../../../shared-module/domain/override-notification-status';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { SharedDataService } from '@global';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { IRoleAndAccount } from './../../../../shared-module/domain/IRoleAndAccount';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { Component, OnInit, AfterViewInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Constants } from '@shared';
import { ActivatedRoute } from '@angular/router';
import { VacationAbsenceService } from '@shared/services/vacation-absence.service';
import { ResourceMapper } from '@shared/mapper/master/resourcemapper';
import { DateService } from '@core/services/date.service';
import { IVacationAbsence, IPeriodLostRevenue } from '@shared';
import { MessageService, MenuItem } from 'primeng/api';
import { NotificationService } from '@global';

@Component({
  selector: 'app-vacation-absenses',
  templateUrl: './vacation-absenses.component.html'
})
export class VacationAbsensesComponent implements OnInit, AfterViewInit {
  pipSheetId: number;
  projectId: number;
  dashboardId: number;
  vacationAbsensesForm: FormGroup;
  periodCols: any[] = [];
  vacationAbsensesCols: any[] = [];
  vacationAbsensesTooltip: any;
  translationData: any;
  isMarginSet: Boolean = false;
  totalRevenue: number;
  overrideDifference: number;
  message = { type: '', text: '' };
  isValid: Boolean = true;
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
  loggedInUserId: number;
  isDataAvailable = false;
  isSaveClicked = false;

  constructor(
    private translateService: TranslateService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private vacationAbsenceService: VacationAbsenceService,
    private dateService: DateService,
    private messageService: MessageService,
    private sharedDataService: SharedDataService,
    private notificationService: NotificationService,
    private userWorkflowService: UserWorkflowService
  ) {
  }

  ngOnInit() {
    this.contextMenuItems = [{
      label: 'REPEAT >>',
      command: (event: any) => {
        this.onCellClick(true);
      }
    }];
    this.initializeForm();

    this.translateService.get('VacationAbsences').subscribe(data => {
      this.translationData = data;
      this.vacationAbsensesTooltip = this.translationData.ColumnHeaderPopUpMessages;
    });

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });

    this.route.paramMap.subscribe(
      params => {
        this.pipSheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
        this.projectId = parseInt(params.get(Constants.uiRoutes.routeParams.projectId), 10);
        this.dashboardId = parseInt(params.get(Constants.uiRoutes.routeParams.dashboardId), 10);
      });
    this.getVacationAbsence();
    this.setInitialMessage();
  }

  initializeForm() {
    this.vacationAbsensesForm = this.fb.group({
      percent: ['', [Validators.pattern(Constants.regExType.percentageWithDecimalPrecisionTwo), Validators.max(100)]],
      amount: [''],
      lostRevenue: [''],
      isOverride: [''],
      isPercent: [false, []],
      totalLostRevenue: [''],
      periodsLostRevenueList: this.fb.array([])
    });
  }

  ngAfterViewInit() {
    this.vacationAbsensesForm.valueChanges.subscribe(() => {
      if (this.vacationAbsensesForm.dirty) {
        this.notificationService.notifyFormDirty(true);
      }
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
      setTimeout(() => {
        this.vacationAbsensesForm.disable();
      }, 200);
    }
  }

  getVacationAbsence() {
    let lostRevenue = 0;
    this.initializeForm();
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.vacationAbsenceService.getVacationAbsence(this.pipSheetId).subscribe(data => {
        if (data) {
          this.isMarginSet = data.isMarginSet;
          this.vacationAbsensesForm.get('isPercent').setValue(data.isPercent);
          this.totalRevenue = data.totalRevenue;
          this.vacationAbsensesForm.get('isOverride').setValue(data.isOverride);
          if (this.vacationAbsensesForm.value.isPercent && data.totalRevenue !== null) {
            this.vacationAbsensesForm.get('percent')
              .setValue(+ data.amount);
            this.vacationAbsensesForm.get('amount').setValue(null);
            lostRevenue = +data.totalRevenue * (+data.amount) / 100;
          } else {
            this.vacationAbsensesForm.get('amount').setValue(data.amount);
            this.vacationAbsensesForm.get('percent').setValue(null);
            lostRevenue = +data.amount;
          }
          this.vacationAbsensesForm.get('lostRevenue').setValue(lostRevenue);
          this.vacationAbsensesForm.get('totalLostRevenue').setValue(lostRevenue);
          if (this.totalRevenue && this.totalRevenue < 0) {
            this.vacationAbsensesForm.get('isOverride').setValue(false);
            this.vacationAbsensesForm.get('isOverride').disable();
          }

          if (data.periodLostRevenue && data.periodLostRevenue.length > 0) {
            // generate period columns
            const periodsRevenueArray = this.vacationAbsensesForm.get('periodsLostRevenueList') as FormArray;
            this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(data.periodLostRevenue);
            for (const period of data.periodLostRevenue) {
              periodsRevenueArray.push(this.fb.group({
                billingPeriodId: [period.billingPeriodId, []],
                projectPeriodId: [period.projectPeriodId, []],
                revenue: [period.revenue, []],
                lostRevenue: [period.lostRevenue, []]
              }));
            }
          }
          // set validator for amount - should be max as totalRevenue
          // this.vacationAbsensesForm.get('amount').setValidators(Validators.max(this.totalRevenue));
          this.onPeriodValueChange();
          this.periodWiseLostCalculation(true);
          this.enableDisablePercentAmount();
        } else {
          this.isMarginSet = false;
        }
        this.pipSheetWorkflowStatus = this.sharedDataService.sharedData.pipSheetWorkflowStatus;
        this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
        this.isDataAvailable = true;
        this.enableDisableForm();
        this.isSaveClicked = false;
      });
    }
    else {
      this.isDataAvailable = true;
    }
  }

  onPercentChange() {
    this.enableDisablePercentAmount();
    if (this.vacationAbsensesForm.value.percent) {
      this.vacationAbsensesForm.get('isPercent').setValue(true);
      this.vacationAbsensesForm.get('lostRevenue')
        .setValue(((this.totalRevenue * this.vacationAbsensesForm.value.percent) / 100));
      this.periodWiseLostCalculation(false);
      if (this.vacationAbsensesForm.value.isOverride) {
        this.onPeriodValueChange();
      }
    } else {
      this.vacationAbsensesForm.get('isPercent').setValue(false);
      this.vacationAbsensesForm.get('lostRevenue').setValue(0);
      this.setDefaultValues();
    }
    this.disableOverrideOption();
  }

  onAmountChange() {
    this.enableDisablePercentAmount();
    if (this.vacationAbsensesForm.value.amount) {
      this.vacationAbsensesForm.get('lostRevenue').setValue(this.vacationAbsensesForm.value.amount);
      this.vacationAbsensesForm.get('isPercent').setValue(false);
      this.periodWiseLostCalculation(false);

      if (this.vacationAbsensesForm.value.isOverride) {
        this.onPeriodValueChange();
      }
    } else {
      this.setDefaultValues();
    }
    this.disableOverrideOption();
  }

  enableDisablePercentAmount() {
    if (this.vacationAbsensesForm.value.amount) {
      this.vacationAbsensesForm.get('percent').disable();
      this.vacationAbsensesForm.get('amount').enable();
    } else {
      this.vacationAbsensesForm.get('percent').enable();
    }

    if (this.vacationAbsensesForm.value.percent) {
      this.vacationAbsensesForm.get('amount').disable();
      this.vacationAbsensesForm.get('percent').enable();
    } else {
      this.vacationAbsensesForm.get('amount').enable();
    }

    if (!this.vacationAbsensesForm.value.amount && !this.vacationAbsensesForm.value.percent) {
      this.vacationAbsensesForm.get('percent').enable();
      this.vacationAbsensesForm.get('amount').enable();
    }
  }
  onOverrideChange() {
    if (this.vacationAbsensesForm.value.isOverride) {
      this.setDefaultPeriodValues();
      this.onPeriodValueChange();
    } else {
      this.periodWiseLostCalculation(false);
    }
  }

  // on any period value change - calculate total lost revenue
  onPeriodValueChange() {
    const periodTotal = this.vacationAbsenceService
      .calculateTotalLostRevenue(this.getPeriodRevenueFormArray().value, this.vacationAbsensesForm.value.lostRevenue
        , this.vacationAbsensesForm.value.isOverride);
    this.overrideDifference = periodTotal.overrideDifference;
    this.vacationAbsensesForm.get('totalLostRevenue').setValue(periodTotal.totalLostRevenue);
    this.runValidations();
  }

  periodWiseLostCalculation(initLoad: boolean) {
    const lostRevenue = this.vacationAbsensesForm.value.lostRevenue;
    const revenueArray = this.getPeriodRevenueFormArray();

    if ((this.vacationAbsensesForm.value.isOverride === false || this.vacationAbsensesForm.get('isOverride').status === 'DISABLED')
      && this.vacationAbsensesForm.value.lostRevenue) {
      const periodWiseLostRevenueData = this.vacationAbsenceService
        .calculatePeriodWiseLostRevenue(revenueArray.value, this.totalRevenue, lostRevenue);
      revenueArray.controls.forEach((revenueControl, index) => {
        revenueControl.get('lostRevenue').setValue(periodWiseLostRevenueData.periodWiseLostRevenue[index].toFixed(2));
      });
      this.vacationAbsensesForm.get('totalLostRevenue').setValue(periodWiseLostRevenueData.totalLostRevenue.toFixed(2));
    } else if (!this.vacationAbsensesForm.value.lostRevenue) {
      if (initLoad) {
        let totalOfOverridenLostRevenue = 0;
        revenueArray.value.forEach(period => {
          totalOfOverridenLostRevenue += period.lostRevenue;
        });
        if (totalOfOverridenLostRevenue && totalOfOverridenLostRevenue > 0) {
          // Display Overriden values in textboxes
        } else {
          this.setDefaultPeriodValues();
        }
      } else {
        this.setDefaultPeriodValues();
      }
    }
    this.runValidations();
  }

  // save vacation absence data
  saveVacationAbsence() {
    this.isSaveClicked = true;
    if (this.vacationAbsensesForm.valid && this.isValid) {
      const percent = this.vacationAbsensesForm.value.percent;
      const periodLostRevenueList: IPeriodLostRevenue[] = this.vacationAbsensesForm.value.periodsLostRevenueList;
      let lostRevenue = 0;

      if (this.vacationAbsensesForm.value.isOverride) {
        if (this.totalRevenue >= 0) {
          periodLostRevenueList.forEach(period => {
            period.lostRevenue = Math.abs(+period.lostRevenue);
            lostRevenue += period.lostRevenue;
          });
        }
        else {
          periodLostRevenueList.forEach(period => {
            period.lostRevenue = Math.abs(+period.lostRevenue) * (-1);
            lostRevenue += period.lostRevenue;
          });
        }
      }

      const formData: IVacationAbsence = {
        pipSheetId: this.pipSheetId,
        amount: percent ? + percent : + this.vacationAbsensesForm.value.amount,
        isOverride: this.vacationAbsensesForm.value.isOverride,
        isPercent: this.vacationAbsensesForm.value.isPercent,
        lostRevenue: + lostRevenue,
        totalRevenue: this.totalRevenue,
        isMarginSet: this.isMarginSet,
        totalLostRevenue: lostRevenue,
        createdBy: 1,
        updatedBy: 1,
        periodLostRevenue: periodLostRevenueList
      };

      this.vacationAbsenceService.saveVacationAbsence(formData).subscribe(res => {
        this.vacationAbsenceService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
          this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
          this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
        });
        if (res === null) {
          this.getVacationAbsence();
          this.translateService.get('SuccessMessage.vacationAbsenceSave').subscribe(msg => {
            this.messageService.add({ severity: 'success', detail: msg });
          });
        }
        this.notificationService.notifyFormDirty(false);
        this.getOverrideNotificationStatus();

      }, () => {
        this.translateService.get('ErrorMessage.vacationAbsenceSave').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
          this.isSaveClicked = false;
        });
      });
    }
  }

  private getPeriodRevenueFormArray(): FormArray {
    const revenueFormArray = this.vacationAbsensesForm.controls.periodsLostRevenueList as FormArray;
    return revenueFormArray;
  }

  setDefaultValues() {
    this.vacationAbsensesForm.get('lostRevenue').setValue(0);
    this.vacationAbsensesForm.get('totalLostRevenue').setValue(0);
    this.overrideDifference = 0;
    const revenueArray = this.getPeriodRevenueFormArray();
    revenueArray.controls.forEach(revenueControl => {
      revenueControl.get('lostRevenue').setValue(0);
    });
    this.runValidations();
  }

  setDefaultPeriodValues() {
    const revenueArray = this.getPeriodRevenueFormArray();
    revenueArray.controls.forEach(revenueControl => {
      revenueControl.get('lostRevenue').setValue(0);
    });
    // update total lost revenue
    this.vacationAbsensesForm.get('totalLostRevenue').setValue(0);
  }
  runValidations() {
    this.isValid = true;
    this.setInitialMessage();
    if ((!this.vacationAbsensesForm.value.isPercent && Math.abs(this.totalRevenue)
      < Math.abs(this.vacationAbsensesForm.value.lostRevenue))) {
      this.translateService.get('VacationAbsences.percentAmountGreaterThanTotalRevenue').subscribe(msg => {
        this.message.type = 'error';
        this.message.text = msg;
      });
      this.vacationAbsensesForm.get('amount').setErrors({ invalid: true });
      this.isValid = false;
    }
    else if (this.vacationAbsensesForm.value.isPercent && this.vacationAbsensesForm.value.percent > 100) {
      this.translateService.get('VacationAbsences.percentAmountGreaterThanTotalRevenue').subscribe(msg => {
        this.message.type = 'error';
        this.message.text = msg;
      });
      this.isValid = false;
    } else if (this.vacationAbsensesForm.value.isOverride && (this.overrideDifference > 1 || this.overrideDifference < 0)) {
      this.translateService.get('VacationAbsences.overrideDifference').subscribe(msg => {
        this.message.type = 'error';
        this.message.text = msg;
      });
      this.isValid = false;
    }
  }

  setInitialMessage() {
    this.translateService.get('VacationAbsences.defaultMessage').subscribe(msg => {
      this.message.type = 'success';
      this.message.text = msg;
    });
  }

  getRevenueControls() {
    const periodsRevenueArray = this.vacationAbsensesForm.controls.periodsLostRevenueList as FormArray;
    return periodsRevenueArray;
  }

  onCellClick(isContextEvent: boolean, event?) {
    if (!isContextEvent) {
      this.currentCellIndex = +event.target.id;
      this.currentRowData = this.vacationAbsensesForm;
    } else {
      const periods: FormArray = (<FormArray>this.getRevenueControls());
      this.currentCellValue = +<FormGroup>periods.controls[this.currentCellIndex].value.lostRevenue;
      periods.controls.forEach((lostRevenueControl, index) => {
        if (index > this.currentCellIndex) {
          (<FormGroup>lostRevenueControl).controls['lostRevenue'].setValue(this.currentCellValue);
        }
      });
      this.onPeriodValueChange();
      this.scrollToEnd((periods.controls.length - 1).toString());
    }
  }
  scrollToEnd(length: string) {
    document.getElementById(length).focus();
  }

  disableOverrideOption() {
    // Disable Override option when Lost Revenue > 0 (Negative FTE Case : When Total Revenue is negative, lost revenue > 0)
    if (this.totalRevenue < 0) {
      this.vacationAbsensesForm.get('isOverride').setValue(false);
      this.vacationAbsensesForm.get('isOverride').disable();
    } else {
      this.vacationAbsensesForm.get('isOverride').enable();
    }
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
}
