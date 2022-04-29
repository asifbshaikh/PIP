import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { SelectItem, MessageService } from 'primeng/api';
import { SharedDataService } from '@global';
import { Mastermapper } from '@shared';
import { ReportsService } from '@shared/services/reports.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import * as moment from 'moment';
import { isNullOrUndefined } from 'util';
import { IGenerateReport } from '@shared/domain/IGenerateReport';
import { ISelectedAccount } from '@shared/domain/ISelectedAccount';
import { ISelectedProject } from '@shared/domain/ISelectedProject';
import { ISelectedKPI } from '@shared/domain/ISelectedKPI';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html'
})
export class ReportsComponent implements OnInit {
  reportForm: FormGroup;
  Report: any;
  menu: number;
  accountDropdownItems: SelectItem[];
  projectListDropdownItems: SelectItem[];
  kPIListDropdownItems: SelectItem[];
  accountDTO: ISelectedAccount[];
  projectListDTO: ISelectedProject[];
  kPIList: ISelectedKPI[];
  startDate: any;
  endDate: any;
  selStartDate: Date;
  selEndDate: Date;
  dateToolTip: string;
  startDateValidationMsg: string;
  endDateValidationMsg: string;
  noProjectsError: string;
  dateValidation: string;
  dateformat: string;
  dateError: boolean;
  dateErrorFlag: boolean;
  invalidFormat: boolean;
  constructor(
    private translateService: TranslateService,
    private sharedDataService: SharedDataService,
    private reportsService: ReportsService,
    private fb: FormBuilder,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.translateService.get('Reports').subscribe(Report => {
      this.Report = Report;
    });
    this.menu = 1;
    this.initializeFrom();
  }

  initializeFrom() {
    this.reportForm = this.fb.group({
      account: ['', [Validators.required]],
      project: ['', [Validators.required]],
      kpi: [],
      currency: ['true', [Validators.required]],
      startDate: [],
      endDate: [],
    });
    if (this.menu === 1) {
      this.reportForm.get('kpi').setValidators([Validators.required]);
    }
    if (this.menu !== 4) {
      this.reportForm.get('startDate').setValidators([Validators.required]);
      this.reportForm.get('endDate').setValidators([Validators.required]);
    }
    this.getAccountDropdown();
    this.getKPIDropdown();
    this.voidValidationMsg();
  }
  startDateValidation($event) {
    this.dateErrorFlag = false;
    if (moment(this.startDate, 'MM-DD-YYYY', true).isValid()) {
      this.dateErrorFlag = false;
    }
    else {
      this.dateErrorFlag = true;
    }
  }
  getKPIDropdown() {
    this.reportsService.getReportKPIList().subscribe(reportKPI => {
      this.kPIListDropdownItems = new Mastermapper().getReportKPIComboItems(reportKPI);
    });
  }

  getAccountDropdown() {
    this.reportsService.getAuthorizedAccounts().subscribe(authAccounts => {
      if (this.menu === 1 || this.menu === 2) {
        this.accountDropdownItems = new Mastermapper().
          getAccountComboItemsForReport(this.sharedDataService.sharedData.accountDTO, authAccounts,
            this.sharedDataService.sharedData.hasAccountLevelEditorAccess,
            this.sharedDataService.sharedData.hasFinanceApproverAccess, false);
      }
      else {
        this.accountDropdownItems = new Mastermapper().
          getAccountComboItemsForReport(this.sharedDataService.sharedData.accountDTO, authAccounts,
            this.sharedDataService.sharedData.hasAccountLevelEditorAccess,
            this.sharedDataService.sharedData.hasFinanceApproverAccess, true);
      }
    });
  }

  onMenuChange(path) {
    this.menu = path;
    this.projectListDropdownItems = [];
    this.getAccountDropdown();
    this.initializeFrom();
  }

  onAccountChange(accountItems) {
    this.accountDTO = [];
    accountItems.forEach(account => {
      this.accountDTO.push({
        accountId: account.id,
        accountName: account.name,
        accountCode: account.code,
        paymentLag: account.value,
      });
    });
    this.reportsService.getProjectListForAccount(this.accountDTO).subscribe(projects => {
      if (projects.length < 1) {
        this.reportForm.controls.project.markAsDirty();
        this.noProjectsError = this.translateService.instant('MESSAGES.Error.NoProjectError');
      }
      else {
        this.noProjectsError = '';
      }
      if (this.menu === 1 || this.menu === 2 || this.menu === 3) {
        this.projectListDropdownItems = new Mastermapper().getProjectListForAccountComboItems(projects, false);
      }
      else {
        this.projectListDropdownItems = new Mastermapper().getProjectListForAccountComboItems(projects, true);
      }
      this.reportForm.controls.project.reset();
      this.reportForm.setErrors({ invalid: true });
    });
  }

  onProjectSelect(projectList) {
    this.projectListDTO = [];
    projectList.forEach(element => {
      if (element.id !== -1) {
        this.projectListDTO.push({
          sFProjectId: element.code,
          projectName: element.name,
          projectId: element.id
        });
      }
      else {
        this.reportForm.controls.project.setErrors({ invalid: true });
      }
    });
  }
  onKpiSelect(kPIList) {
    this.kPIList = [];
    kPIList.forEach(kpi => {
      this.kPIList.push({
        kpiId: kpi.id,
        kpiName: kpi.code,
        plForecastLabelId: kpi.name
      });
    });
  }

  generateProjectReport() {
    let reportList: IGenerateReport;
    reportList = {
      startDate: this.reportForm.value.startDate ? this.reportForm.value.startDate.toLocaleString() : new Date(),
      endDate: this.reportForm.value.endDate ? this.reportForm.value.endDate.toLocaleString() : new Date(),
      isUSDCurrency: this.reportForm.value.currency,
      reportType: this.menu,
      selectedAccounts: this.accountDTO ? this.accountDTO : [],
      selectedKPIs: this.kPIList ? this.kPIList : [],
      selectedProjects: this.projectListDTO ? this.projectListDTO : []
    };
    this.reportsService.generateReport(reportList).subscribe(res => {
      if (!res) {
        this.translateService.get('SuccessMessage.GenerateReport').subscribe(msg => {
          this.reportForm.reset();
          this.messageService.add({ severity: 'success', detail: msg });
        });
      }
      else {
        this.translateService.get('ErrorMessage.GenerateReport').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
        });
      }
    });
  }

  validateStartDate(date) {
    this.reportForm.controls.startDate.markAsDirty();
    this.startDate = date;
    this.selStartDate = null;
    if (moment(this.startDate, 'MM-YYYY', true).isValid()) {
      this.validateDate();
    }
    else {
      this.reportForm.controls.startDate.setErrors({ invalid: true });
      this.startDateValidationMsg = this.translateService.instant('MESSAGES.Error.MonthYearFormatError');
    }
  }

  validateEndDate(date) {
    this.reportForm.controls.endDate.markAsDirty();
    this.selEndDate = null;
    this.endDate = date;
    if (moment(this.endDate, 'MM-YYYY', true).isValid()) {
      this.validateDate();
    }
    else {
      this.reportForm.controls.endDate.setErrors({ invalid: true });
      this.endDateValidationMsg = this.translateService.instant('MESSAGES.Error.MonthYearFormatError');
    }
  }

  validateDate() {
    const startDate = isNullOrUndefined(this.startDate) ?
      (((this.selStartDate.getMonth()) < 10 ? ('0' + this.selStartDate.getMonth()) : this.selStartDate.getMonth())
        + '-' + this.selStartDate.getFullYear()).toLocaleString() : (this.startDate.split(''));
    const endDate = isNullOrUndefined(this.endDate) ? isNullOrUndefined(this.selEndDate) ? '' : ((
      this.selEndDate.getMonth() < 10 ? '0' + this.selEndDate.getMonth() : this.selEndDate.getMonth()) +
      '-' + this.selEndDate.getFullYear()).toLocaleString() : this.endDate.split('');
    const startMonth = isNullOrUndefined(this.startDate) ? isNullOrUndefined(this.selStartDate) ? ''
      : (this.selStartDate.getMonth() + 1) : +(this.startDate[0] + this.startDate[1]);
    const endMonth = isNullOrUndefined(this.endDate) ? isNullOrUndefined(this.selEndDate) ? ''
      : (this.selEndDate.getMonth() + 1) : +(this.endDate[0] + this.endDate[1]);
    let startYear = isNullOrUndefined(this.startDate) ? '' : +(this.startDate[3] +
      this.startDate[4] + this.startDate[5] + this.startDate[6]);
    let endYear = isNullOrUndefined(this.endDate) ? '' : +(this.endDate[3] + this.endDate[4] + this.endDate[5] + this.endDate[6]);
    const getStartYear = isNullOrUndefined(this.selStartDate) ? '' : this.selStartDate.getFullYear();
    const getEndYear = isNullOrUndefined(this.selEndDate) ? '' : this.selEndDate.getFullYear();

    if (startDate.length === 7 && endDate.length === 7) {
      if (startYear === '' ? startYear = getStartYear : startYear && endYear === '' ? endYear = getEndYear : endYear) {
        if (startYear > endYear || (startYear === endYear && startMonth > endMonth) || startMonth > 12 || endMonth > 12) {
          this.reportForm.controls.startDate.setErrors({ invalid: true });
          this.reportForm.controls.endDate.setErrors({ invalid: true });
          if (startMonth > endMonth) {
            this.reportForm.controls.startDate.setErrors({ invalid: true });
            this.reportForm.controls.endDate.setErrors({ invalid: true });
          }
          this.dateToolTip = this.translateService.instant('MESSAGES.Tooltip.ProjectControlStartDateError');
        }
        else {
          this.reportForm.controls.startDate.setErrors(null);
          this.reportForm.controls.endDate.setErrors(null);
          this.dateToolTip = '';
        }
      }
      else {
        if (moment(this.startDate, 'MM-YYYY', true).isValid()) {
          if (this.startDate) {
            this.reportForm.controls.startDate.setValue(this.startDate);
          }
        }
        else if (moment(this.endDate, 'MM-YYYY', true).isValid()) {
          if (this.endDate) {
            this.reportForm.controls.endDate.setValue(this.endDate);
          }
        }
      }
    }
    if (startDate || endDate) {
      if (startDate.length === 7 && (startYear >= 2018 && startYear <= 2050)) {
        this.startDateValidationMsg = '';
      }
      else if (startYear && startYear < 2018 || startYear > 2050) {
        this.reportForm.controls.startDate.setErrors({ invalid: true });
        this.startDateValidationMsg = this.translateService.instant('MESSAGES.Tooltip.ReportStartDateRange');
      }
      if (endDate.length === 7 && (endYear >= 2018 && endYear <= 2050)) {
        this.endDateValidationMsg = '';
      }
      else if (endYear && (endYear < 2018 || endYear > 2050)) {
        this.reportForm.controls.endDate.setErrors({ invalid: true });
        this.endDateValidationMsg = this.translateService.instant('MESSAGES.Tooltip.ReportEndDateRange');
      }
    }
  }

  valicateSelectedDate() {
    const selStartDate = isNullOrUndefined(this.selStartDate) ? isNullOrUndefined(this.startDate) ? undefined
      : moment('01' + '-' + this.startDate, 'DD/MM/YYYY') : this.selStartDate;
    const selEndDate = isNullOrUndefined(this.selEndDate) ? isNullOrUndefined(this.endDate) ? undefined
      : moment('01' + '-' + this.endDate, 'DD/MM/YYYY') : this.selEndDate;
    if (moment(selStartDate).year() < 2018 || moment(selEndDate).year() > 2050) {
      this.selStartDate.setFullYear(2018);
      this.selEndDate.setFullYear(2018);
    }
    if (selStartDate > selEndDate) {
      this.reportForm.controls.startDate.setErrors({ invalid: true });
      this.reportForm.controls.endDate.setErrors({ invalid: true });
      this.dateToolTip = this.translateService.instant('MESSAGES.Tooltip.ProjectControlStartDateError');
    }
    else {
      if (selStartDate && selEndDate) {
        this.reportForm.controls.startDate.setErrors(null);
        this.reportForm.controls.endDate.setErrors(null);
      }
      else {
        this.reportForm.setErrors({ invalid: true });
      }
      this.dateToolTip = '';
    }
  }

  onStartDateSelect(date) {
    this.startDate = null;
    this.selStartDate = date;
    this.startDateValidationMsg = '';
    if (this.selStartDate.getFullYear() < 2018 || this.selStartDate.getFullYear() > 2050) {
      this.selStartDate.setFullYear(2018);
    }
    this.valicateSelectedDate();
  }

  onEndDateSelect(date) {
    this.endDate = null;
    this.selEndDate = date;
    if (this.selEndDate.getFullYear() < 2018 || this.selEndDate.getFullYear() > 2050) {
      this.selEndDate.setFullYear(2018);
    }
    this.endDateValidationMsg = '';
    this.valicateSelectedDate();
  }

  voidValidationMsg() {
    this.dateToolTip = '';
    this.startDateValidationMsg = '';
    this.endDateValidationMsg = '';
    this.noProjectsError = '';
    this.startDate = null;
    this.selStartDate = null;
    this.endDate = null;
    this.endDate = null;
  }
}
