import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { IRoleAndAccount } from './../../../../shared-module/domain/IRoleAndAccount';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { LocationComboItem } from '@shared/domain/locationcomboitem';
import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ICurrency, IEbitda, Constants, AllowedEbidaSeatCostLocationsOverride, CurrencyConversionPipe } from '@shared';
import { EbitdaService } from '@shared/services/ebitda.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedDataService, NotificationService } from '@global';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { IHeader1 } from '@shared/domain/IHeader1';
import { Breadcrumb } from '@shared/breadcrumb/breadcrumb.model';
import { BreadcrumbService } from '@shared/breadcrumb/breadcrumb.service';
import { OverrideNotificationStatus } from '@shared/domain/override-notification-status';
import { markDirty } from '@angular/core/src/render3';

const allowedEbidaSeatCostLocationsForOverride = Object.values(AllowedEbidaSeatCostLocationsOverride);

@Component({
  selector: 'app-ebitda',
  templateUrl: './ebitda.component.html',
})
export class EbitdaComponent implements OnInit, AfterViewInit, OnDestroy {

  subscriptions: Subscription[] = [];
  isDataAvailable = false;
  ebitdaForm: FormGroup;
  ebitdaCols: any[] = [];
  ebitdaList: IEbitda[] = [];

  projectId: number;
  pipSheetId: number;
  accountId: number;
  dashboardId: number;
  currencyData: ICurrency;
  usdToLocal: number;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  loggedInUserId: number;
  isSaveClicked = false;
  pOverrideShowToolTip: string;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private ebitdaService: EbitdaService,
    private notificationService: NotificationService,
    private sharedData: SharedDataService,
    private messageService: MessageService,
    private breadcrumbService: BreadcrumbService,
    private router: Router,
    private userWorkflowService: UserWorkflowService
  ) {
    this.route.paramMap.subscribe(
      params => {
        this.projectId = parseInt(params.get(Constants.uiRoutes.routeParams.projectId), 10);
        this.pipSheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
        this.accountId = parseInt(params.get(Constants.uiRoutes.routeParams.accountId), 10);
        this.dashboardId = parseInt(params.get(Constants.uiRoutes.routeParams.dashboardId), 10);
      });
  }

  ngOnInit() {
    this.translateService.get('Ebitda.EbitdaColumns').subscribe(cols => {
      this.ebitdaCols = cols;
    });

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });

    this.ebitdaForm = this.fb.group({
      ebitdaStdOverhead: this.fb.array([]),
    });

    this.getEbitda(this.pipSheetId);

    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.ebitdaService.getCurrencyConversionData(this.pipSheetId).subscribe(response => {
        this.usdToLocal = response.usdToLocal;
      });
    }

  }

  ngAfterViewInit() {
    this.ebitdaForm.valueChanges.subscribe(() => {
      if (this.ebitdaForm.dirty && this.dashboardId !== 3) {
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
        this.ebitdaForm.disable();
      }, 200);
    }
  }

  getEbitdaForm(ebitda: IEbitda): FormGroup {

    return this.fb.group({
      locationId: ebitda.locationId,
      locationName: ebitda.locationName,
      refUSD: ebitda.refUSD,
      overrideValue: [ebitda.isStdOverheadOverriden === true ? this.setOverrideValueValidation(ebitda) : ''
        , [this.validateOverriddenValue.bind('', ebitda)]],
      overheadAmount: ebitda.overheadAmount,
      ebitdaSeatCost: ebitda.ebitdaSeatCost,
      isStdOverheadOverriden: ebitda.isStdOverheadOverriden,
      sharedSeatsUsePercent: [ebitda.sharedSeatsUsePercent, [Validators.pattern(Constants.regExType.percentageWithDecimalPrecisionTwo)]],
      charges: [''],
    });
  }

  validateOverriddenValue(ebitda: IEbitda, control: AbstractControl): { [key: string]: any } | null {
    let valid = true;
    if (control.value === '' || control.value === null) {
      valid = true;
    }
    else if (+ebitda.refUSD.toFixed(2) > + parseFloat(control.value).toFixed(2)) {
      valid = false;
      control.markAsDirty();
      control.markAsTouched();
    }
    else {
      valid = true;
    }
    return valid
      ? null
      : { invalidNumber: { valid: false, value: control.value } };
  }

  setOverrideValueValidation(ebitda: IEbitda) {

    if (+ ebitda.refUSD.toFixed(2) > +ebitda.overheadAmount.toFixed(2)) {
      this.isSaveClicked = true;
      return ebitda.overheadAmount;
    }
    else {
      this.isSaveClicked = false;
      return ebitda.overheadAmount;
    }
  }

  getEbitda(pipSheetId: number) {
    this.ebitdaService.getEbitdaData(pipSheetId).subscribe(ebitdaList => {
      this.ebitdaList = ebitdaList;
      this.buildEbitdaForm();
      this.ebitdaService.getWorkflowStatusAccountRole(this.pipSheetId, this.accountId)
        .subscribe(roleAndWfStatus => {
          this.roleAndAccount = roleAndWfStatus.roleAndAccountDTO;
          this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
          this.sharedData.roleAndAccount = roleAndWfStatus.roleAndAccountDTO;
          if (this.roleAndAccount != null) {
            this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
          }
          if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
            this.pipSheetWorkflowStatus = roleAndWfStatus.pipSheetWorkflowStatus;
            this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus, this.wfstatus);
            this.isDataAvailable = true;
            this.enableDisableForm();
          }
          else {
            this.isDataAvailable = true;
          }
        });
    });
  }

  buildEbitdaForm() {
    const ebitdaListForm = this.fb.array([]);
    this.ebitdaList.forEach((ebitda: IEbitda) => {
      ebitdaListForm.push(this.getEbitdaForm(ebitda));
    });
    this.ebitdaForm.setControl('ebitdaStdOverhead', ebitdaListForm);

    for (let i = 0; i < this.ebitdaList.length; i++) {
      this.updateCharges(i);
    }
  }

  isEbidaSeatCostLocationAllowedToOverride(localtionId: number): boolean {
    return allowedEbidaSeatCostLocationsForOverride.includes(localtionId);
  }

  onStdOverheadOverride(rowIndex) {
    const ebitdaStdOverheadFormList = this.ebitdaForm.get('ebitdaStdOverhead') as FormArray;
    const ebitda = ebitdaStdOverheadFormList.controls[rowIndex] as FormGroup;

    if (ebitda.value.overrideValue === '' || ebitda.value.overrideValue === null) {
      ebitda.controls.isStdOverheadOverriden.setValue(false);
      this.isSaveClicked = false;
    }
    else {
      ebitda.controls.isStdOverheadOverriden.setValue(true);
      if (+((ebitda.value.overrideValue).toFixed(2)) < +((ebitda.value.refUSD).toFixed(2))) {
        this.isSaveClicked = true;
      } else {
        this.isSaveClicked = false;
      }
    }
  }

  updateCharges(index: number): void {
    const ebitdaStdOverheadFormList = this.ebitdaForm.get('ebitdaStdOverhead') as FormArray;
    const ebitdaSeatCost = ebitdaStdOverheadFormList.controls[index].value.ebitdaSeatCost;
    let percentageUse = ebitdaStdOverheadFormList.controls[index].value.sharedSeatsUsePercent;
    const ebitdaFormControls = ebitdaStdOverheadFormList.controls[index];

    if (!percentageUse || percentageUse === '') {
      percentageUse = 100;
    }
    ebitdaStdOverheadFormList.controls[index].patchValue({ charges: this.calculateCharges(percentageUse, ebitdaSeatCost) });
  }

  calculateCharges(percentageUse: number, ebitdaSeatCost: number): number {
    return (percentageUse * ebitdaSeatCost) / 100;
  }

  getOverrideNotificationErrorMsg(rowIndex) {
    const ebitdaStdOverheadFormList = this.ebitdaForm.get('ebitdaStdOverhead') as FormArray;
    const ebitda = ebitdaStdOverheadFormList.controls[rowIndex] as FormGroup;
    if (ebitda.value.overrideValue === '' || ebitda.value.overrideValue === null) {
      this.pOverrideShowToolTip = '';
    }
    else if (ebitda.value.overrideValue < ebitda.value.refUSD) {
      this.translateService.get('Ebitda.OverrideLessThanRefUSDMsg').subscribe(msg => {
        this.pOverrideShowToolTip = msg;
      });
    } else {
      this.pOverrideShowToolTip = '';
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

  onSaveClick() {
    this.isSaveClicked = true;
    const ebitdaList: IEbitda[] = [];

    this.ebitdaForm.get('ebitdaStdOverhead').value.map((ebitdaFormValue) => {
      const ebitda: IEbitda = {
        // ...ebitdaFormValue, //Should use spread operator if all control's name are same as the model
        locationId: ebitdaFormValue.locationId,
        locationName: ebitdaFormValue.locationName,
        pipSheetId: this.pipSheetId,
        refUSD: ebitdaFormValue.refUSD,
        overheadAmount: parseFloat(ebitdaFormValue.isStdOverheadOverriden === true ? ebitdaFormValue.overrideValue
          : ebitdaFormValue.refUSD),
        ebitdaSeatCost: ebitdaFormValue.ebitdaSeatCost,
        sharedSeatsUsePercent: ebitdaFormValue.sharedSeatsUsePercent ? ebitdaFormValue.sharedSeatsUsePercent : 100,
        isStdOverheadOverriden: (ebitdaFormValue.overrideValue !== '' && ebitdaFormValue.overrideValue !== null) ? true : false,
        createdBy: 0, updatedBy: 0,
      };
      ebitdaList.push(ebitda);
    });

    if (this.ebitdaForm.valid) {
      this.ebitdaService.saveEbitdaData(ebitdaList).subscribe((res: number) => {
        this.ebitdaService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
          this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
          this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
        });
        this.translateService.get('SuccessMessage.EbitdaSave').subscribe(msg => {
          this.messageService.add({ severity: 'success', detail: msg });
          this.notificationService.notifyFormDirty(false);
          this.getOverrideNotificationStatus();
          this.isSaveClicked = false;
        },
          () => {
            this.translateService.get('ErrorMessage.EbitdaSave').subscribe(msg => {
              this.messageService.add({ severity: 'error', detail: msg });
              this.isSaveClicked = false;
            });
          });
      });
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => {
      s.unsubscribe();
    });
  }
}
