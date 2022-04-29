import { OverrideNotificationStatus } from './../../../../shared-module/domain/override-notification-status';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { SharedDataService } from '@global';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IRoleAndAccount } from './../../../../shared-module/domain/IRoleAndAccount';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { Component, OnInit, AfterViewInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, FormBuilder, FormArray, Validators, } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Constants, IPriceAdjustment, IPriceAdjustmentLocation } from '@shared';
import { PriceAdjustmentService } from '@shared/services/price-adjustment.service';
import { MessageService } from 'primeng/api';
import * as moment from 'moment';
import { NotificationService } from '@global';
import { UtilityService } from '@core/infrastructure/utility.service';

@Component({
  selector: 'app-price-adjustment',
  templateUrl: './price-adjustment.component.html'
})
export class PriceAdjustmentComponent implements OnInit, AfterViewInit {
  pipSheetId: number;
  projectId: number;
  priceAdjustmentCols: any[] = [];
  locationCols: any[] = [];
  priceAdjustmentForm: FormGroup;
  startdate: Date = new Date();
  endDate: Date = new Date();
  priceLocationControl: Boolean = true;
  message = { type: '', text: '' };
  priceAdjustmentList: IPriceAdjustment;
  effectiveDate: Date = new Date('01-01-2099');
  isValid = true;
  dateValidationMsg: string;
  dateFormat: string;
  dateValidation: string;
  dateErrorFlag = false;
  triggerDate: string;
  notSupportedDate: string;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  dashboardId: number;
  minDate: Date;
  maxDate: Date;
  isSaveClicked = false;

  constructor(
    private translateService: TranslateService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private priceAdjustmentService: PriceAdjustmentService,
    private messageService: MessageService,
    private sharedDataService: SharedDataService,
    private notificationService: NotificationService,
    private userWorkflowService: UserWorkflowService,
    private utilityService: UtilityService
  ) {
    this.route.paramMap.subscribe(
      params => {
        this.pipSheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
        this.projectId = parseInt(params.get(Constants.uiRoutes.routeParams.projectId), 10);
        this.dashboardId = parseInt(params.get(Constants.uiRoutes.routeParams.dashboardId), 10);
      });
  }
  ngOnInit() {
    this.minDate = this.utilityService.setCalendarMinDate();
    this.maxDate = this.utilityService.setCalendarMaxDate();
    this.translateService.get('PriceAdjustment.PriceAdjustmentColumns').subscribe((cols: any[]) => {
      this.priceAdjustmentCols = cols;
    });
    this.translateService.get('PLANNING.Staff.Control.format').subscribe(format => {
      this.dateFormat = format;
      this.dateValidationMsg = this.dateFormat;
    });
    this.translateService.get('PLANNING.Staff.Control.dateValidation').subscribe(dateValidation => {
      this.dateValidation = dateValidation;
    });
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.notSupportedDate = this.translateService.instant('PriceAdjustment.DateExceed');
    this.priceAdjustmentForm = this.fb.group({
      triggerDate: ['', []],
      effectiveDate: ['', []],
      locationPriceList: this.fb.array([])
    });
    this.getPriceAdjustment();
  }

  ngAfterViewInit() {
    this.priceAdjustmentForm.valueChanges.subscribe(() => {
      if (this.priceAdjustmentForm.dirty) {
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
        this.priceAdjustmentForm.disable();
      }, 200);
    }
  }

  // Get Price Adjustment form
  getPriceAdjustment() {
    const locationPriceArray = this.priceAdjustmentForm.get('locationPriceList') as FormArray;
    const locationColumns = [];
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.priceAdjustmentService.getPriceAdjustmentData(this.pipSheetId).subscribe(priceAdjustmentList => {
        let triggerDate: Date;
        triggerDate = new Date();
        let effectiveDate: Date;
        effectiveDate = new Date();
        if (priceAdjustmentList.priceAdjustmentYoyDTO === null) {
          triggerDate = new Date(triggerDate);
          effectiveDate = new Date(this.effectiveDate);
        }
        else {
          triggerDate = new Date(priceAdjustmentList.priceAdjustmentYoyDTO.triggerDate.toString());
          effectiveDate = new Date(priceAdjustmentList.priceAdjustmentYoyDTO.effectiveDate.toString());
        }
        if (priceAdjustmentList) {
          this.pipSheetId = this.pipSheetId;
          this.startdate = new Date(priceAdjustmentList.projectDurationDTO.startDate);
          this.endDate = new Date(priceAdjustmentList.projectDurationDTO.endDate);
          this.priceAdjustmentForm.get('triggerDate').setValue(triggerDate);
          this.priceAdjustmentForm.get('effectiveDate').setValue(effectiveDate);
          if (priceAdjustmentList.colaDTO && priceAdjustmentList.colaDTO.length > 0) {
            for (const location of priceAdjustmentList.colaDTO) {
              locationColumns.push({ field: location.locationId, header: location.locationName });
              locationPriceArray.push(this.fb.group({
                locationId: [location.locationId, []],
                locationName: [location.locationName, []],
                colaPercent: [location.colaPercent,
                [Validators.pattern(Constants.regExType.percentageWithDecimalPrecisionTwo)]]
              }));
            }
            this.priceAdjustmentCols = this.priceAdjustmentCols;
            this.locationCols = locationColumns;
          }
        }
        this.pipSheetWorkflowStatus = this.sharedDataService.sharedData.pipSheetWorkflowStatus;
        this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
        this.isDataAvailable = true;
        this.enableDisableForm();
      });
    }
    else {
      this.isDataAvailable = true;
    }
  }

  // Consume post api here
  saveAdjustedPrice() {
    this.isSaveClicked = true;
    if (this.priceAdjustmentForm.valid && this.isValid) {
      const locationPriceList: IPriceAdjustmentLocation[] = this.priceAdjustmentForm.value.locationPriceList;
      locationPriceList.forEach(location => {
        location.colaPercent = + location.colaPercent;
        location.locationId = location.locationId;
      });
      const endDate = new Date(this.endDate).toLocaleString();
      const startDate = new Date(this.startdate).toLocaleString();
      let triggerDate: String;
      triggerDate = new Date(this.priceAdjustmentForm.value.triggerDate).toLocaleString();
      let effectiveDate: String;
      effectiveDate = new Date(this.priceAdjustmentForm.value.effectiveDate).toLocaleString();
      const formData: IPriceAdjustment = {
        'priceAdjustmentYoyDTO': {
          pipSheetId: this.pipSheetId,
          triggerDate: triggerDate,
          effectiveDate: effectiveDate,
          createdBy: 1,
          updatedBy: 1,
        },
        'projectDurationDTO': {
          startDate: startDate,
          endDate: endDate,
        },
        colaDTO: locationPriceList,
      };
      this.priceAdjustmentService.savePriceAdjustmentData(formData).subscribe(res => {
        this.priceAdjustmentService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
          this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
          this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
        });
        if (res === null) {
          this.translateService.get('SuccessMessage.PriceAdjustmentSave').subscribe(msg => {
            this.messageService.add({ severity: 'success', detail: msg });
          });
        }
        this.notificationService.notifyFormDirty(false);
        this.getOverrideNotificationStatus();
        this.isSaveClicked = false;
      }, () => {
        this.translateService.get('ErrorMessage.PriceAdjustmentSave').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
          this.isSaveClicked = false;
        });
      });
    }
  }

  // trigger date and effective date logic

  onTriggerDateSelect($event) {
    this.dateErrorFlag = false;
    this.dateValidationMsg = this.dateFormat;
    // this.message.text = '';
    this.triggerDate = $event.target.value;
    if (moment(this.triggerDate, 'MM-DD-YYYY', true).isValid()) {
      this.dateErrorFlag = false;
      this.dateValidationMsg = this.dateFormat;
      this.priceAdjustmentForm.get('effectiveDate').
        setValue(this.priceAdjustmentService.getEffectiveDate(this.endDate, this.priceAdjustmentForm.value.triggerDate));
      this.triggerDateValidationMsg();
      this.priceAdjustmentValidations();
    }
    else {
      this.dateErrorFlag = true;
      this.dateValidationMsg = this.dateValidation;
      this.priceAdjustmentForm.controls['triggerDate'].setErrors({ 'invalid': true });
      this.message.text = '';
    }
    this.notificationService.notifyFormDirty(true);
  }

  onTriggerDateSelect1() {
    if (this.priceAdjustmentForm.controls.triggerDate.value.getFullYear() < 2018) {
      this.priceAdjustmentForm.controls.triggerDate.value.setFullYear(2018);
    }
    this.triggerDate = this.priceAdjustmentForm.controls.triggerDate.value;
    if (moment(this.triggerDate, 'MM-DD-YYYY', true).isValid()) {
      this.dateErrorFlag = false;
      this.dateValidationMsg = this.dateFormat;
      this.priceAdjustmentForm.get('effectiveDate').
        setValue(this.priceAdjustmentService.getEffectiveDate(this.endDate, this.priceAdjustmentForm.value.triggerDate));
      this.triggerDateValidationMsg();
      this.priceAdjustmentValidations();
    }
    else {
      this.dateErrorFlag = true;
      this.dateValidationMsg = this.dateValidation;
    }
  }

  // trigere and effective date validations
  triggerDateValidationMsg() {
    const triggerDate = this.triggerDate;
    const projectEndDate: Date = new Date(this.endDate);
    const monthDiff = ((projectEndDate.getMonth() - this.priceAdjustmentForm.value.triggerDate.getMonth()) +
      (12 * (projectEndDate.getFullYear() - this.priceAdjustmentForm.value.triggerDate.getFullYear())));
    if (new Date(triggerDate).getFullYear() > 2050 || new Date(triggerDate).getFullYear() < 2018) {
      this.priceAdjustmentForm.controls['triggerDate'].setErrors({ 'invalid': true });
      this.dateValidationMsg = 'Year Range : 2018 - 2050';
      this.dateErrorFlag = true;
      // this.message.type = 'error';
    }
    else {
      if (this.priceAdjustmentForm.controls.triggerDate.value <= projectEndDate && monthDiff < 72) {
        this.message.text = 'Applied';
        this.message.type = 'success';
        if (this.priceAdjustmentForm.value.triggerDate.getDate() !== 1 &&
          this.priceAdjustmentForm.value.triggerDate.getMonth() + 1 === 12) {
          this.message.text = 'Applied';
          this.message.type = 'success';
        }
        else if (this.priceAdjustmentForm.value.triggerDate.getDate() !== 1) {
          this.message.text = 'Applied';
          this.message.type = 'success';
        }
      }
      else if (this.priceAdjustmentForm.controls.triggerDate.value > projectEndDate) {
        this.message.text = 'NONE Applied';
        this.message.type = 'error';
      }
      else {
        this.message.text = this.notSupportedDate;
        this.message.type = 'error';
      }
    }
  }

  priceAdjustmentValidations() {
    this.isValid = true;
    const projectEndDate: Date = new Date(this.endDate);
    const monthDiff = ((projectEndDate.getMonth() - this.priceAdjustmentForm.value.triggerDate.getMonth()) +
      (12 * (projectEndDate.getFullYear() - this.priceAdjustmentForm.value.triggerDate.getFullYear())));
    if (this.priceAdjustmentForm.controls.triggerDate.value <= projectEndDate && monthDiff > 72) {
      this.isValid = false;
    }
    if (this.priceAdjustmentForm.controls.triggerDate.value > projectEndDate) {
      this.isValid = false;
      this.noneApplied(true);
    } else {
      this.noneApplied(false);
    }
  }

  noneApplied(isNoneApplied) {
    const locations = this.priceAdjustmentForm.get('locationPriceList') as FormArray;
    if (locations && locations.controls.length > 0) {
      locations.controls.forEach(location => {
        if (isNoneApplied) {
          location.get('colaPercent').setValue(null);
          location.get('colaPercent').disable();
        } else {
          location.get('colaPercent').enable();
        }
      });
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
