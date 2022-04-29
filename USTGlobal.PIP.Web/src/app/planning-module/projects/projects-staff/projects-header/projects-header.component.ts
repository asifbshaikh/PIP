import { IPipSheetWFStatusAndAccountSpecificRole } from './../../../../shared-module/domain/IPipSheetWFStatusAndAccountSpecificRole';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { IRoleAndAccount } from './../../../../shared-module/domain/IRoleAndAccount';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IProjectCurrencyHeader } from '../../../../shared-module/domain/IProjectCurrencyHeader';
import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ProjectService } from '@shared/services/project.service';
import { IProject, IProjectHeader, ICurrency } from '@shared';
import { SelectItem, MessageService } from 'primeng/api';
import { Mastermapper } from '@shared/mapper/master/mastermapper';
import { ActivatedRoute, Router, RoutesRecognized } from '@angular/router';
import { isNullOrUndefined, isUndefined } from 'util';
import { TranslateService } from '@ngx-translate/core';
import { ValidationService } from '@core/infrastructure/validation.service';
import { SharedDataService, NotificationService } from '@global';
import { Constants } from './../../../../shared-module/infrastructure/constants';
import { IUser } from '@global';
import { IHeader1 } from '@shared/domain/IHeader1';
import { Breadcrumb } from '@shared/breadcrumb/breadcrumb.model';
import { BreadcrumbService } from '@shared/breadcrumb/breadcrumb.service';
import { OverrideNotificationStatus } from '@shared/domain/override-notification-status';

@Component({
  selector: 'app-projects-header',
  templateUrl: './projects-header.component.html'
})

export class ProjectsHeaderComponent implements OnInit, OnDestroy, AfterViewInit {


  projectHeader: FormGroup;
  pShowToolTip: string;
  projects: IProject[];
  accountNameEntities: SelectItem[];
  contractingEntities: SelectItem[];
  servicePorfolios: SelectItem[];
  serviceLines: SelectItem[];
  deliveryTypes: SelectItem[];
  billingType: SelectItem[];
  editingServiceLineId: number;
  sProjectHeader: IProjectCurrencyHeader;
  user: IUser;
  versionNumber: number;
  filteredServiceLines: SelectItem[];
  filteredBillingType: SelectItem[];
  selectedProjectId: string;
  selectedPipSheetId: string;
  selectedAccountId: string;
  selectedAccountNameEntity: SelectItem;
  selectedContractingEntity: SelectItem;
  selectedPortfolioGroup: SelectItem;
  selectedServiceLine: SelectItem;
  selectedDeliveryType: SelectItem;
  selectedBillingType: SelectItem;
  countries: SelectItem[];
  isDataAvailable = false;
  currencyControl: FormGroup;
  projectId: number;
  pipSheetId: number;
  selectedCurrency: string;
  idFormat: string;
  currency: string;
  currencyConversion: [];
  fields: [];
  pipSheetWFStatusAndAccSpecificRole: IPipSheetWFStatusAndAccountSpecificRole;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  loggedInUserId: number;
  accountId: number;
  dashboardId: number;
  approverComments: string;
  resendComments: string;
  approvedBy: string;
  approvedOn: Date;
  resendBy: string;
  resendOn: Date;
  isDummy: boolean;
  savedAccountId: number;
  savedAccountName: string;
  hasAccountLevelAccess: boolean;
  pipSheetIdCheck: number;
  contractingEntityId = Constants.contractingEntityId;
  msgs: any[];

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private route: ActivatedRoute,
    private router: Router,
    private translateService: TranslateService,
    private validate: ValidationService,
    private sharedData: SharedDataService,
    private messageService: MessageService,
    private notificationService: NotificationService,
    private breadcrumbService: BreadcrumbService,
    private userWorkflowService: UserWorkflowService
  ) { }

  ngOnInit() {
    this.isDummy = this.router.url.includes('samples') ? true : false;
    this.projectHeader = this.fb.group({
      accountNameEntity: ['', [Validators.required, this.validate.validateDeselectedDropdown]],
      servicePortfolioGroup: ['', [Validators.required, this.validate.validateDeselectedDropdown]],
      contractingEntity: ['', [Validators.required, this.validate.validateDeselectedDropdown]],
      beatTax: [null],
      serviceLine: ['', [Validators.required, this.validate.validateDeselectedDropdown]],
      projectIdPerSf: [this.isDummy ? { value: '', disabled: true } : { value: '', disabled: false },
      [Validators.required, Validators.maxLength(15),
      this.isDummy ? Validators.pattern(null) : Validators.pattern(Constants.regExType.projectName)
      ]],
      projectDeliveryType: ['', [Validators.required, this.validate.validateDeselectedDropdown]],
      projectNamePerSf: ['', [Validators.required, this.validate.noWhitespaceValidator, Validators.maxLength(100)]],
      sfBillingType: ['', [Validators.required, this.validate.validateDeselectedDropdown]],
      deliveryOwner: ['', [Validators.maxLength(100)]],
      updatedBy: [{ value: '', disabled: true }, [Validators.maxLength(100)]],
      createdBy: [{ value: '', disabled: true }, [Validators.maxLength(100)]],
      pipsheetCreatedBy: [{ value: '', disabled: true }, [Validators.maxLength(100)]],
      submittedBy: [{ value: '', disabled: true }, [Validators.maxLength(100)]],
      lastUpdatedBy: [{ value: '', disabled: true }, [Validators.maxLength(100)]],
      approverComments: ['', []],
      approvedBy: ['', []],
      approvedOn: ['', []],
      resendComments: ['', []],
      resendBy: ['', []],
      resendOn: ['', []],
      currencyControl: this.fb.group({
        country: [''],
        symbol: [''],
        usdToLocal: [''],
        localToUSD: [''],
        currencyId: [''],
        factorUsed: ['']
      }),
    });
    this.notificationService.notifySubmitClick(false);
    this.sharedData.showProjectMilestone = false;
    this.contractingEntities = new Mastermapper().getContractingEntityComboItems(this.sharedData.sharedData.contractingEntityDTO);

    this.servicePorfolios = new Mastermapper().getServicePortfolioComboItems(this.sharedData.sharedData.servicePortfolioDTO);

    this.serviceLines = new Mastermapper().getServiceLineComboItems(this.sharedData.sharedData.serviceLineDTO);

    this.deliveryTypes = new Mastermapper().getDeliveryTypesComboItems(this.sharedData.sharedData.projectDeliveryTypeDTO);

    this.billingType = new Mastermapper().getBillingTypesComboItems(this.sharedData.sharedData.projectBillingTypeDTO);
    this.user = this.sharedData.sharedData.userRoleAccountDTO;
    this.countries = new Mastermapper().getCountryDetails(this.sharedData.sharedData.countryDTO, this.sharedData.sharedData.currencyDTO);

    this.translateService.get('ProjectHeader.format').subscribe(format => {
      this.idFormat = format;
    });
    this.translateService.get('ProjectHeader.conversion').subscribe(conversion => {
      this.currency = conversion;
    });
    this.translateService.get('ProjectHeader.fields').subscribe(fields => {
      this.fields = fields;
    });
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });

    this.route.paramMap.subscribe(
      params => {
        this.selectedProjectId = params.get('projectId');
        this.selectedPipSheetId = params.get('pipSheetId');
        this.selectedAccountId = params.get('accountId');
        this.pipSheetIdCheck = + this.selectedPipSheetId;
        this.dashboardId = parseInt(params.get('dashboardId'), 10);
        if (+this.selectedPipSheetId === 0 && +this.selectedProjectId === 0) {
          this.projectService.getUserRoleForAllAccounts().subscribe(roleData => {
            this.accountNameEntities = new Mastermapper().getAccountNameEntityComboItemsOnProjectHeader
              (this.sharedData.sharedData.accountDTO, roleData, parseInt(this.selectedPipSheetId, 10), this.dashboardId, true, false, -1);
          });
        }
        this.setPipSheetCreatedBy();
        if ((+this.selectedProjectId > 0) && (+this.selectedPipSheetId > 0)) {
          this.getProjectHeaderData(this.selectedProjectId, this.selectedPipSheetId, this.selectedAccountId);
        }
        if (parseInt(this.selectedProjectId, 10) === 0 && parseInt(this.selectedPipSheetId, 10) === 0) {
          this.getCurrencyConversionDetailsByCountryIdAndBind(23);
        }
        this.isDataAvailable = true;
      });
  }

  ngAfterViewInit() {
    this.projectHeader.valueChanges.subscribe(() => {
      if (this.projectHeader.dirty) {
        this.notificationService.notifyFormDirty(true);
      }
    });
  }

  enableDisableForm(): boolean {
    let flag = false;
    if (this.dashboardId === 3) {     // To be opened in readonly mode
      flag = true;
    }
    else {
      flag = this.userWorkflowService.isFormDisabled(this.checkRole, this.workflowFlag, this.loggedInUserId, this.dashboardId);
    }
    if (flag) {
      setTimeout(() => {
        this.projectHeader.disable();
      }, 200);
    }
    return flag;
  }

  setBeatTax(contractingEntityId: any, isBind: boolean, beatTaxValue: boolean) {
    if (contractingEntityId === Constants.contractingEntityId && !isBind) {
      this.projectHeader.get('beatTax').enable();
      this.projectHeader.get('beatTax').setValue(true);
    }
    else if (isBind) {
      if (contractingEntityId === Constants.contractingEntityId) {
        this.projectHeader.get('beatTax').enable();
        this.projectHeader.get('beatTax').setValue(beatTaxValue);
      }
      else if (contractingEntityId !== Constants.contractingEntityId) {
        this.projectHeader.get('beatTax').disable();
        this.projectHeader.get('beatTax').setValue(beatTaxValue);
      }
    }
    else {
      this.projectHeader.get('beatTax').disable();
      this.projectHeader.get('beatTax').setValue(null);
    }
  }

  setPipSheetCreatedBy() {
    if (parseInt(this.selectedPipSheetId, 10) === 0 && parseInt(this.selectedProjectId, 10) === 0) {
      this.projectHeader.controls.pipsheetCreatedBy.patchValue(this.user.firstName + this.user.lastName);
    }
  }

  getProjectHeaderData(projectId: string, pipSheetId: string, accountId: string) {
    if (parseInt(this.selectedPipSheetId, 10) > 0 && this.selectedPipSheetId !== undefined) {

      //   need to call save dependency in case of navigation coming from replicate pipsheet

      this.projectService.getProjectHeaderData(projectId, pipSheetId).subscribe(data => {
        this.sProjectHeader = data;
        this.savedAccountId = this.sProjectHeader.projectHeader.accountId;
        this.savedAccountName = this.sProjectHeader.projectHeader.sfProjectId;
        this.setBeatTax(this.sProjectHeader.projectHeader.contractingEntityId, true, this.sProjectHeader.projectHeader.beatTax);
        this.notificationService.notifyTotalVersionExists(data.totalVersionsPresent);
        if (parseInt(projectId, 10) > 0) {
          this.projectService.getWorkflowStatusAccountRole(parseInt(this.selectedPipSheetId, 10),    // get work flow status
            parseInt(this.selectedAccountId, 10))
            .subscribe(roleAndWfStatus => {
              this.pipSheetWFStatusAndAccSpecificRole = roleAndWfStatus;
              this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWFStatusAndAccSpecificRole.pipSheetWorkflowStatus,
                this.wfstatus);
              this.notificationService.notifyHasAccountLevelAccess(roleAndWfStatus.hasAccountLevelAccess);
              this.hasAccountLevelAccess = roleAndWfStatus.hasAccountLevelAccess;
              this.roleAndAccount = roleAndWfStatus.roleAndAccountDTO;
              this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
              this.sharedData.roleAndAccount = roleAndWfStatus.roleAndAccountDTO;
              if (this.roleAndAccount != null) {
                this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
              }
              this.notificationService.notifyWorkflowFlagExists(this.workflowFlag);
              this.notificationService.notifyRoleExists(this.checkRole);
              const flag = this.enableDisableForm();
              this.projectService.getUserRoleForAllAccounts().subscribe(roleData => {
                this.accountNameEntities = new Mastermapper().getAccountNameEntityComboItemsOnProjectHeader
                  (this.sharedData.sharedData.accountDTO, roleData,
                    parseInt(this.selectedPipSheetId, 10), this.dashboardId, flag, false, data.projectHeader.accountId);
                this.bindControls();
                this.isDataAvailable = true;
              });
            });
        }
      });
    }
    else {
      this.isDataAvailable = true;
    }
  }


  // convenience getter for easy access to form fields
  get refPh() { return this.projectHeader.controls; }

  onServicePortfolioGroupchange(data: any) {
    this.projectHeader.get('serviceLine').reset();
    this.selectedPortfolioGroup = data;
    if (this.serviceLines) {
      this.filteredServiceLines = this.serviceLines.filter(x => x.value.code === data.id);
      if (this.filteredServiceLines.length > 0 && this.sProjectHeader) {
        const selectedServiceLine = this.filteredServiceLines.find(val => val.value.id === this.sProjectHeader.
          projectHeader.serviceLineId);
        if (selectedServiceLine) {
          this.projectHeader.patchValue({ serviceLine: selectedServiceLine.value });
        }
      }
    }
  }

  onDeliveryTypechange(data: any) {
    this.filteredBillingType = []; // check why forcontrol not getting reset here
    this.projectHeader.controls.sfBillingType.reset();
    this.selectedDeliveryType = data;
    if (this.billingType && data.id !== -1) {
      this.projectService.getBillingTypeBasedOnDeliveryID(data.id).then(billingdata => {
        this.filteredBillingType = new Mastermapper().getBillingTypesComboItems(billingdata);
        if (this.sProjectHeader) {
          const selectedBillingType = this.filteredBillingType.find(val => val.value.id === this.sProjectHeader.
            projectHeader.projectBillingTypeId);
          if (selectedBillingType) {
            this.selectedBillingType = selectedBillingType.value;
          }
        }
      });
    }
  }

  setCurrencyControls(currencyData: ICurrency) {

    this.projectHeader.controls.currencyControl.patchValue({
      symbol: currencyData.symbol,
      usdToLocal: currencyData.usdToLocal,
      localToUSD: currencyData.localToUSD,
      currencyId: currencyData.currencyId,
      factorUsed: currencyData.factors,
      country: this.countries.filter(country => country.value.id === currencyData.countryId)[0].value
    });
  }

  onCountryChange(value) {
    if (+this.selectedPipSheetId) {
      this.showCurrencyInfoMsg();
    }
    this.getCurrencyConversionDetailsByCountryIdAndBind(value.id);
  }

  getCurrencyConversionDetailsByCountryIdAndBind(countryId: number) {
    this.projectService.getCurrencyConversionDetailsByCountryId(countryId).subscribe(currencyData => {
      this.setCurrencyControls(currencyData);
    });
  }

  getOverrideNotificationStatus() {
    let overrideNotification: OverrideNotificationStatus;
    this.userWorkflowService.getOverrideNotificationStatus(+this.selectedPipSheetId).subscribe(item => {
      overrideNotification = item;
      if (overrideNotification.clientPrice || overrideNotification.riskManagement
        || overrideNotification.vacationAbsence || overrideNotification.ebitdaStdOverhead) {
        this.notificationService.showNotificationDialog(this.selectedPipSheetId);
      }
    });
  }

  onSaveClick() {
    const project: IProjectHeader = {

      sfProjectId: this.projectHeader.getRawValue().projectIdPerSf.toUpperCase(),
      projectId: parseInt(this.selectedProjectId, 10),
      pipSheetId: parseInt(this.selectedPipSheetId, 10),
      projectName: this.projectHeader.value.projectNamePerSf,
      deliveryOwner: this.projectHeader.value.deliveryOwner === '' ? null : this.projectHeader.value.deliveryOwner,
      accountId: isNullOrUndefined(this.projectHeader.value.accountNameEntity) ? null :
        this.projectHeader.value.accountNameEntity.id === -1 ? null : this.projectHeader.value.accountNameEntity.id,
      contractingEntityId: isNullOrUndefined(this.projectHeader.value.contractingEntity) ? null :
        this.projectHeader.value.contractingEntity.id === -1 ? null : this.projectHeader.value.contractingEntity.id,
      beatTax: this.projectHeader.getRawValue().beatTax,
      servicePortfolioId: isNullOrUndefined(this.projectHeader.value.servicePortfolioGroup) ?
        null : this.projectHeader.value.servicePortfolioGroup.id === -1 ? null : this.projectHeader.value.servicePortfolioGroup.id,
      serviceLineId: isNullOrUndefined(this.projectHeader.value.serviceLine) ? null : this.projectHeader.value.serviceLine.id,
      projectDeliveryTypeId: isNullOrUndefined(this.projectHeader.value.projectDeliveryType) ?
        null : this.projectHeader.value.projectDeliveryType.id,
      projectBillingTypeId: isNullOrUndefined(this.projectHeader.value.sfBillingType) ? null
        : this.projectHeader.value.sfBillingType.id === -1 ? null : this.projectHeader.value.sfBillingType.id,
      submittedBy: this.sharedData.sharedData.userRoleAccountDTO.userId,
      createdBy: this.sharedData.sharedData.userRoleAccountDTO.userId,
      updatedBy: this.sharedData.sharedData.userRoleAccountDTO.userId,
      errorCode: 0,
      currencyId: this.projectHeader.value.currencyControl.currencyId,
      lastUpdatedBy: this.projectHeader.value.lastUpdatedBy,
      pipsheetCreatedBy: this.projectHeader.value.pipsheetCreatedBy,
      approverComments: '',
      approvedBy: '',
      approvedOn: new Date(),
      approverStatusId: 0,
      resendBy: '',
      resendOn: new Date(),
      resendComments: '',
      isDummy: this.isDummy,
    };

    // save new project
    if (project.projectId === 0) {
      this.projectService.saveProjectData(project).subscribe(res => {
        if (res && res.projectId > -1 && res.pipSheetId > -1 && res.errorCode > -1) {
          this.selectedCurrency = this.projectHeader.value.currencyControl.symbol;
          this.savedAccountId = project.accountId;
          this.savedAccountName = project.sfProjectId;
          this.translateService.get('SuccessMessage.ProjectSave').subscribe(msg => {
            this.messageService.add({ severity: 'success', detail: msg });
            this.sharedData.populateCommonData(res.pipSheetId);
            this.getOverrideNotificationStatus();
          });
          this.projectHeader.controls.lastUpdatedBy.patchValue(this.user.firstName + this.user.lastName);
          this.setProjectDataOnHeader(res.projectId, res.pipSheetId);
          if (this.isDummy) {
            this.router.navigate([`samples/${res.projectId}/${res.pipSheetId}/${project.accountId}/${this.dashboardId}/Staff`]);
          }
          else {
            this.router.navigate([`projects/${res.projectId}/${res.pipSheetId}/${project.accountId}/${this.dashboardId}/Staff`]);
          }
          this.projectHeader.markAsPristine();
        }
        else if (res.errorCode === -1) {
          let ErrorMessage = 'ErrorMessage.ProjectExist';

          if (this.isDummy) {
            ErrorMessage = 'ErrorMessage.ProjectExistDummy';
            this.projectHeader.controls['accountNameEntity'].reset();
            this.projectHeader.controls['projectIdPerSf'].reset();
          }

          this.translateService.get(ErrorMessage).subscribe(msg => {
            this.messageService.add({ severity: 'error', detail: msg });
          });
        }

        this.notificationService.notifyFormDirty(false);
      }, () => {
        this.translateService.get('ErrorMessage.ProjectSave').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
        });
      });
    } else { // update existing project
      this.projectService.saveProjectData(project).subscribe(res => {
        if (res && res.projectId > -1 && res.pipSheetId > -1 && res.errorCode > -1) {
          this.selectedCurrency = this.projectHeader.value.currencyControl.symbol;
          this.savedAccountId = project.accountId;
          this.savedAccountName = project.sfProjectId;
          this.translateService.get('SuccessMessage.ProjectUpdated').subscribe(msg => {
            this.messageService.add({ severity: 'success', detail: msg });
            this.getOverrideNotificationStatus();
          });
          this.projectHeader.controls.lastUpdatedBy.patchValue(this.user.firstName + this.user.lastName);
          if (this.isDummy) {
            this.router.navigate([`samples/${res.projectId}/${res.pipSheetId}/${project.accountId}/${this.dashboardId}/Staff`]);
          }
          else {
            this.router.navigate([`projects/${res.projectId}/${res.pipSheetId}/${project.accountId}/${this.dashboardId}/Staff`]);
          }
          this.setProjectDataOnHeader(res.projectId, res.pipSheetId);
          this.projectHeader.markAsPristine();
        }
        else if (res.errorCode === -1) {
          let ErrorMessage = 'ErrorMessage.ProjectExist';

          if (this.isDummy) {
            ErrorMessage = 'ErrorMessage.ProjectExistDummy';
            this.projectHeader.controls['accountNameEntity'].reset();
            this.projectHeader.controls['projectIdPerSf'].reset();
          }
          this.translateService.get(ErrorMessage).subscribe(msg => {
            this.messageService.add({ severity: 'error', detail: msg });
          });
        }
        this.notificationService.notifyFormDirty(false);

      }, () => {
        this.translateService.get('ErrorMessage.ProjectUpdated').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
        });
      });
    }
  }

  setProjectDataOnHeader(projectId: number, pipSheetId: number): void {
    let projectName = '', sfProjectId = '';
    if (projectId > 0) {
      this.projectService.getHeader1Data(projectId, pipSheetId).subscribe(header1Data => {
        projectName = header1Data.header1.projectName;
        sfProjectId = header1Data.header1.sfProjectId;
        this.notificationService.notifyProjectNameExists(projectName);
        this.notificationService.notifySFProjectIdExists(sfProjectId);
        this.notificationService.notifyCurrencyChange(this.selectedCurrency);
        this.notificationService.notifyTotalClientPriceExists(header1Data.header1.totalClientPrice ?
          header1Data.header1.totalClientPrice.toString() : null);
        this.notificationService.notifyPercentEbitdaExists(header1Data.headerEbitda.projectEBITDAPercent.toString());
      });
    }
    else {
      this.notificationService.notifyProjectNameExists(projectName);
      this.notificationService.notifySFProjectIdExists(sfProjectId);
      this.notificationService.notifyCurrencyChange(this.selectedCurrency);
    }
  }

  bindControls() {
    if (this.sProjectHeader.projectHeader.projectDeliveryTypeId) {
      const deliveryType = this.deliveryTypes.find(val => val.value.id === this.sProjectHeader.projectHeader.projectDeliveryTypeId).value;
      this.onDeliveryTypechange(deliveryType);
    }
    this.versionNumber = this.sProjectHeader.projectHeader.versionNumber;
    this.approverComments = this.sProjectHeader.projectHeader.approverComments;
    this.approvedBy = this.sProjectHeader.projectHeader.approvedBy;
    this.approvedOn = this.sProjectHeader.projectHeader.approvedOn;
    this.resendComments = this.sProjectHeader.projectHeader.resendComments;
    this.resendBy = this.sProjectHeader.projectHeader.resendBy;
    this.resendOn = this.sProjectHeader.projectHeader.resendOn;

    this.projectHeader.patchValue({
      // accountNameEntity: this.sProjectHeader.projectHeader.accountId,
      projectNamePerSf: this.sProjectHeader.projectHeader.projectName,
      projectIdPerSf: this.sProjectHeader.projectHeader.sfProjectId,
      deliveryOwner: this.sProjectHeader.projectHeader.deliveryOwner,
      submittedBy: this.sProjectHeader.projectHeader.submittedBy,
      approverComments: this.sProjectHeader.projectHeader.approverComments,
      approvedBy: this.sProjectHeader.projectHeader.approvedBy,
      approvedOn: this.sProjectHeader.projectHeader.approvedOn,
      pipsheetCreatedBy: this.sProjectHeader.projectHeader.pipsheetCreatedBy,
      lastUpdatedBy: this.sProjectHeader.projectHeader.lastUpdatedBy
    });

    this.projectHeader.controls.currencyControl.patchValue({
      symbol: this.sProjectHeader.currency.symbol,
      usdToLocal: this.sProjectHeader.currency.usdToLocal,
      localToUSD: this.sProjectHeader.currency.localToUSD,
      currencyId: this.sProjectHeader.currency.currencyId,
      factorUsed: this.sProjectHeader.currency.factors,
      country: this.countries.filter(country => country.value.id === this.sProjectHeader.currency.countryId)[0].value
    });

    this.notificationService.notifyCurrencyChange(this.sProjectHeader.currency.symbol);

    if (this.sProjectHeader.projectHeader.accountId != null) {
      this.selectedAccountNameEntity = this.accountNameEntities.find(val => val.value.id === this.sProjectHeader.
        projectHeader.accountId).value;

    }
    if (this.sProjectHeader.projectHeader.contractingEntityId != null) {
      this.selectedContractingEntity = this.contractingEntities.find(val => val.value.id === this.sProjectHeader.
        projectHeader.contractingEntityId).value;

    }
    if (this.sProjectHeader.projectHeader.servicePortfolioId != null) {
      const spGroup = this.servicePorfolios.find(val => val.value.id === this.sProjectHeader.projectHeader.servicePortfolioId).value;
      this.onServicePortfolioGroupchange(spGroup);
    }
  }

  projectIdError() {
    if (this.projectHeader.get('projectIdPerSf').invalid) {
      this.translateService.get('MESSAGES.Tooltip.ProjectIdTooltipMessage').subscribe((resource) => {
        this.pShowToolTip = resource['message'];
      });
    } else {
      this.pShowToolTip = '';
    }
  }

  projectDeliveryTypeError() {
    if (this.projectHeader.get('projectDeliveryType').invalid) {
      this.translateService.get('MESSAGES.Tooltip.DeliveryTypeTooltipMessage').subscribe((resource) => {
        this.pShowToolTip = resource['message'];
      });
    } else {
      this.pShowToolTip = '';
    }
  }

  projectBillingTypeError() {
    if (this.projectHeader.get('sfBillingType').invalid) {
      this.translateService.get('MESSAGES.Tooltip.BillingTypeTooltipMessage').subscribe((resource) => {
        this.pShowToolTip = resource['message'];
      });
    } else {
      this.pShowToolTip = '';
    }
  }

  accountNameError() {
    if (this.projectHeader.get('accountNameEntity').invalid) {
      this.translateService.get('MESSAGES.Tooltip.DeliveryTypeTooltipMessage').subscribe((resource) => {
        this.pShowToolTip = resource['message'];
      });
    } else {
      this.pShowToolTip = '';
    }
  }

  projectNameError() {
    if (this.projectHeader.get('projectNamePerSf').invalid) {
      this.translateService.get('MESSAGES.Tooltip.ProjectNameTooltipMessage').subscribe((resource) => {
        this.pShowToolTip = resource['message'];
      });
    } else {
      this.pShowToolTip = '';
    }
  }
  onAccountNameChange(accId) {
    if (this.isDummy && (isNullOrUndefined(this.savedAccountId) || this.savedAccountId !== accId)) {
      const accountCode = this.sharedData.sharedData.accountDTO.find(id => +accId === id.accountId).accountCode;
      this.projectService.getAutoGeneratedProjectId(accId, accountCode).subscribe(dummyId => {
        this.projectHeader.controls['projectIdPerSf'].setValue(dummyId);
      });
    }
    else if (isNullOrUndefined(this.savedAccountId) || this.savedAccountId !== accId) {
      const accountCode = this.sharedData.sharedData.accountDTO.find(id => +accId === id.accountId).accountCode;
      this.projectHeader.controls['projectIdPerSf'].setValue(accountCode + '-');
    }
    else {
      this.projectHeader.controls['projectIdPerSf'].setValue(this.savedAccountName);
    }
  }
  showCurrencyInfoMsg() {
    this.messageService.clear();
    this.messageService.add({
      key: 'msgs', severity: 'info', detail: this.translateService.instant('ProjectHeader.info.currencyMsg'), life: 5000
    });
  }

  ngOnDestroy(): void {
    if (this.projectHeader.value.projectDeliveryType && [2, 5, 6].includes(this.projectHeader.value.projectDeliveryType.id)) {
      this.sharedData.showProjectMilestone = true;
    }
  }
}
