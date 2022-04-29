import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { IRoleAndAccount } from './../../../../shared-module/domain/IRoleAndAccount';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { MonthlyData } from './../../../../shared-module/domain/monthlydata';
import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl, AbstractControl, ValidatorFn } from '@angular/forms';
import { SelectItem, MessageService } from 'primeng/api';
import { ProjectControlService } from '@shared/services/projects-control.service';
import { Mastermapper } from '@shared/mapper/master/mastermapper';
import {
  IMasterDetails, MasterColumn, LocationComboItem, IProjectControl, Constants,
  IPIPSheet, IProjectMilestone, IProjectLocation, IMilestone
} from '@shared';
import { TranslateService } from '@ngx-translate/core';
import { ValidationService } from '@core';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedDataService, NotificationService } from '@global';
import { DateService } from '@core/services/date.service';
import * as moment from 'moment';
import { isNullOrUndefined } from 'util';
import { OverrideNotificationStatus } from '@shared/domain/override-notification-status';
import { UtilityService } from '@core/infrastructure/utility.service';
import { isEmptyObject } from 'jquery';

const MINIMUM_NO_OF_MILESTONE_GRID_ROWS = 15;

export class NgbdDatepickerPopup {
  model;
}
@Component({
  selector: 'app-projects-control',
  templateUrl: './projects-control.component.html'
})

export class ProjectsControlComponent implements OnInit, AfterViewInit {
  projectControl: FormGroup;
  locations: SelectItem[];
  selectedLocations: LocationComboItem[];
  selectedMilestones: any[] = [];
  locationsData: IMasterDetails;
  tooltipMessage: any;
  cols: MasterColumn[];
  milestoneColumns: MasterColumn[];
  holidaysOption: SelectItem[];
  projControl: IProjectControl;
  projectId: number;
  currencyId: number;
  selectedLocationsValues: any;
  locationsHeaderValues: any;
  pShowToolTip: string;
  pLocationShowToolTip: string;
  dateToolTip: string;
  calendar: any;
  numeric: {};
  projectMilestoneGroup: SelectItem[];
  masterMilestoneList: IMilestone[];
  projectMilestoneList: IProjectMilestone[];
  pipSheetId: number;
  dashboardId: number;
  lastMasterMilestoneId: number;
  locationTooltip: any[] = [];
  dateValidationMsg: string;
  error: any = { isError: false, errorMessage: '' };
  dateValidation: string;
  dateformat: string;
  dateError: boolean;
  dateErrorFlag: boolean;
  invalidFormat: boolean;
  startDate: any;
  endDate: any;
  endDateValidationMsg: string;
  invalidEndFormat: boolean;
  totalPeriods = 0;
  overrideLimits: any;
  isChecked: boolean;
  isMilestoneDeleted: boolean;
  isCustomMilestone: boolean;
  milestoneGroupId: number;
  setFlag = false;
  display = false;
  isInvoiceAmount = false;
  locationIndex: number;
  locationId: number;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  disableOtherFormControls = false;
  overrideNotification: OverrideNotificationStatus;
  minDate: Date;
  maxDate: Date;
  startDateValidationMsg: string;
  selStartDate: Date = new Date();
  selEndDate: Date = new Date();

  constructor(
    private fb: FormBuilder,
    private translateService: TranslateService,
    private projectControlService: ProjectControlService,
    private route: ActivatedRoute,
    private validate: ValidationService,
    private sharedData: SharedDataService,
    private notificationService: NotificationService,
    private dateService: DateService,
    private router: Router,
    private messageService: MessageService,
    private userWorkflowService: UserWorkflowService,
    private utilityService: UtilityService
  ) {
    this.getLocations();
    this.route.paramMap.subscribe(
      params => {
        this.projectId = parseInt(params.get(Constants.uiRoutes.routeParams.projectId), 10);
        this.pipSheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
        this.dashboardId = parseInt(params.get(Constants.uiRoutes.routeParams.dashboardId), 10);
      });
  }
  getLocations() {
    return this.locations;
  }

  ngOnInit() {
    this.minDate = this.utilityService.setCalendarMinDate();
    this.maxDate = this.utilityService.setCalendarMaxDate();

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.translateService.get('MESSAGES.Tooltip.LocationTooltipMessage').subscribe(tooltip => {
      this.locationTooltip = tooltip;
    });
    this.translateService.get('PLANNING.Staff.Control.dateValidation').subscribe(dateValidation => {
      this.dateValidation = dateValidation;
    });
    this.translateService.get('PLANNING.Staff.Control.format').subscribe(format => {
      this.dateformat = format;
    });

    this.translateService.get('ProjectControl.OverrideLimits').subscribe(projectControl => {
      this.overrideLimits = projectControl;
    });

    this.numeric = Constants.regExType.numeric;
    this.projectControl = this.fb.group({
      startDate: ['', [Validators.required]],
      endingDate: ['', Validators.required],
      holidaysOption: ['', [this.validate.validateDeselectedDropdown]],
      locations: ['', Validators.required],
      selectedLocations: this.fb.array([], Validators.required),
      milestone: this.fb.group({
        defaultGroup: ['', []],
        milestones: this.fb.array([])
      }),
    });

    this.calendar = Constants.calendar;
    this.holidaysOption = this.getHolidayOptions();

    this.roleAndAccount = this.sharedData.roleAndAccount;
    this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }

    if (this.projectId > 0) {
      this.projectControlService.getLocations(this.projectId, this.pipSheetId).subscribe(data => {
        this.locations = new Mastermapper().getLocationComboItems(data);
        if (this.pipSheetId) {
          this.projectControlService.getProjectControlData(this.pipSheetId).subscribe(locationData => {
            this.projControl = locationData;
            if (locationData.projectMilestoneListDTO.length > 0) {
              this.projectMilestoneList = locationData.projectMilestoneListDTO;
            }
            else {
              this.projectMilestoneList = [];
              this.masterMilestoneList.forEach(m => {
                const projectMilestone: IProjectMilestone = {
                  uId: 0,
                  milestoneId: m.milestoneId,
                  milestoneGroupId: m.milestoneGroupId,
                  milestoneName: m.milestoneName,
                  pipSheetId: this.pipSheetId,
                  projectMilestoneId: 0,
                  isChecked: false,
                  originalValue: null,
                  overrideValue: null,
                  invoiceAmount: null,
                  milestoneMonth: '',
                  createdBy: 1,
                  updatedBy: 1,
                  createdOn: new Date(),
                  updatedOn: new Date()
                };
                this.projectMilestoneList.push(projectMilestone);
              });
            }

            if (this.projControl.pipSheetListDTO.length > 0) { this.bindProjectControls(); }
            this.showPeriods();
            this.pipSheetWorkflowStatus = this.sharedData.sharedData.pipSheetWorkflowStatus;
            this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
            this.isDataAvailable = true;
            this.enableDisableForm();
          });
        }
        this.locationsHeaderValues = this.locations[0].value;
        if (this.locationsHeaderValues.codeperday > 0) {
          this.translateService.get('PLANNING.Staff.Control').subscribe((resource) => {
            this.cols = [
              resource['LocationCoulmn'],
              resource['DefaultHoursPerDayCoulmn'],
              resource['OverrideHoursPerDayCoulmn']
            ];
          });
        }
        else {
          this.translateService.get('PLANNING.Staff.Control').subscribe((resource) => {
            this.cols = [
              resource['LocationCoulmn'],
              resource['DefaultHoursPerMonthCoulmn'],
              resource['OverrideHoursPerMonthCoulmn']
            ];
          });
        }
      });
    }
    else {
      this.isDataAvailable = true;
    }

    this.translateService.get('PLANNING.Staff.Control.MilestoneColumns').subscribe((milestoneColumns) => {
      this.milestoneColumns = milestoneColumns;
    });

    this.translateService.get('PLANNING.Staff.Control').subscribe((resource) => {
      this.cols = [
        resource['LocationCoulmn'],
        resource['DefaultHoursPerMonthCoulmn'],
        resource['DefaultHoursPerDayCoulmn'],
        resource['OverrideHoursPerMonthCoulmn'],
        resource['OverrideHoursPerDayCoulmn']
      ];
    });

    this.projectMilestoneGroup = new Mastermapper().getMilestoneGroupComboItems(this.sharedData.sharedData.milestoneGroupDTO);
    this.masterMilestoneList = this.sharedData.sharedData.milestoneDTO;
  }

  ngAfterViewInit() {
    this.projectControl.valueChanges.subscribe(() => {
      if (this.projectControl.dirty) {
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
        this.disableOtherFormControls = true;
        this.projectControl.disable();
      }, 200);
    }
  }

  getHolidayOptions(): Array<SelectItem> {
    const items: Array<SelectItem> = [];
    items.push(
      {
        label: Constants.HolidayOptionLabel.on,
        value: {
          id: 1,
          code: true,
          name: Constants.HolidayOptionLabel.on
        }
      },
      {
        label: Constants.HolidayOptionLabel.off,
        value: {
          id: 0,
          code: false,
          name: Constants.HolidayOptionLabel.off
        }
      });
    return items;
  }

  onLocationSelected(items: any) {
    const selLocs = this.projectControl.controls.selectedLocations as FormArray;
    if (items.length >= 1) {
      this.updateSelectedLocationsForm(items);
    }
    else {
      while (selLocs.length > 0) {
        selLocs.removeAt(0);
      }
    }
  }

  updateSelectedLocationsForm(selectedItems: LocationComboItem[]) {
    this.selectedLocationsValues = selectedItems[0];
    const codePerDay = this.selectedLocationsValues.codeperday > 0 ? true : false;
    const codePerMonth = this.selectedLocationsValues.codepermonth > 0 ? true : false;

    // populate selected locations grid
    const selLocs: FormArray = new FormArray([]);
    let count = 0;

    selectedItems.forEach(item => {
      const overRideItem = this.projControl.projectLocationListDTO.find(pl => pl.locationId === item.id);
      let matchedItem: any;
      if (this.projectControl.value.selectedLocations) {
        this.projectControl.value.selectedLocations.forEach(sl => {
          if (sl.id === item.id) {
            matchedItem = sl;
            this.setFlag = true;
          }
        });
      }

      selLocs.push(new FormGroup({
        overrideCodePerDay: new FormControl(codePerDay ? (matchedItem ? matchedItem.overrideCodePerDay : (overRideItem ?
          (overRideItem.isOverride ? (overRideItem.hoursPerDay === 0 ? '' : overRideItem.hoursPerDay) : '') : '')) : 0,
          [this.validateHoursPerDay.bind(this)]),
        overrideCodePerMonth: new FormControl(codePerMonth ? (matchedItem ? matchedItem.overrideCodePerMonth : (overRideItem ?
          (overRideItem.isOverride ? (overRideItem.hoursPerMonth === 0 ? '' : overRideItem.hoursPerMonth) : '') : '')) : 0,
          [this.validateHoursPerMonth.bind(this)]),
        name: new FormControl(item.name),
        id: new FormControl(item.id),
        isOverride: new FormControl(matchedItem ? matchedItem.isOverride : (overRideItem ? overRideItem.isOverride : false)),
        defaultCodePerDay: new FormControl(item.codeperday),
        defaultCodePerMonth: new FormControl(item.codepermonth)
      }));
      count = count + 1;
    });
    this.projectControl.setControl('selectedLocations', selLocs);
  }

  public validateHoursPerDay(control: AbstractControl): { [key: string]: any } | null {
    if (this.selectedLocationsValues ? (this.selectedLocationsValues.codeperday > 0) : false) {
      const valid = ((control.value < 1 && control.value !== '') || (control.value > 24 && control.value !== '')) ? false : true;
      if (!valid && this.setFlag === true) {
        this.setValidation(control);
      }
      return valid
        ? null
        : { invalidNumber: { valid: false, value: control.value } };
    }
  }

  public validateHoursPerMonth(control: AbstractControl): { [key: string]: any } | null {
    if (this.selectedLocationsValues ? (this.selectedLocationsValues.codepermonth > 0) : false) {
      const valid = ((control.value < 1 && control.value !== '') || (control.value > 300 && control.value !== '')) ? false : true;
      if (!valid && this.setFlag === true) {
        this.setValidation(control);
      }
      return valid
        ? null
        : { invalidNumber: { valid: false, value: control.value } };
    }
  }

  setValidation(control: AbstractControl) {
    control.markAsDirty();
    control.markAsTouched();
    this.setFlag = false;
  }

  setIsOverride(value: any, index: number) {
    const rowDataGroup = this.projectControl.controls.selectedLocations as FormGroup;
    const rowDataArray = rowDataGroup.controls[index] as FormArray;
    if (+ value.currentTarget.value > 0) {
      rowDataArray.get('isOverride').setValue(true);
    }
    else {
      rowDataArray.get('isOverride').setValue(false);
    }
  }

  get refPc() { return this.projectControl.controls; }

  getDate(date: Date) {
    return date;
  }

  showPeriods() {
    if (this.projectControl.controls.startDate.status === 'VALID' && this.projectControl.controls.endingDate.status === 'VALID') {
      this.totalPeriods = this.dateService.getDifferenceInMonths(this.getDate(this.projectControl.value.startDate),
        this.getDate(this.projectControl.value.endingDate));
    } else {
      this.totalPeriods = 0;
    }
  }

  onDeleteClick(index: any, id: number) {
    this.locationIndex = index;
    this.locationId = id;
    this.display = true;
  }

  deleteLocation() {
    const selLocs = this.projectControl.controls.selectedLocations as FormArray;
    this.projectControl.markAsDirty();
    selLocs.removeAt(this.locationIndex);

    const selectedIds: any[] = selLocs.value.map(sl => sl.id);
    const filteredMasterData = this.locations.filter(loc => {
      return selectedIds.includes(loc.value.id);
    }).map(l => l.value);

    if (this.projControl) {
      const projLoc = this.projControl.projectLocationListDTO.find(loc => this.locationId === loc.locationId);
      if (projLoc) {
        projLoc.hoursPerDay = 0;
        projLoc.hoursPerMonth = 0;
        projLoc.isOverride = false;
      }
    }
    this.projectControl.controls.locations.setValue(filteredMasterData);

    if (this.projectControl.controls.locations.value.length < 1) {
      this.projectControl.controls.locations.markAsDirty();
    }
  }

  bindProjectControls() {
    const item = this.holidaysOption.find(val => val.value.code === this.projControl.pipSheetListDTO[0].holidayOption).value;

    // Filter master data with data from api
    const selectedLocationsIDs: number[] = this.projControl.projectLocationListDTO.map(loc => loc.locationId);
    const filteredMasterData: LocationComboItem[] = this.locations.filter(loc =>
      selectedLocationsIDs.includes(loc.value.id)).map(l => l.value);

    this.updateSelectedLocationsForm(filteredMasterData);

    this.projectControl.patchValue({
      startDate: new Date(this.projControl.pipSheetListDTO[0].startDate),
      endingDate: new Date(this.projControl.pipSheetListDTO[0].endDate),
      holidaysOption: item,
      locations: filteredMasterData
    });

    // when milestone data exists
    if (this.sharedData.showProjectMilestone) {
      this.projectMilestoneList = this.getMilestoneWithUId(this.projectMilestoneList);
      const milestoneArray = [];
      let count: number;
      const milestoneList = this.masterMilestoneList.filter(m => {
        return m.milestoneGroupId === this.projControl.pipSheetListDTO[0].milestoneGroupId;
      });
      milestoneList.forEach(x => {
        count = 0;
        this.projControl.projectMilestoneListDTO.forEach(y => {
          if (x.milestoneId === y.milestoneId && (y.milestoneId !== null)) {
            milestoneArray.push(y);
            return;
          }
          count = count + 1;
        });
        if (count === this.projControl.projectMilestoneListDTO.length) {
          milestoneArray.push(x);
        }
      });
      this.projControl.projectMilestoneListDTO.forEach(y => {
        if (y.milestoneId == null) {
          milestoneArray.push(y);
        }
      });

      // select milestone group
      const selectedProjectMilestoneGroup = this.projectMilestoneGroup.find(group => {
        return group.value.id ===
          this.projControl.pipSheetListDTO[0].milestoneGroupId;
      });
      if (selectedProjectMilestoneGroup) {
        this.projectControl.controls.milestone.patchValue({ defaultGroup: selectedProjectMilestoneGroup.value, });
      }
      this.loadMilestoneGridData(milestoneArray);

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
    let pipsheet: IPIPSheet;
    const pipsheets: Array<IPIPSheet> = [];
    let projectLocation: IProjectLocation;
    const projectLocations: Array<IProjectLocation> = [];
    let milestone: IProjectMilestone;
    const milestones: Array<IProjectMilestone> = [];
    const codePerDay = this.selectedLocationsValues.codeperday > 0 ? true : false;
    const codePerMonth = this.selectedLocationsValues.codepermonth > 0 ? true : false;

    let milestoneGroupId = this.projectControl.value.milestone.defaultGroup.id;
    if (!milestoneGroupId) { milestoneGroupId = -1; }

    pipsheet = {
      startDate: new Date(this.projectControl.value.startDate).toLocaleString(),
      endDate: new Date(this.projectControl.value.endingDate).toLocaleString(),
      holidayOption: this.projectControl.value.holidaysOption === '' ? 1 : this.projectControl.value.holidaysOption.code,
      milestoneGroupId: milestoneGroupId === -1 ? null : milestoneGroupId,
      currencyId: this.currencyId,
      projectId: this.projectId,
      versionNumber: 1,
      pipSheetId: this.pipSheetId,
      pipSheetStatusId: 1,
      comments: null,
      createdOn: new Date(),
      updatedOn: new Date(),
      createdBy: 1,
      updatedBy: 1
    };
    pipsheets.push(pipsheet);

    const selLocs = this.projectControl.controls.selectedLocations as FormArray;
    selLocs.value.forEach(control => {
      projectLocation = {
        locationId: control.id,
        hoursPerDay: control.overrideCodePerDay === '' ? control.defaultCodePerDay : +control.overrideCodePerDay,
        hoursPerMonth: control.overrideCodePerMonth === '' ? control.defaultCodePerMonth : +control.overrideCodePerMonth,
        pipSheetId: this.pipSheetId,
        isOverride: codePerDay ? (control.overrideCodePerDay === '' ? false : true) :
          (codePerMonth ? (control.overrideCodePerMonth === '' ? false : true) : false),
        createdBy: 0,
        updatedBy: 0,
        createdOn: new Date(),
        updatedOn: new Date()
      };
      projectLocations.push(projectLocation);
    });
    const milestoneArr: IProjectMilestone[] = [];
    this.projectMilestoneList.forEach(m => {
      milestoneArr.push(m);
    });

    milestoneArr.forEach(milestoneItem => {
      if (milestoneItem.originalValue !== '') {
        milestone = {
          uId: milestoneItem.uId,
          pipSheetId: this.pipSheetId,
          projectMilestoneId: isNullOrUndefined(milestoneItem.projectMilestoneId) ? 0 : milestoneItem.projectMilestoneId,
          milestoneName: milestoneItem.milestoneName,
          milestoneId: milestoneItem.milestoneId,
          milestoneGroupId: milestoneItem.milestoneGroupId,
          isChecked: milestoneItem.isChecked,
          originalValue: null,
          overrideValue: null,
          invoiceAmount: milestoneItem.invoiceAmount,
          milestoneMonth: milestoneItem.milestoneMonth === '' ? null : milestoneItem.milestoneMonth,
          createdBy: 1,
          updatedBy: 1,
          createdOn: new Date(),
          updatedOn: new Date()
        };
        milestones.push(milestone);
      }
    });

    const projectControlData: IProjectControl = {
      pipSheetListDTO: pipsheets,
      projectMilestoneListDTO: milestones,
      projectLocationListDTO: projectLocations,
    };

    this.projectControlService.saveProjectControlData(projectControlData).subscribe(() => {
      this.getHeader1Data();
      this.translateService.get('SuccessMessage.PipSheetSave').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
        this.getOverrideNotificationStatus();
      });
      this.notificationService.notifyFormDirty(false);
    }, () => {
      this.translateService.get('ErrorMessage.PipSheetSave').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
    });
  }

  getHeader1Data(): void {
    let projectDuration = '';
    const startDate = new Date(this.getDate(this.projectControl.value.startDate));
    const endDate = new Date(this.getDate(this.projectControl.value.endingDate));
    const durationStartDate = new Date(startDate.getDate().toString() + ' ' + this.dateService.getMonthName(startDate.getMonth()).toString()
      + ' ' + startDate.getFullYear().toString());
    const durationEndDate = new Date(endDate.getDate().toString() + ' ' + this.dateService.getMonthName(endDate.getMonth()).toString()
      + ' ' + endDate.getFullYear().toString());
    projectDuration = moment(durationStartDate).format('MM-DD-YYYY') + '-' + moment(durationEndDate).format('MM-DD-YYYY');
    this.notificationService.notifyDurationExists(projectDuration);

    this.projectControlService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
      this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
      this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
    });
  }

  // Validations

  validateStartDate(date) {
    this.projectControl.controls['startDate'].markAsDirty();
    this.startDate = date;
    this.selStartDate = null;
    this.selEndDate = this.projectControl.value.endingDate ? this.projectControl.value.endingDate : null;
    if (moment(this.startDate, 'MM-DD-YYYY', true).isValid()) {
      this.validateDate();
    }
    else {
      this.projectControl.controls['startDate'].setErrors({ invalid: true });
      this.startDateValidationMsg = this.translateService.instant('PLANNING.Staff.Control.dateValidation');
    }
    this.notificationService.notifyFormDirty(true);
  }

  validateEndDate(date) {
    this.projectControl.controls['endingDate'].markAsDirty();
    this.selEndDate = null;
    this.endDate = date;
    this.selStartDate = this.projectControl.value.startDate ? this.projectControl.value.startDate : null;
    if (moment(this.endDate, 'MM-DD-YYYY', true).isValid()) {
      this.validateDate();
    }
    else {
      this.projectControl.controls['endingDate'].setErrors({ invalid: true });
      this.endDateValidationMsg = this.translateService.instant('PLANNING.Staff.Control.dateValidation');
    }
    this.notificationService.notifyFormDirty(true);
  }

  validateDate() {
    const startDate = isNullOrUndefined(this.startDate) ? isNullOrUndefined(this.selStartDate) ? '' :
      (((this.selStartDate.getDate()) < 10 ? ('0' + this.selStartDate.getDate()) : this.selStartDate.getDate()) + '-' +
        ((this.selStartDate.getMonth()) < 10 ? ('0' + this.selStartDate.getMonth()) : this.selStartDate.getMonth())
        + '-' + this.selStartDate.getFullYear()).toLocaleString() : (this.startDate.split(''));
    const endDate = isNullOrUndefined(this.endDate) ? isNullOrUndefined(this.selEndDate) ? '' :
      (((this.selEndDate.getDate()) < 10 ? ('0' + this.selEndDate.getDate()) : this.selEndDate.getDate()) + '-' +
        (this.selEndDate.getMonth() < 10 ? '0' + this.selEndDate.getMonth() : this.selEndDate.getMonth()) +
        '-' + this.selEndDate.getFullYear()).toLocaleString() : this.endDate.split('');
    const startDay = isNullOrUndefined(this.startDate) ? isNullOrUndefined(this.selStartDate) ? ''
      : (this.selStartDate.getDate()) : +(this.startDate[3] + this.startDate[4]);
    const endDay = isNullOrUndefined(this.endDate) ? isNullOrUndefined(this.selEndDate) ? ''
      : (this.selEndDate.getDate()) : +(this.endDate[3] + this.endDate[4]);
    const startMonth = isNullOrUndefined(this.startDate) ? isNullOrUndefined(this.selStartDate) ? ''
      : (this.selStartDate.getMonth() + 1) : +(this.startDate[0] + this.startDate[1]);
    const endMonth = isNullOrUndefined(this.endDate) ? isNullOrUndefined(this.selEndDate) ? ''
      : (this.selEndDate.getMonth() + 1) : +(this.endDate[0] + this.endDate[1]);
    let startYear = isNullOrUndefined(this.startDate) ? '' : +(this.startDate[6] +
      this.startDate[7] + this.startDate[8] + this.startDate[9]);
    let endYear = isNullOrUndefined(this.endDate) ? '' : +(this.endDate[6] + this.endDate[7] + this.endDate[8] + this.endDate[9]);
    const getStartYear = isNullOrUndefined(this.selStartDate) ? '' : this.selStartDate.getFullYear();
    const getEndYear = isNullOrUndefined(this.selEndDate) ? '' : this.selEndDate.getFullYear();

    if (startDate.length === 10 && endDate.length === 10) {
      if (startYear === '' ? startYear = getStartYear : startYear && endYear === '' ? endYear = getEndYear : endYear) {
        if ((startYear > endYear || (startYear === endYear && startMonth > endMonth) || startMonth > 12 || endMonth > 12)
          || (startDay > endDay && startMonth === endMonth && startYear === endYear)) {
          this.projectControl.controls['endingDate'].markAsDirty();
          this.projectControl.controls['startDate'].markAsDirty();

          this.projectControl.controls['startDate'].setErrors({ invalid: true });
          this.projectControl.controls['endingDate'].setErrors({ invalid: true });

          this.dateToolTip = this.translateService.instant('MESSAGES.Tooltip.ProjectControlStartDateError');
        }
        else {
          this.projectControl.controls['startDate'].setErrors(null);
          this.projectControl.controls['endingDate'].setErrors(null);
          this.dateToolTip = '';
        }
      }
      else {
        if (moment(this.startDate, 'MM-YYYY', true).isValid()) {
          if (this.startDate) {
            this.projectControl.controls['startDate'].setValue(this.startDate);
          }
        }
        else if (moment(this.endDate, 'MM-YYYY', true).isValid()) {
          if (this.endDate) {
            this.projectControl.controls['endingDate'].setValue(this.endDate);
          }
        }
      }
    }
    if (startDate || endDate) {
      if (startDate.length === 10 && (startYear >= 2018 && startYear <= 2050)) {
        this.startDateValidationMsg = '';
      }
      else if (startYear && startYear < 2018 || startYear > 2050) {
        this.projectControl.controls['startDate'].setErrors({ invalid: true });
        this.startDateValidationMsg = this.translateService.instant('MESSAGES.Tooltip.ReportStartDateRange');
      }
      if (endDate.length === 10 && (endYear >= 2018 && endYear <= 2050)) {
        this.endDateValidationMsg = '';
      }
      else if (endYear && (endYear < 2018 || endYear > 2050)) {
        this.projectControl.controls['endingDate'].setErrors({ invalid: true });
        this.endDateValidationMsg = this.translateService.instant('MESSAGES.Tooltip.ReportEndDateRange');
      }
    }
    this.showPeriods();
  }

  onStartDateSelect(date) {
    this.startDate = null;
    this.selStartDate = date;
    if (this.selStartDate.getFullYear() < 2018 || this.selStartDate.getFullYear() > 2050) {
      this.selStartDate.setFullYear(2018);
    }
    this.startDateValidationMsg = '';
    this.validateSelectedDate();
  }

  onEndDateSelect(date) {
    this.endDate = null;
    this.selEndDate = date;
    if (this.selEndDate.getFullYear() < 2018 || this.selEndDate.getFullYear() > 2050) {
      this.selEndDate.setFullYear(2018);
    }
    this.endDateValidationMsg = '';
    this.validateSelectedDate();
  }

  validateSelectedDate() {
    const selStartDate = isNullOrUndefined(this.selStartDate) ? isNullOrUndefined(this.startDate) ? undefined
      : moment(this.startDate, 'MM/DD/YYYY') : this.selStartDate;
    const selEndDate = isNullOrUndefined(this.selEndDate) ? isNullOrUndefined(this.endDate) ? undefined
      : moment(this.endDate, 'MM/DD/YYYY') : this.selEndDate;
    if (selStartDate && selEndDate) {
      if (selStartDate > selEndDate) {
        this.projectControl.controls['endingDate'].markAsDirty();
        this.projectControl.controls['startDate'].markAsDirty();

        this.projectControl.controls['startDate'].setErrors({ invalid: true });
        this.projectControl.controls['endingDate'].setErrors({ invalid: true });
        this.dateToolTip = this.translateService.instant('MESSAGES.Tooltip.ProjectControlStartDateError');
      }
      else {
        if (selStartDate && selEndDate) {
          if (moment(selStartDate).year() >= 2018 && moment(selStartDate).year() <= 2050) {
            this.projectControl.controls['startDate'].setErrors(null);
          }
          this.projectControl.controls['endingDate'].setErrors(null);
        }
        else {
          this.projectControl.setErrors({ invalid: true });
        }
        this.dateToolTip = '';
      }
    }
    this.showPeriods();
  }

  holidayOptionsError() {
    if (this.projectControl.get('holidaysOption').invalid && this.projectControl.get('holidaysOption').dirty) {
      this.translateService.get('MESSAGES.Tooltip.RequiredDropDownMessage').subscribe((resource) => {
        this.pShowToolTip = resource['message'];
      });
    } else {
      this.pShowToolTip = '';
    }
  }

  locationsError() {
    this.pLocationShowToolTip = '';
    if (this.projectControl.get('locations').invalid && this.projectControl.get('locations').dirty) {
      this.translateService.get('MESSAGES.Tooltip.RequiredDropDownMessage').subscribe((resource) => {
        this.pLocationShowToolTip = resource['message'];
      });
    }
  }

  locationCodePerMonthError(codePerMonth) {
    this.pShowToolTip = '';
    if ((this.overrideLimits.OverrideHoursPerMonthMinLimit > codePerMonth ||
      this.overrideLimits.OverrideHoursPerMonthMaxLimit < codePerMonth) && codePerMonth !== '') { //
      this.translateService.get('MESSAGES.Tooltip.LocationHoursPerMonthMessage').subscribe((resource) => {
        this.pShowToolTip = resource['message'];
      });
    }
  }

  locationCodePerDayError(codePerDay) {
    this.pShowToolTip = '';
    if ((this.overrideLimits.OverrideHoursPerDayMinLimit > codePerDay ||
      this.overrideLimits.OverrideHoursPerDayMaxLimit < codePerDay) && codePerDay !== '') {
      this.translateService.get('MESSAGES.Tooltip.LocationHoursPerDayMessage').subscribe((resource) => {
        this.pShowToolTip = resource['message'];
      });
    }
  }

  showProjectMilestone(): boolean {
    return this.sharedData.showProjectMilestone;
  }

  showMilestoneGrid() {
    const milestoneGroupId = this.projectControl.value.milestone.defaultGroup ? this.projectControl.value.milestone.defaultGroup.id : -1;
    this.milestoneGroupId = milestoneGroupId;
    return (milestoneGroupId && milestoneGroupId !== -1);
  }

  addMilestoneRow(addedRowId: number) {
    const milestoneArray = this.projectControl.get('milestone').get('milestones') as FormArray;
    milestoneArray.push(this.fb.group({
      milestoneId: [addedRowId, []],
      originalValue: ['', []],
      overrideValue: ['', [Validators.maxLength(100)]],
      previousValue: [''],
      invoiceAmount: [''],
      milestoneMonth: ['', [Validators.pattern(Constants.regExType.milestoneMonthFormat)]],
      projectMilestoneId: 0
    }));
  }

  projectMilestoneGroupChange(value) {
    const milestoneList = this.projectMilestoneList.filter(m => {
      return m.milestoneGroupId === value.id;
    });
    this.selectedMilestones = [];
    this.loadMilestoneGridData(milestoneList);
  }

  loadMilestoneGridData(milestoneList: any[]) {
    const milestoneGroup = this.projectControl.controls.milestone as FormGroup;
    const milestonesArr = milestoneGroup.controls.milestones as FormArray;

    // clear all items from grid
    while (milestonesArr.length > 0) {
      milestonesArr.removeAt(0);
    }

    milestoneList = milestoneList.filter(x => x.milestoneGroupId === this.projectControl.value.milestone.defaultGroup.id);

    milestoneList.forEach((milestone) => {
      const milestoneFormGroup = this.fb.group({
        milestoneId: [milestone.milestoneId, []],
        originalValue: [milestone.milestoneName, []],
        overrideValue: ['', [Validators.maxLength(100)]],
        previousValue: [''],
        invoiceAmount: [milestone.invoiceAmount !== 0 ? milestone.invoiceAmount : null],
        milestoneMonth: [milestone.milestoneMonth, [Validators.pattern(Constants.regExType.milestoneMonthFormat)]],
        isChecked: [milestone.isChecked],
        projectMilestoneId: milestone.projectMilestoneId
      });
      if (milestone.isChecked === true) {
        this.selectedMilestones.push(milestoneFormGroup);
      }
      milestonesArr.push(milestoneFormGroup);
    });

    if (milestoneList.length > 0) {
      milestonesArr.controls.forEach(c => {
        const masterMilestone = this.masterMilestoneList.find(m => c.value.milestoneId === m.milestoneId);
        if (masterMilestone && masterMilestone.milestoneName !== c.value.originalValue) { // custom overrides available
          c.patchValue({ overrideValue: c.value.originalValue });
        }
      });
    }
    const masterMilestoneList = this.masterMilestoneList.filter(m => {
      return m.milestoneGroupId === milestoneGroup.value.defaultGroup.id;
    });

    // minimum no of rows to be display on milestone grid is 12
    const noOfEmptyRowsToBeAdded = (MINIMUM_NO_OF_MILESTONE_GRID_ROWS - milestonesArr.length);
    const customMilestoneGroupId = Constants.milestoneGroupId;
    if (milestoneGroup.value.defaultGroup.id === customMilestoneGroupId) {   // For Milestone Group = CUSTOM
      this.lastMasterMilestoneId = 0;
    }
    else if (milestoneGroup.value.defaultGroup.id === -1 || milestoneGroup.value.defaultGroup === '') {
      this.lastMasterMilestoneId = null;
    }
    else {
      this.lastMasterMilestoneId = milestonesArr.value[masterMilestoneList.length - 1].milestoneId + 1;
    }

    let addedRowId = this.lastMasterMilestoneId;
    for (let i = 0; i < noOfEmptyRowsToBeAdded; i++) {
      this.addMilestoneRow(addedRowId);
      addedRowId++;
    }
  }

  updateOverrideValue(milestone, index: number) {
    const milestoneGroup = this.projectControl.controls.milestone as FormGroup;
    const milestonesArr = milestoneGroup.controls.milestones as FormArray;
    const overrideValue: string = milestone.overrideValue.trim();

    let updatedOriginalValue = overrideValue;

    if (overrideValue === '') {
      const masterMilestone =
        this.masterMilestoneList.find(m => milestone.milestoneId === m.milestoneId && m.milestoneGroupId === this.milestoneGroupId);
      if (masterMilestone) { updatedOriginalValue = masterMilestone.milestoneName; }
    }

    if (milestonesArr.controls[index].valid) {
      milestonesArr.controls[index].patchValue({
        originalValue: updatedOriginalValue,
        overrideValue: overrideValue,
        previousValue: milestone.originalValue
      });
    }
  }

  customOverrideKeyUp() {
    const milestoneGroup = this.projectControl.controls.milestone as FormGroup;
    const milestonesArr = milestoneGroup.controls.milestones as FormArray;

    const existingOverrideValueList: string[] = milestonesArr.value.map(m => m.overrideValue);
    milestonesArr.controls.forEach((milestoneCtrl: FormGroup) => {
      if (milestoneCtrl.value.overrideValue !== ''
        && existingOverrideValueList.filter(m => m === milestoneCtrl.value.overrideValue).length > 1) {
        this.translateService.get('ErrorMessage.DuplicateOverrideValue').subscribe(msg => {
          milestoneCtrl.controls.overrideValue.setErrors({
            duplicateOverridenValue: { message: msg }
          });
        });
      } else {
        // delete single error type
        milestoneCtrl.controls.overrideValue.setErrors({ 'duplicateOverridenValue': null });
        milestoneCtrl.controls.overrideValue.updateValueAndValidity();
      }
    });
  }

  getOverrideValueError(overrideControl: FormControl): string {

    if (overrideControl.getError('duplicateOverridenValue')) {
      return overrideControl.getError('duplicateOverridenValue').message;
    }
    else if (overrideControl.getError('maxlength')) {
      return 'Maximum ' + overrideControl.getError('maxlength').requiredLength + ' characters are allowed';
    }
    else if (overrideControl.getError('pattern')) {
      this.translateService.get('ErrorMessage.AlphanumericValue').subscribe(msg => {
        return msg;
      });
    }
    return '';
  }

  onMilestoneOverride(milestone: any) {
    const customMilestoneGroupId = Constants.milestoneGroupId;     // Custom Milestone Group
    const milestoneGroupId = this.projectControl.value.milestone.defaultGroup.id;
    const milestones = this.masterMilestoneList.filter(m => m.milestoneGroupId === milestoneGroupId).sort(x => x.milestoneId);
    const highestMilestoneId = milestoneGroupId !== customMilestoneGroupId ? milestones[milestones.length - 1].milestoneId : -1;
    const lowestMilestoneId = milestoneGroupId !== customMilestoneGroupId ? milestones[0].milestoneId : -1;
    this.isMilestoneDeleted = false;
    this.isCustomMilestone = false;
    if (milestone.originalValue === '') {         // for all the custom(null) milestones added
      this.isCustomMilestone = true;
    }
    if (milestone.milestoneId >= lowestMilestoneId && milestone.milestoneId <= highestMilestoneId) {
      if (milestoneGroupId !== customMilestoneGroupId && this.isMilestoneDeleted === false) {  // to update existing master milestone value
        this.projectMilestoneList[milestone.milestoneId - 1].milestoneName = milestone.originalValue;
        this.projectMilestoneList[milestone.milestoneId - 1].invoiceAmount = milestone.invoiceAmount;
        this.projectMilestoneList[milestone.milestoneId - 1].milestoneMonth = milestone.milestoneMonth;
      }
    }
    else if (milestoneGroupId === customMilestoneGroupId && milestone.previousValue !== '' && this.isCustomMilestone === false) {
      const uId = this.projectMilestoneList.find(x => x.milestoneName === milestone.previousValue // to update Custom milestone Group
        && x.milestoneGroupId === milestoneGroupId).uId;
      this.projectMilestoneList[uId].milestoneName = milestone.originalValue;
      this.projectMilestoneList[uId].invoiceAmount = milestone.invoiceAmount;
      this.projectMilestoneList[uId].milestoneMonth = milestone.milestoneMonth;
    }
    else {
      if (milestone.milestoneId > highestMilestoneId && !this.isInvoiceAmount) {   // to update null milestone value
        if (!this.isCustomMilestone) {
          milestone.milestoneId = null;
          const milestoneToBeSaved: IProjectMilestone = {
            uId: 0,
            milestoneId: null,
            milestoneGroupId: milestoneGroupId,
            milestoneName: milestone.originalValue,
            pipSheetId: this.pipSheetId,
            projectMilestoneId: 0,
            isChecked: false,
            originalValue: null,
            overrideValue: null,
            invoiceAmount: milestone.originalValue !== null ? milestone.invoiceAmount : null,
            milestoneMonth: milestone.originalValue !== '' ? milestone.milestoneMonth : null,
            createdBy: 1,
            updatedBy: 1,
            createdOn: new Date(),
            updatedOn: new Date()
          };
          this.projectMilestoneList.push(milestoneToBeSaved);
          this.projectMilestoneList = this.getMilestoneWithUId(this.projectMilestoneList);
        }
      }
      else if (this.isInvoiceAmount) {        // for invoice amount and milestone month
        this.isInvoiceAmount = false;
        if (milestone.originalValue !== '') {
          const uId = this.projectMilestoneList.find(x => x.milestoneName === milestone.originalValue
            && x.milestoneGroupId === milestoneGroupId).uId;
          this.projectMilestoneList[uId].invoiceAmount = milestone.invoiceAmount;
          this.projectMilestoneList[uId].milestoneMonth = milestone.milestoneMonth;
        }
      }
      else {
        if (!this.isInvoiceAmount) {
          const uId = this.projectMilestoneList.find(x => x.milestoneName === milestone.previousValue
            && x.milestoneGroupId === milestoneGroupId).uId;
          if (uId) {
            if (milestone.originalValue === '') {   // to delete blank milestone
              this.isMilestoneDeleted = true;
              this.projectMilestoneList.splice(uId, 1);
              this.projectMilestoneList = this.getMilestoneWithUId(this.projectMilestoneList);
            }
            else {
              this.projectMilestoneList[uId].milestoneName = milestone.originalValue;
            }
          }
        }
      }
    }
    if (milestoneGroupId === customMilestoneGroupId && this.isCustomMilestone) { // to delete blank custom milestone group milestone
      const uId = this.projectMilestoneList.find(x => x.milestoneName === milestone.previousValue
        && x.milestoneGroupId === milestoneGroupId).uId;
      if (uId) {
        if (milestone.originalValue === '') {
          this.isMilestoneDeleted = true;
          this.projectMilestoneList.splice(uId, 1);
          this.projectMilestoneList = this.getMilestoneWithUId(this.projectMilestoneList);
        }
        else {
          this.projectMilestoneList[uId].milestoneName = milestone.originalValue;
        }

      }
    }
  }
  onCheckboxSelect(milestone) {
    this.notificationService.notifyFormDirty(true);
    const customMilestoneGroupId = Constants.milestoneGroupId;                 // Custom Milestone Group
    const milestoneGroupId = this.projectControl.value.milestone.defaultGroup.id;
    const milestones = this.masterMilestoneList.filter(m => m.milestoneGroupId === milestoneGroupId).sort(x => x.milestoneId);
    const highestMilestoneId = milestoneGroupId !== customMilestoneGroupId ? milestones[milestones.length - 1].milestoneId : -1;
    const lowestMilestoneId = milestoneGroupId !== customMilestoneGroupId ? milestones[0].milestoneId : -1;
    if (milestone.milestoneId >= lowestMilestoneId && milestone.milestoneId <= highestMilestoneId) {
      if (this.projectMilestoneList[milestone.milestoneId - 1].isChecked === true) {
        this.projectMilestoneList[milestone.milestoneId - 1].isChecked = false;
      }
      else {
        this.projectMilestoneList[milestone.milestoneId - 1].isChecked = true;
      }
    }
    else {
      if (milestone.originalValue !== '') {
        const uId = this.projectMilestoneList.find(x => x.milestoneName === milestone.originalValue
          && x.milestoneGroupId === milestoneGroupId).uId;
        if (uId) {
          if (this.projectMilestoneList[uId].isChecked === true) {
            this.projectMilestoneList[uId].isChecked = false;
          }
          else {
            this.projectMilestoneList[uId].isChecked = true;
          }
        }
      }
    }
  }
  onSelectAllCheckbox() {
    this.notificationService.notifyFormDirty(true);
    const milestoneGroupId = this.projectControl.value.milestone.defaultGroup.id;
    const milestoneList = this.projectMilestoneList.filter(m => m.milestoneGroupId === milestoneGroupId);
    const selectedMilestonesLength = this.selectedMilestones.filter(m => m.value.isChecked === false).length;
    if (selectedMilestonesLength > 0 || this.selectedMilestones.length > milestoneList.length) {
      milestoneList.forEach(m => {
        return m.isChecked = true;
      });
    }
    else {
      milestoneList.forEach(m => {
        return m.isChecked = false;
      });
    }
  }

  getMilestoneWithUId(milestones: IProjectMilestone[]): IProjectMilestone[] {
    const milestonesWithUId: IProjectMilestone[] = [];
    milestones.forEach((m, index) => {
      m.uId = index;
      milestonesWithUId.push(m);
    });
    return milestonesWithUId;
  }

  onInvoiceAmountOrMonthChange(milestone) {
    this.isInvoiceAmount = true;
    this.onMilestoneOverride(milestone);
  }
}


