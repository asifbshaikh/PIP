import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { CommonModule } from '@angular/common';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { IRoleAndAccount } from './../../../../shared-module/domain/IRoleAndAccount';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { Component, Input, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { UtilityService } from '@core/infrastructure/utility.service';
import { ValidationService } from '@core/infrastructure/validation.service';
import { DateService } from '@core/services/date.service';
import { SharedDataService } from '@global';
import { TranslateService } from '@ngx-translate/core';
import {
  IResourceUSTRole, MonthlyData, ProjectPeriod, Resource, ResourcePlanMasterData,
  ResourcePlanning, ResourcePlanningMasterData
} from '@shared';
import { Constants } from '@shared/infrastructure/constants';
import { Mastermapper } from '@shared/mapper/master/mastermapper';
import { ResourceMapper } from '@shared/mapper/master/resourcemapper';
import { ResourcePlanningService } from '@shared/services/resource-planning.service';
import { MessageService, SelectItem, MenuItem } from 'primeng/api';
import { NotificationService } from '@global';
import { TableHeaderCheckbox } from 'primeng/table';
import { isNullOrUndefined } from 'util';
import { OverrideNotificationStatus } from '@shared/domain/override-notification-status';
@Component({
  selector: 'app-resource-planning',
  templateUrl: './resource-planning.component.html'
})
export class ResourcePlanningComponent implements OnInit, AfterViewInit {

  @Input() location: SelectItem[] = [];
  @Input() optionalPhase: SelectItem[] = [];

  pipSheetId: number;
  projectId: number;
  dashboardId: number;
  cols: any[] = [];
  frozenCols: any[] = [];
  totalPeriods: any;
  periodCols: any[] = [];
  resourcePlanningList: Resource[] = [];
  resourcePlanning: ResourcePlanning;
  roleGroup: SelectItem[] = [];
  ustRole: SelectItem[] = [];
  technology: SelectItem[] = [];
  contractorMarkup: SelectItem[] = [];
  resourceLocations: SelectItem[];
  resourceHolidays: SelectItem[];
  resourceRoleGroup: SelectItem[] = [];
  resourceUSTRole: SelectItem[] = [];
  resourceMarkup: SelectItem[];
  resourceOptionalPhase: SelectItem[];
  resourcePlanningForm: FormGroup;
  selectedResources: any[] = [];
  filteredUSTRole: any[] = [];
  filteredRoleGroups: any[] = [];
  sResourceUSTRole: IResourceUSTRole;
  selectedRoleGroup: SelectItem[] = [];
  selectedUSTRole: SelectItem[];
  USTGrade: any;
  decimalPrecisionTwo: {};
  oldRole: string;
  totalFTEValue: number[] = [];
  monthlyData: MonthlyData[] = [];
  resourceMasterData: ResourcePlanningMasterData;
  projectDurationInDays: number;
  totalDaysInProjectDurationMonths: number;
  resourceBillingType: any;
  totalOfTotalHours: string;
  totalCostHours = 0;
  totalCostHourNormalizedFTEValue: number[] = [];
  colSpanSize = 0;
  showfooter = false;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  disableOtherFormControls = false;
  isInvalid: boolean;
  isSaveClicked = false;

  // new
  contextMenuItems: MenuItem[];
  isCellActive = false;
  currentCellIndex = -1;
  currentCellValue = -1;
  currentRowIndex = -1;
  currentRowData: any;

  constructor(
    private fb: FormBuilder,
    private translateService: TranslateService,
    private resourcePlanningService: ResourcePlanningService,
    private validate: ValidationService,
    private route: ActivatedRoute,
    private sharedData: SharedDataService,
    private dateService: DateService,
    private messageService: MessageService,
    private utilityService: UtilityService,
    private notificationService: NotificationService,
    private userWorkflowService: UserWorkflowService
  ) {
    this.route.paramMap.subscribe(
      params => {
        this.pipSheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
        this.projectId = parseInt(params.get(Constants.uiRoutes.routeParams.projectId), 10);
        this.dashboardId = parseInt(params.get(Constants.uiRoutes.routeParams.dashboardId), 10);
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

    this.decimalPrecisionTwo = Constants.regExType.decimalPrecisionTwo;

    this.translateService.get('ResourcePlanning.columns').subscribe(columns => {
      this.cols = columns;
    });

    this.translateService.get('ResourcePlanning.frozenColumns').subscribe(frozenColumns => {
      this.frozenCols = frozenColumns;
    });

    this.translateService.get('ResourcePlanning.totalPeriods').subscribe(periods => {
      this.totalPeriods = periods;
    });

    this.translateService.get('ResourcePlanning.technology').subscribe(tech => {
      this.technology = tech;
    });

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });

    this.resourcePlanningForm = this.fb.group({
      resources: this.fb.array([])
    });

    this.resourcePlanningService.getResourceLocations(this.pipSheetId).subscribe(data => {
      this.resourceLocations = new Mastermapper().getLocationComboItems(data, true);

      this.resourcePlanningService.getResourceOptionalPhase(this.pipSheetId).subscribe(phase => {
        this.resourceOptionalPhase = new Mastermapper().getOptionalPhaseComboItems(phase, true);

        this.initalizeResourcePlanningData();
      });
    });
    this.resourceRoleGroup = new Mastermapper().getResourceRoleGroupComboItems(this.sharedData.sharedData.resourceGroupDTO);
    this.resourceUSTRole = new Mastermapper().getResourceUSTRoleComboItems(this.sharedData.sharedData.resourceDTO);
    this.resourceMarkup = new Mastermapper().getResourceMarkupComboItems(this.sharedData.sharedData.markupDTO);
    this.resourceHolidays = new Mastermapper().getResourceHolidayComboItems(this.sharedData.sharedData.holidayDTO);
    this.resourcePlanningService.getResourceOptionalPhase(this.pipSheetId).subscribe(data => {
      this.resourceOptionalPhase = new Mastermapper().getOptionalPhaseComboItems(data, true);
    });
    this.makeRowsSameHeight();
  }

  ngAfterViewInit() {
    this.resourcePlanningForm.valueChanges.subscribe(() => {
      if (this.resourcePlanningForm.dirty) {
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
      this.disableOtherFormControls = true;
      setTimeout(() => {
        this.resourcePlanningForm.disable();
      }, 200);
    }
  }

  preCalculate(resourcePlanMasterData: ResourcePlanMasterData[]) {
    const resMasterData = resourcePlanMasterData[0];
    this.resourceMasterData = {
      startDate: new Date(resMasterData.startDate),
      endDate: new Date(resMasterData.endDate),
      sfBillingType: this.sharedData.sharedData.projectBillingTypeDTO.filter(x =>
        x.projectBillingTypeId === resMasterData.projectBillingTypeID)[0].billingTypeName,
      isHolidayOptionOn: resMasterData.holidayOption
    };

    this.monthlyData = this.resourcePlanningService.getMonthlyData(this.resourceLocations, this.resourceHolidays,
      this.resourceMasterData.startDate, this.resourceMasterData.endDate);
    this.projectDurationInDays =
      this.dateService.getTotalDaysBetweenDates(this.resourceMasterData.startDate, this.resourceMasterData.endDate);
    this.totalDaysInProjectDurationMonths =
      this.dateService.getTotalDaysInMonths(this.resourceMasterData.startDate, this.resourceMasterData.endDate);
  }

  // build resource form array depend on data
  buildResourcesForm() {
    const resourceForm = this.fb.array([]);
    this.resourcePlanningList.forEach((data) => {
      resourceForm.push(this.getResourceForm(data));
    });
    this.resourcePlanningForm.setControl('resources', resourceForm);
  }

  // build and give single resource form
  getResourceForm(data: Resource): FormGroup {
    const periods: FormArray = this.fb.array([]);

    let location = this.resourceLocations.find(loc => loc.value.id === data.locationId);
    if (!location) { location = Constants.selectComboItem; }
    this.onResourceLocationChanged(location.value, data.uId);
    const roleGroup = this.filteredRoleGroups[data.uId].find(rg => rg.value.id === data.resourceGroupId);
    let ustRole = Constants.selectComboItem;
    let USTGrade = null;
    let oldRole = null;

    if (roleGroup && roleGroup.value.id !== -1) {
      this.onResourceRoleGroupChange(roleGroup.value, data.uId);
      const selectedUSTRole = this.filteredUSTRole[data.uId].find(usrole => usrole.value.id === data.resourceId);
      if (selectedUSTRole) {
        ustRole = selectedUSTRole;
        const selectedGrade = this.sharedData.sharedData.resourceDTO.find(item => item.resourceId === ustRole.value.id);
        USTGrade = selectedGrade && selectedGrade.grade ? selectedGrade.grade : '';
        oldRole = selectedGrade && selectedGrade.oldName ? selectedGrade.oldName : '';
      }
    }

    const phase = this.resourceOptionalPhase.find(ph => ph.value.id === data.milestoneId);
    const markup = this.resourceMarkup.find(mu => mu.value.id === data.markupId);
    let flagValue = '';
    if (markup && markup.value.name) {
      const value = markup.value.name.toLowerCase().includes('contract');
      flagValue = value ? Constants.resourceContractorFlag : Constants.resourcePermanentFlag;
    }
    for (let i = 0; i < this.periodCols.length; i++) {
      periods.push(this.fb.group({
        FTEValue: [data.projectPeriod[i].fteValue, []],
        TotalHours: [data.projectPeriod[i].totalHours, []],
        costHours: [data.projectPeriod[i].costHours, []]
      }));
    }

    return this.fb.group({
      id: [data.id, []], // this id is for primeng table
      uId: [data.uId, []],
      alias: [data.alias, []],
      location: [location ? location.value : '', [Validators.required, this.validate.validateDeselectedDropdown]],
      roleGroup: [roleGroup.value, [Validators.required, this.validate.validateDeselectedDropdown]],
      ustRole: [ustRole.value, [Validators.required, this.validate.validateDeselectedDropdown]],
      technology: [data.technologyId, []],
      utilization: [data.utilizationType, [Validators.required]],
      phase: [phase ? phase.value : '', []],
      contractorFlag: [flagValue, []],
      contractorMarkup: [markup.value, []],
      totalHours: [data.totalhoursPerResource, []],
      totalCostHours: [data.costHrsPerResource, []],
      grade: [USTGrade, []],
      oldRole: [oldRole],
      clientRole: [data.clientRole, [Validators.maxLength(100)]],
      totalFTE: [data.totalFTE, []],
      isDeleted: [data.isDeleted, []],
      periods: periods,
    },
      { validator: this.requiredFieldsValitions });
  }

  addResource(newResource: Resource) {
    const newResourceForm = this.getResourceForm(newResource);
    newResourceForm.patchValue(newResource);
    this.resourcePlanningForm.markAsDirty();
    (this.resourcePlanningForm.get('resources') as FormArray).push(newResourceForm);
  }


  onAddRow() {
    let newResourceData: Resource;
    if (this.resourcePlanningList.length === 0) {
      const uId = 0;
      newResourceData = this.resourcePlanningService.getDataForNewResource(this.pipSheetId, uId,
        null, this.resourcePlanning.projectPeriods);
    } else {
      const uId = this.resourcePlanningList[this.resourcePlanningList.length - 1].uId + 1;
      newResourceData = this.resourcePlanningService.getDataForNewResource(this.pipSheetId, uId,
        this.resourcePlanningList[0].projectPeriod, null);
    }
    this.resourcePlanningList.push(newResourceData);
    this.addResource(newResourceData);
  }

  refreshSelectedRows() {
    const resourcesList = this.resourcePlanningForm.get('resources') as FormArray;
    this.selectedResources.forEach(res => {
      const formResource = resourcesList.value.find(row => row.uId === res.uId);
      res.alias = formResource.alias;
      res.phase = formResource.phase;
      res.location = formResource.location;
      res.roleGroup = formResource.roleGroup;
      res.ustRole = formResource.ustRole;
      res.grade = formResource.grade;
      res.clientRole = formResource.clientRole;
      res.contractorMarkup = formResource.contractorMarkup;
      res.contractorFlag = formResource.contractorFlag;
      res.totalHours = formResource.totalHours;
      res.totalFTE = formResource.totalFTE;
      res.periods = formResource.periods;
      res.totalhoursPerResource = formResource.totalhoursPerResource;
      res.costHrsPerResource = formResource.costHrsPerResource;
    });
  }
  onCopySelected() {
    const copiedData = this.selectedResources;
    this.refreshSelectedRows();
    // this.selectedResources = [];
    // find max uid
    const uId = this.resourcePlanningList[this.resourcePlanningList.length - 1].uId + 1;
    // sort copied rows ascending order of uid
    copiedData.sort(this.utilityService.compareValues('uId', 'asc'));
    const copyResourceData: Resource[] = this.resourcePlanningService.composeCopyData(copiedData, uId,
      this.resourcePlanningList[0].projectPeriod, this.pipSheetId);
    copyResourceData.forEach(resource => {
      this.resourcePlanningList.push(resource);
      this.addResource(resource);
    });
    this.displayFTEValueAndTotalCostHours();
    this.displayTotalHours();
    this.selectedResources = [];
  }

  initalizeResourcePlanningData() {
    this.resourcePlanningService.GetResourcePlanningData(this.pipSheetId).subscribe(data => {
      this.resourcePlanning = data;
      if (!isNullOrUndefined(this.resourcePlanning)) {
        this.getPeriods(this.resourcePlanning.projectPeriods);

        let resourceListData: Resource[];
        if (this.resourcePlanning.resources.length > 0) {
          // data exists again current pipsheet id;
          resourceListData = this.resourcePlanning.resources;
        } else {
          // data doesnt exists against pipsheet id.. new pipsheet creation is in progress. hence read dummy data
          resourceListData = this.resourcePlanningService.getDummyData();
        }

        this.resourcePlanningList = new ResourceMapper(this.dateService)
          .formulateResourceModel(resourceListData, this.resourcePlanning.projectPeriods);
        this.buildResourcesForm();
        this.resourcePlanningForm.patchValue({ resources: this.resourcePlanningList });

        if (this.resourcePlanning.resourcePlanMasterData.length > 0) {
          this.preCalculate(this.resourcePlanning.resourcePlanMasterData);
        }
        const resourceGroup = this.getResourcesControls();
        if (resourceGroup.length > 0) {
          resourceGroup.forEach((rowDatacontrol, index) => {
            this.calculateFTE(rowDatacontrol, index);
          });
        }
        this.displayFTEValueAndTotalCostHours();
        this.displayTotalHours();
        this.roleAndAccount = this.sharedData.roleAndAccount;
        this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
        if (this.roleAndAccount != null) {
          this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
        }
      }
      if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
        this.pipSheetWorkflowStatus = this.sharedData.sharedData.pipSheetWorkflowStatus;
        this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
        this.isDataAvailable = true;
        this.enableDisableForm();
      }
      else {
        this.isDataAvailable = true;
      }
    });
  }

  onHeaderCheckChange(headerCheckbox: TableHeaderCheckbox) {
    if (headerCheckbox.checked) { // select all
      this.selectedResources = this.resourcePlanningForm.controls.resources.value;
    } else {  // unselectall
      this.selectedResources = [];
    }
  }

  onDeleteRow() {
    this.resourcePlanningForm.markAsDirty();
    const resourcesControlList = this.resourcePlanningForm.get('resources') as FormArray;
    if (this.selectedResources.length > 0) {
      for (let i = 0; i < this.selectedResources.length; i++) {
        const resourceDeleteIndex = resourcesControlList.value.findIndex(x => x.uId === this.selectedResources[i].uId);
        resourcesControlList.removeAt(resourceDeleteIndex);
        this.filteredUSTRole.splice(resourceDeleteIndex, 1);
      }
      // Empty selected resources
      this.selectedResources = [];
    }

    this.displayFTEValueAndTotalCostHours();
    this.displayTotalHours();
  }

  onResourceRoleGroupChange(data: any, rowIndex) {
    const resourceGroup = this.getResourcesControls();
    const resourceFormGroup = resourceGroup[rowIndex] as FormGroup;
    this.filteredUSTRole[rowIndex] = this.resourceUSTRole.filter(x => x.value.code === data.id);
    if (this.resourceUSTRole) {
      // add select item to UST Role
      this.filteredUSTRole[rowIndex].unshift({
        label: '--- select ---',
        value: {
          id: -1,
          code: '--select---',
          name: 'select'
        }
      });
      // to reset the grade
      if (data.id === -1) {
        resourceFormGroup.controls.grade.setValue(this.USTGrade = null);
        resourceFormGroup.controls.oldRole.setValue(this.oldRole = null);
      }
      if (resourceFormGroup) {
        if (!this.filteredUSTRole[rowIndex].map(g => g.value).includes(resourceFormGroup.controls.ustRole.value)) {
          resourceFormGroup.controls.ustRole.setValue(null);
          resourceFormGroup.controls.grade.setValue(this.USTGrade = null);
          resourceFormGroup.controls.oldRole.setValue(this.oldRole = null);
        }
        // this.onUSTRoleChange(id, rowIndex);
      }

    }
  }

  onUSTRoleChange(id: number, rowIndex) {
    const resourceGroup = this.getResourcesControls();
    const resourceFormGroup = resourceGroup[rowIndex] as FormGroup;

    if (id === -1) {
      resourceFormGroup.controls.grade.setValue(this.USTGrade = null);
      resourceFormGroup.controls.oldRole.setValue(this.oldRole = null);
      // this.oldRole = '';
    }
    else {
      this.USTGrade = this.sharedData.sharedData.resourceDTO.find(item => item.resourceId === id).grade;
      resourceFormGroup.controls.grade.setValue(this.USTGrade);
      this.oldRole = this.sharedData.sharedData.resourceDTO.find(item => item.resourceId === id).oldName;
      resourceFormGroup.controls.oldRole.setValue(this.oldRole);
    }
  }

  onResourceMarkupChange(data: any, rowIndex: number) {
    let value: any;
    const resources = this.resourcePlanningForm.controls.resources as FormArray;
    const resourceGroup = resources.controls;
    const resourceFormGroup = resourceGroup[rowIndex] as FormGroup;
    if (data.id === -1) {
      resourceFormGroup.controls.contractorFlag.setValue(value = null);
      this.displayFTEValueAndTotalCostHours();
      return;
    }
    value = data.name.toLowerCase().includes('contract');
    const flagValue: string = value ? Constants.resourceContractorFlag : Constants.resourcePermanentFlag;
    resourceFormGroup.controls.contractorFlag.setValue(flagValue);
    this.displayFTEValueAndTotalCostHours();
  }

  private getPeriods(periods: ProjectPeriod[]) {
    // calculate colspansize
    this.colSpanSize = this.cols.length + periods.length + 1;
    if (this.cols.length > 0) {
      this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
      this.cols = this.cols.concat(this.periodCols);
      // calculate table footer colspansize
      this.colSpanSize = this.cols.length + this.periodCols.length + 1;
    }
  }

  getFTEOfCurrentResource(rowDataControl) {
    const fte: number[] = [];

    rowDataControl.controls.periods.controls.forEach(period => {
      fte.push(period.value.FTEValue ? period.value.FTEValue : 0);
    });
    return fte;
  }

  getResourcesControls(): AbstractControl[] {
    const resources = this.resourcePlanningForm.controls.resources as FormArray;
    const resourceGroup = resources.controls;

    return resourceGroup;
  }

  calculateFTE(rowDataControl, rowIndex) {
    const resourceGroup = this.getResourcesControls();
    const resourceFormGroup = resourceGroup[rowIndex] as FormGroup;
    const selectedResourceLocationId = resourceFormGroup.controls.location.value ?
      resourceFormGroup.controls.location.value.id : 0;
    const PeriodsPerResource = resourceFormGroup.controls.periods as FormArray;

    if (selectedResourceLocationId < 1) {
      return;
    }

    const fte = this.getFTEOfCurrentResource(rowDataControl);
    const resourceLocation = this.resourceLocations.find(val => val.value.id === selectedResourceLocationId).value;

    let totalHrs = 0;
    const totalHrsForTheMonth = 0;
    const daysWorked = 0;
    const noOfHolidays = 0;
    const totalNumberOfWorkingDays = 0;
    const noOfHolidaysInParticularMonth = 0;

    this.monthlyData.forEach((currentMonthData, index) => {

      let computedValue = 0;
      let calculatedData: number[] = [];

      calculatedData = this.resourcePlanningService.getTotalHours(computedValue, totalHrsForTheMonth, daysWorked, noOfHolidays,
        noOfHolidaysInParticularMonth
        , totalNumberOfWorkingDays, totalHrs, resourceLocation, this.totalDaysInProjectDurationMonths, this.projectDurationInDays,
        selectedResourceLocationId, fte, this.resourceMasterData, currentMonthData, index);

      totalHrs = calculatedData[0];
      computedValue = calculatedData[1];

      if (totalHrs === 0) {
        resourceFormGroup.controls.totalHours.setValue('');
      }
      else {
        resourceFormGroup.controls.totalHours.setValue(totalHrs.toFixed(1));
      }
      (<FormGroup>PeriodsPerResource.controls[index]).controls.TotalHours.setValue(computedValue);
    });
    this.displayTotalHours();
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

  onSubmit() {
    this.isSaveClicked = true;
    if (this.resourcePlanningForm.valid) {
      this.displayFTEValueAndTotalCostHours();
      const dataToSave = JSON.parse(JSON.stringify(this.resourcePlanningForm.value));
      this.composeSaveData(dataToSave);
      this.resourcePlanningService.saveResourcePlanning(this.resourcePlanningList).subscribe(val => {
        this.resourcePlanningForm.markAsPristine();
        this.resourcePlanningService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
          this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
          this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
        });
        this.resourcePlanningService.GetResourcePlanningData(this.pipSheetId).subscribe(data => {
          if (data.resources.length < 1) {
            data.resources.push(this.resourcePlanningService.getDataForNewResource(this.pipSheetId, 0,
              null, this.resourcePlanning.projectPeriods));
            this.addResource(data.resources[0]);
          }
          this.resourcePlanning = data;
          this.resourcePlanningList = new ResourceMapper(this.dateService).formulateResourceModel(data.resources, data.projectPeriods);
          this.isSaveClicked = false;
        });
        this.translateService.get('SuccessMessage.ResourcePlanningSave').subscribe(msg => {
          this.messageService.add({ severity: 'success', detail: msg });
          this.getOverrideNotificationStatus();
        });
        this.notificationService.notifyFormDirty(false);
      }, () => {
        this.translateService.get('ErrorMessage.ResourcePlanningSave').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
          this.isSaveClicked = false;
        });
      });
    }
    // enable the save button
  }

  composeSaveData(formData: any) {
    this.resourcePlanningList.forEach(resource => {
      const data = formData.resources.find(value => value.uId === resource.uId);

      if (!isNullOrUndefined(data)) {
        const resourceServiceLineId = this.sharedData.sharedData.resourceDTO.find(sl =>
          sl.resourceId === data.ustRole.id).resourceServiceLineId;

        resource.locationId = data.location.id;
        resource.alias = data.alias;
        resource.markupId = data.contractorMarkup.id;
        resource.milestoneId = data.phase.id;
        resource.pipSheetId = this.pipSheetId;
        resource.resourceGroupId = data.roleGroup.id;
        resource.resourceId = data.ustRole.id;
        resource.totalhoursPerResource = data.totalHours;
        resource.costHrsPerResource = data.totalCostHours;
        resource.grade = data.grade;
        resource.totalFTE = data.totalFTE === '' ? 0 : data.totalFTE;
        resource.utilizationType = data.utilization;
        resource.createdBy = 1;
        resource.updatedBy = 1;
        resource.clientRole = data.clientRole;
        resource.resourceServiceLineId = resourceServiceLineId;
        for (let i = 0; i < data.periods.length; i++) {
          resource.projectPeriod[i].fteValue = data.periods[i].FTEValue;
          resource.projectPeriod[i].revenue = null;
          resource.projectPeriod[i].totalHours = data.periods[i].TotalHours; // total hours per period
          resource.projectPeriod[i].costHours = data.periods[i].costHours;
          resource.projectPeriod[i].inflatedCostHours = this.resourcePlanningService.calculateInflatedCostHours(resource.projectPeriod[i],
            this.resourcePlanning.projectPeriods, resource.locationId);
        }
      } else {
        resource.isDeleted = true;
        resource.pipSheetId = this.pipSheetId;
      }
    });
  }

  displayFTEValueAndTotalCostHours() {
    let totalFTE = 0;
    let totalNormalizedCostHoursFTE = 0;
    let costHourPerResource = 0;
    let fTEPerResource = 0;
    let costHrs = 0;
    let costHrsFactor = 0;
    const firstPeriod = 'first';
    const lastPeriod = 'last';
    const resource: any = this.resourcePlanningForm.controls.resources;
    const costAndFTEPerResource: { key: number, ch: number, fte: number }[] = [];
    // START : Taking form control value for Location
    const resourceGroup = this.getResourcesControls();
    let resourceFormGroup: FormGroup = this.fb.group([]);
    // END
    const costHoursEquivalent = 160;
    this.totalCostHours = 0;
    for (let index = 0; index < this.periodCols.length; index++) {
      for (let i = 0; i < resource.value.length; i++) {
        resourceFormGroup = resourceGroup[i] as FormGroup;
        const locationId = resourceFormGroup.controls.location.value.id;
        for (let j = index; j < resource.value[i].periods.length; j++) {
          if (isNullOrUndefined(resource.value[i].periods[j].FTEValue) || resource.value[i].periods[j].FTEValue === '') {
            resource.value[i].periods[j].FTEValue = 0;
          }
          if (index === 0) {
            costHrsFactor = parseFloat(this.getCostHourFactor(firstPeriod, locationId));
            totalNormalizedCostHoursFTE = totalNormalizedCostHoursFTE + costHrsFactor * parseFloat(resource.value[i].periods[j].FTEValue);
            totalFTE = totalFTE + parseFloat(resource.value[i].periods[j].FTEValue);
            if (resource.value[i].contractorFlag === Constants.resourceContractorFlag) {
              costHrs = parseFloat(resource.value[i].periods[j].TotalHours) / costHoursEquivalent;
            } else {
              costHrs = costHrsFactor * parseFloat(resource.value[i].periods[j].FTEValue);
            }

            // point 1
            costAndFTEPerResource.push({
              key: i,
              ch: (costHrs * costHoursEquivalent),
              fte: parseFloat(resource.value[i].periods[j].FTEValue)
            });
          }
          else if (index === (resource.value[i].periods.length - 1)) {
            costHrsFactor = parseFloat(this.getCostHourFactor(lastPeriod, locationId));
            totalNormalizedCostHoursFTE = totalNormalizedCostHoursFTE + costHrsFactor * parseFloat(resource.value[i].periods[j].FTEValue);
            totalFTE = totalFTE + parseFloat(resource.value[i].periods[j].FTEValue);
            if (resource.value[i].contractorFlag === Constants.resourceContractorFlag) {
              costHrs = parseFloat(resource.value[i].periods[j].TotalHours) / costHoursEquivalent;
            } else {
              costHrs = costHrsFactor * parseFloat(resource.value[i].periods[j].FTEValue);
            }

            // point 2
            costAndFTEPerResource.push({
              key: i,
              ch: costHrs * costHoursEquivalent,
              fte: parseFloat(resource.value[i].periods[j].FTEValue)
            });
          }
          else {
            totalNormalizedCostHoursFTE = totalNormalizedCostHoursFTE + parseFloat(resource.value[i].periods[j].FTEValue);
            totalFTE = totalFTE + parseFloat(resource.value[i].periods[j].FTEValue);
            if (resource.value[i].contractorFlag === Constants.resourceContractorFlag) {
              costHrs = parseFloat(resource.value[i].periods[j].TotalHours) / costHoursEquivalent;
            } else {
              costHrs = parseFloat(resource.value[i].periods[j].FTEValue);
            }

            // point 3
            costAndFTEPerResource.push({
              key: i,
              ch: costHrs * costHoursEquivalent,
              fte: parseFloat(resource.value[i].periods[j].FTEValue)
            });
          }
          if (resource.value[i].contractorFlag === Constants.resourceContractorFlag) {
            resource.value[i].periods[j].costHours = parseFloat(resource.value[i].periods[j].TotalHours);
          } else {
            resource.value[i].periods[j].costHours = costHrs * costHoursEquivalent;
          }
          break;
        }
      }
      this.totalFTEValue[index] = totalFTE;
      this.totalCostHours = this.resourcePlanningService.getTotalCostHours(resource);
      if (isNaN(this.totalCostHours)) {
        this.isInvalid = false;
      }
      else {
        this.isInvalid = true;
      }
      totalNormalizedCostHoursFTE = 0;
      totalFTE = 0;
    }

    // recording cost per resource per location
    for (let i = 0; i < resource.value.length; i++) {
      const costHrsAndFTEPerResource = this.resourcePlanningService.calculateResourcePerHour(costAndFTEPerResource, i);
      costHourPerResource = costHrsAndFTEPerResource.totalCostValue;
      fTEPerResource = costHrsAndFTEPerResource.ftePerResource;
      (<FormGroup>resourceGroup[i]).controls['totalCostHours'].setValue(costHourPerResource.toFixed(2));
      (<FormGroup>resourceGroup[i]).controls['totalFTE'].setValue(fTEPerResource);
    }
  }

  displayTotalHours() {
    let total = 0;
    const resource: any = this.resourcePlanningForm.controls.resources;
    for (let i = 0; i < resource.length; i++) {
      if (isNullOrUndefined(resource.value[i].totalHours) || resource.value[i].totalHours === '') {
        resource.value[i].totalHours = 0;
      }
      total = total + parseFloat(resource.value[i].totalHours);
    }
    this.totalOfTotalHours = total.toString();
  }

  getCostHourFactor(firstOrLastperiod: string, locationId: number): string {

    return this.resourcePlanningService.getCostHourFactor(firstOrLastperiod, locationId, this.resourceMasterData, this.resourceHolidays);

  }

  resetForm() {
    this.resourcePlanningForm.reset();
  }

  onResourceLocationChanged(data: any, rowIndex) {
    const resourceGroup = this.getResourcesControls();
    const resourceFormGroup = resourceGroup[rowIndex] as FormGroup;
    this.filteredRoleGroups[rowIndex] = this.resourceRoleGroup;
    if (resourceFormGroup) {
      if (!this.filteredRoleGroups[rowIndex].map(g => g.value).includes(resourceFormGroup.controls.roleGroup.value)) {
        resourceFormGroup.controls.roleGroup.setValue(null);
        resourceFormGroup.controls.ustRole.setValue(null);

        this.filteredUSTRole[rowIndex] = [{
          label: '--- select ---',
          value: {
            id: -1,
            code: '--select---',
            name: 'select'
          }
        }];
      }
      else {
        this.onResourceRoleGroupChange(resourceFormGroup.controls.roleGroup.value, rowIndex);
      }
    }
  }

  requiredFieldsValitions(group: FormGroup) {
    const location = group.get('location');
    const roleGroup = group.get('roleGroup');
    if (location.value && location.value.id === -1) {
      location.setErrors({ emptyLocation: true });
    } else {
      location.setErrors(null);
    }

    if (roleGroup.value && roleGroup.value.id === -1) {
      roleGroup.setErrors({ emptyGroup: true });
    } else {
      roleGroup.setErrors(null);
    }
  }
  makeRowsSameHeight() {
    setTimeout(() => {
      if ($('.ui-table-scrollable-wrapper').length) {
        const wrapper = $('.ui-table-scrollable-wrapper');
        wrapper.each(function () {
          const w = $(this);
          const frozen_rows: any = w.find('.ui-table-frozen-view tr');
          const unfrozen_rows = w.find('.ui-table-unfrozen-view tr');
          for (let i = 0; i < frozen_rows.length; i++) {
            if (frozen_rows.eq(i).height() > unfrozen_rows.eq(i).height()) {
              unfrozen_rows.eq(i).height(frozen_rows.eq(i).height());
            } else if (frozen_rows.eq(i).height() < unfrozen_rows.eq(i).height()) {
              frozen_rows.eq(i).height(unfrozen_rows.eq(i).height());
            }
          }
        });
      }
    });
  }

  calculateTotalFTEPerResource(rowIndex: number) {
    const resource = this.getResourcesControls();
    const totalResourceFTE = this.resourcePlanningService.calculateTotalFTEPerResource(resource[rowIndex]);
    (<FormGroup>resource[rowIndex]).controls['totalFTE'].setValue(totalResourceFTE.toFixed(2));
  }

  onCellClick(isContextEvent: boolean, rowData?, event?, rowIndex?: number) {
    if (!isContextEvent) {
      this.currentCellIndex = +(<string>event.target.id).split('a')[1];
      this.isCellActive = (this.currentCellIndex >= 0) ? true : false;
      this.currentRowIndex = rowIndex;
      this.currentRowData = rowData;
    } else {
      const periods: FormArray = (<FormArray>this.getResourcesControls()[this.currentRowIndex]).controls['periods'];
      this.currentCellValue = +<FormGroup>periods.controls[this.currentCellIndex].value.FTEValue;
      if (!isNaN(this.currentCellValue)) {
        periods.controls.forEach((fteControl, index) => {
          if (index > this.currentCellIndex) {
            (<FormGroup>fteControl).controls['FTEValue'].setValue(this.currentCellValue);
          }
        });
        this.calculateFTE(this.currentRowData, this.currentRowIndex);
        this.displayFTEValueAndTotalCostHours();
        this.calculateTotalFTEPerResource(this.currentRowIndex);
        this.isCellActive = false;
        this.scrollToEnd(this.currentRowIndex, (this.resourcePlanning.projectPeriods.length - 1).toString());
      }
    }
  }

  activateContext() {
    this.isCellActive = true;
  }
  deactivateContext() {
    this.isCellActive = false;
  }
  scrollToEnd(rowIndex: number, length: string) {
    const index = rowIndex.toString() + 'a' + length;
    document.getElementById(index).focus();
  }
}
