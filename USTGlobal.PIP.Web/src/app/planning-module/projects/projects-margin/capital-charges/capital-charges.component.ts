import { isNullOrUndefined } from 'util';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NotificationService, SharedDataService } from '@global';
import { MessageService } from 'primeng/api';
import { ActivatedRoute, Router } from '@angular/router';
import { CapitalChargeService } from '@shared/services/capital-charge.service';
import { ICapitalCharge, Constants } from '@shared';
import { ICapitalChargeMain } from '@shared/domain/ICapitalChargeMain';

@Component({
  selector: 'app-capital-charges',
  templateUrl: './capital-charges.component.html'
})
export class CapitalChargesComponent implements OnInit {
  capitalChargeForm: FormGroup;
  capitalChargeData: ICapitalChargeMain;
  pipSheetId: number;
  projectId: number;
  dashboardId: number;
  accountId: number;
  totalProjectCost = 0;
  translationData: any;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  paymentLag: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private notificationService: NotificationService,
    private messageService: MessageService,
    private capitalChargeService: CapitalChargeService,
    private sharedDataService: SharedDataService,
    private userWorkflowService: UserWorkflowService
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
      this.projectId = parseInt(data.get(Constants.uiRoutes.routeParams.projectId), 10);
      this.dashboardId = parseInt(data.get(Constants.uiRoutes.routeParams.dashboardId), 10);
      this.accountId = parseInt(data.get(Constants.uiRoutes.routeParams.accountId), 10);
    });
  }

  ngOnInit() {
    this.initializeForm();
    this.translationData = {};
    this.translateService.get('CapitalCharges').subscribe(data => {
      this.translationData = data;
    });
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.getCapitalChargeData();
  }

  initializeForm() {
    this.capitalChargeForm = this.fb.group({
      pipSheetId: [this.pipSheetId],
      paymentLag: [],
      capitalChargeDailyRate: [],
      totalCostBeforeCap: [],
      capitalCharge: []
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
        this.capitalChargeForm.disable();
      }, 200);
    }
  }

  configureDirtyCheck() {
    this.capitalChargeForm.valueChanges.subscribe(() => {
      if (this.capitalChargeForm.dirty) {
        this.notificationService.notifyFormDirty(true);
      }
    });
  }

  getCapitalChargeData() {
    this.initializeForm();
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.capitalChargeService.getCapitalChargeData(this.pipSheetId)
        .subscribe(data => {
          this.capitalChargeData = data;
          this.capitalChargeData.capitalChargeDTO.pipSheetId = this.pipSheetId;
          this.bindFormData();
          this.onCalculations();
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

  bindFormData() {
    if (isNullOrUndefined(this.capitalChargeData.capitalChargeDTO.paymentLag)) {
      this.capitalChargeData.capitalChargeDTO.paymentLag =
        this.sharedDataService.sharedData.accountDTO.find(pl => pl.accountId === this.accountId).paymentLag;
    }

    this.capitalChargeForm.patchValue(this.capitalChargeData.capitalChargeDTO);
  }

  onCalculations() {
    const calculatedCapitalChargeData = this.capitalChargeService
      .calculations(this.capitalChargeForm.value, this.capitalChargeData);
    this.capitalChargeData.capitalChargeDTO.capitalCharge = calculatedCapitalChargeData.totalCapitalCharge;
    this.capitalChargeData.projectPeriodTotalDTO = calculatedCapitalChargeData.calculatedPeriodData;
    this.capitalChargeData.capitalChargeDTO.paymentLag = this.capitalChargeForm.value.paymentLag;
    this.capitalChargeForm.get('capitalCharge').setValue(this.capitalChargeData.capitalChargeDTO.capitalCharge);
  }

  onSave(formData) {
    this.capitalChargeService.saveCapitalCharge(this.capitalChargeData).subscribe(success => {
      this.capitalChargeService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
        this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
      });
      this.getCapitalChargeData();
      this.getOverrideNotificationStatus();
      this.translateService.get('SuccessMessage.CapitalCharge').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      this.notificationService.notifyFormDirty(false);
    }, () => {
      this.translateService.get('ErrorMessage.CapitalCharge').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
    });
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
