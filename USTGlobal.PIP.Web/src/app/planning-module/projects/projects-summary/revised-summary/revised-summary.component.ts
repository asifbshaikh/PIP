import { Constants, ISubmitPipSheet } from '@shared';
import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, MessageService, Message } from 'primeng/api';
import { SummaryService } from '@shared/services/summary.service';
import { BreadcrumbService, Breadcrumb } from '@shared';
import { IHeader1 } from '@shared/domain/IHeader1';
import { SharedDataService, NotificationService } from '@global';
import { IUsers } from '@shared/domain/IUsers';
import { DefineAdminService } from '@shared/services/define-admin.service';
import { isNullOrUndefined } from 'util';
import { ISharedPipRole } from '@shared/domain/ISharedPipRole';
import { UserRoleService } from '@shared/services/user-role.service';
import { ProjectService } from '@shared/services/project.service';
import { IPipCheckIn } from '@shared/domain';
import { IPipVersionSummaryDetail } from '@shared/domain/IPipVersionSummaryDetail';
import { IPipSheetWorkflowStatus } from '@shared/domain/IPipSheetWorkflowStatus';
import { IWorkflowFlag } from '@shared/domain/IWorkflowFlag';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { ICheckRole } from '@shared/domain/ICheckRole';
import { UserWorkflowService } from '@shared/services/user-workflow.service';
import { ISubmitPipSheetReturnType } from '@shared/domain/ISubmitPipSheetReturnType';

@Component({
  selector: 'app-revised-summary',
  templateUrl: './revised-summary.component.html'
})

export class RevisedSummaryComponent implements OnInit {
  pipCheckIn: IPipCheckIn;
  pipSheet: ISubmitPipSheet;
  sharePIPSheetFrom: FormGroup;
  pipSheetStatus: number;
  pipSheetId: number;
  projectId: number;
  accountId: number;
  dashboardId: number;
  comments: string;
  approverComments: string;
  public submitted = false;
  public approved = false;
  public submitClick = false;
  public approveClick = false;
  public resendClick = false;
  public reviseClick = false;
  msgs: Message[] = [];
  public href = '';
  version: string;
  confirmMessage: string;
  confirmMessageHeader: string;
  confirm1Message: string;
  confirmApproveMessageHeader: string;
  confirmResendMessageHeader: string;
  confirmReviseMessageHeader: string;
  isCurrencyId = false;
  userList: IUsers[];
  user: IUsers;
  errorMessege: string;
  isCheckedIn = true; pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  pipVersionSummaryDetail: IPipVersionSummaryDetail;
  alreadyApproved = false;
  alreadyResend = false;
  alreadyRevise = false;
  approversName: string;
  sfProjectId: string;
  repeatedActionMessage = '';
  resendComments: string;
  hasAccountLevelAccess: boolean;
  showFooterBtn = false;
  isDummy: boolean;
  showInvestment = false;
  showGpm = false;
  showYoy = false;
  showForecast = false;
  showBilling = false;
  showEffortSummary = false;
  showSummary = false;
  stepPageIndex: any;
  stepLabels = [];
  PerformanceIndicators: any;
  isValid: boolean;
  isLoading = false;
  @Input()
  get pipURL() {
    const str = this.router.url;
    const splitted = str.split('/', 7);
    splitted[5] = '3';
    splitted[splitted.length - 1] = 'Staff';
    const url = splitted.join('/');
    this.href = location.origin + url;
    return this.href;
  }
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private summaryService: SummaryService,
    private breadcrumbService: BreadcrumbService,
    private sharedData: SharedDataService,
    private defineAdminService: DefineAdminService,
    private userRoleService: UserRoleService,
    private projectService: ProjectService,
    private userWorkflowService: UserWorkflowService,
    private notificationService: NotificationService
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
      this.projectId = parseInt(data['params'].projectId, 10);
      this.accountId = parseInt(data['params'].accountId, 10);
      this.dashboardId = parseInt(data['params'].dashboardId, 10);
    });
    this.isLoading = true;
  }

  ngOnInit() {
    this.isDummy = this.router.url.includes('samples') ? true : false;
    this.sharedData.populateCommonData(this.pipSheetId).then(sharedData => {
      this.isCurrencyId = sharedData.currencyId ? true : false;
      this.isLoading = false;
    });
    this.breadcrumbService.newBreadcrumb.subscribe((breadcrumb: Breadcrumb[]) => {
      if (breadcrumb.length > 0) {
        this.version = breadcrumb[2] ? breadcrumb[2].label : this.version;
      }
    });

    this.summaryService.getHeader1Data(this.projectId, this.pipSheetId)
      .subscribe(headerInfo => {
        this.setProjectBreadcrumb(headerInfo.header1);
      });

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });

    this.confirmMessage = this.translateService.instant('ProjectSummary.confirmMessage');
    this.confirmMessageHeader = this.translateService.instant('ProjectSummary.confirmMessageHeader');
    this.confirm1Message = this.translateService.instant('ProjectSummary.confirm1Message');

    this.confirmApproveMessageHeader = this.translateService.instant('ProjectSummary.confirmApproveMessageHeader');
    this.confirmResendMessageHeader = this.translateService.instant('ProjectSummary.confirmResendMessageHeader');
    this.confirmReviseMessageHeader = this.translateService.instant('ProjectSummary.confirmReviseMessageHeader');
    this.initializeFrom();

    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.projectService.getWorkflowStatusAccountRole(this.pipSheetId, this.accountId)
        .subscribe(roleAndWfStatus => {
          this.pipSheetWorkflowStatus = roleAndWfStatus.pipSheetWorkflowStatus;
          this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus, this.wfstatus);
          this.roleAndAccount = roleAndWfStatus.roleAndAccountDTO;
          this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
          this.hasAccountLevelAccess = roleAndWfStatus.hasAccountLevelAccess;
          if (this.roleAndAccount != null) {
            this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
          }
          this.isDataAvailable = true;
        });
    }
    else {
      this.isDataAvailable = true;
    }
    this.translateService.get('PLANNING.Staff.SummarySteps').subscribe((resource) => {
      this.stepLabels = resource;
    });
  }
  displayYOY(yoyState) {
    this.showYoy = yoyState;
  }
  displayForecast(forecastState) {
    this.showForecast = forecastState;
  }
  displayBilling(billingLoad) {
    this.showBilling = billingLoad;
  }
  displayInvestment(investmentLoad) {
    this.showInvestment = investmentLoad;
  }
  displayEffortSummary(effortuSmmaryLoad) {
    this.showEffortSummary = effortuSmmaryLoad;
  }
  displaySummary(summaryLoad) {
    this.showSummary = summaryLoad;
  }
  displayGpm(gpmLoad) {
    this.showGpm = gpmLoad;
  }

  GetPipSheetSubmitInfo(): ISubmitPipSheet {
    return {
      pipSheetId: this.pipSheetId,
      projectId: this.projectId,
      versionNumber: 0,
      currencyId: null,
      pipSheetStatusId: 0,
      comments: this.comments,
      approverComments: this.approverComments,
      resendComments: this.resendComments,
      isCheckedOut: this.isCheckedIn,
      isSubmit: false,
      isApprove: false,
      isResend: false,
      isRevise: false,
      isEdit: false,
    };
  }

  setProjectBreadcrumb(headerData?: IHeader1) {
    const isVersionRoute = this.router.url.match(Constants.uiRoutes.versions);

    let newBreadcrumbData: Breadcrumb[];
    if (headerData) {
      newBreadcrumbData = [
        {
          label: Constants.breadcrumbLabels.projects,
          url: Constants.uiRoutes.projects
        },
        {
          label: headerData.projectName,
          url: !isVersionRoute ?
            `${Constants.uiRoutes.projects}/${(this.projectId)}/${Constants.uiRoutes.versions}` : ''
        }];
    }
    else {
      newBreadcrumbData = [
        {
          label: Constants.breadcrumbLabels.projects,
          url: Constants.uiRoutes.projects
        }];
    }

    if (!isVersionRoute && headerData) {
      newBreadcrumbData.push({
        label: `Version ${headerData.versionNumber}`,
        url: ''
      });
    }
    this.breadcrumbService.updateBreadcrumb(newBreadcrumbData);
  }

  confirmSubmit() {
    this.userWorkflowService.getOverrideNotificationStatus(+this.pipSheetId).subscribe(item => {
      const overrideNotification = item;
      if ((isNullOrUndefined(overrideNotification.clientPrice) ? false : overrideNotification.clientPrice)
        || (isNullOrUndefined(overrideNotification.riskManagement) ? false : overrideNotification.riskManagement)
        || (isNullOrUndefined(overrideNotification.vacationAbsence) ? false : overrideNotification.vacationAbsence)
        || (isNullOrUndefined(overrideNotification.ebitdaStdOverhead) ? false : overrideNotification.ebitdaStdOverhead)) {
        this.notificationService.showNotificationDialog(this.pipSheetId);
      } else {
        this.summaryService.getPipVersionDetailsOnSummary(this.pipSheetId).subscribe(pip => {
          this.pipVersionSummaryDetail = pip;
          this.submitClick = true;
          this.confirmationService.confirm({
            message: 'Add optional comment for this version of sheet.',
            header: `Project ID ${this.pipVersionSummaryDetail.sfProjectId} has
             ${this.pipVersionSummaryDetail.totalVersionsPresent} Versions.
            Are you sure you want to submit version ${this.pipVersionSummaryDetail.versionNumber}?`,

            accept: () => {
              this.pipSheet = this.GetPipSheetSubmitInfo();
              this.pipSheet.pipSheetStatusId = 3;
              this.pipSheet.isSubmit = true;
              this.summaryService.submitPipSheet(this.pipSheet).subscribe(success => {
                this.translateService.get('SuccessMessage.PipSheetSubmit').subscribe(msg => {
                  this.notificationService.notifyProjectApprovalPending(true);

                  this.messageService.add({ severity: 'success', detail: msg });
                  this.submitConfirm();
                });
              }, () => {
                this.translateService.get('ErrorMessage.PipSheetSubmit').subscribe(msg => {
                  this.messageService.add({ severity: 'error', detail: msg });
                });
              });
              const str = this.router.url;
              const splitted = str.split('/', 7);
              splitted[splitted.length - 1] = 'Staff';
              const url = splitted.join('/');
              this.href = location.origin + url;
            },
            reject: () => {
              this.submitClick = false;
              this.comments = '';
              this.isValid = false;
            }
          });
        });
      }
    });
  }

  submitConfirm() {
    const str = this.router.url;
    const splitted = str.split('/', 7);
    splitted[splitted.length - 1] = 'Staff';
    this.submitted = true;
    this.confirmationService.confirm({
      message: this.confirm1Message + '<br><br>',
      header: 'PIP-' + this.version + ' ' + 'has been submitted for approval.',
      accept: () => {
        this.msgs = [{ severity: 'info', summary: 'Confirmed', detail: 'You have accepted' }];
        if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
          this.projectService.getWorkflowStatusAccountRole(this.pipSheetId, this.accountId)
            .subscribe(roleAndWfStatus => {
              this.pipSheetWorkflowStatus = roleAndWfStatus.pipSheetWorkflowStatus;
              this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus, this.wfstatus);
            });
        }
        this.submitted = false;
        this.submitClick = false;
        this.notificationService.notifySubmitClick(!this.submitClick);
      },
      reject: () => {
        this.msgs = [{ severity: 'info', summary: 'Rejected', detail: 'You have rejected' }];
        this.submitClick = false;
        this.comments = '';
        this.isValid = false;
        this.notificationService.notifySubmitClick(!this.submitClick);
      }
    });
  }

  confirmApprove() {
    this.approveClick = true;

    this.pipSheet = this.GetPipSheetSubmitInfo();
    this.pipSheet.pipSheetStatusId = 2;
    this.pipSheet.isApprove = true;

    this.summaryService.getPIPSheetStatus(this.pipSheet).subscribe(response => {
      if (!response.isAlreadyApproved && !response.isAlreadyResend) {
        this.confirmationService.confirm({
          // message: this.confirmMessage,
          header: this.confirmApproveMessageHeader,
          accept: () => {
            this.pipSheet = this.GetPipSheetSubmitInfo();
            this.pipSheet.pipSheetStatusId = 2;
            this.pipSheet.isApprove = true;
            this.summaryService.submitPipSheet(this.pipSheet).subscribe(success => {
              this.translateService.get('SuccessMessage.PipSheetApproved').subscribe(msg => {
                this.notificationService.notifyProjectApproved(true);
                this.messageService.add({ severity: 'success', detail: msg });
                if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
                  this.projectService.getWorkflowStatusAccountRole(this.pipSheetId, this.accountId)
                    .subscribe(roleAndWfStatus => {
                      this.pipSheetWorkflowStatus = roleAndWfStatus.pipSheetWorkflowStatus;
                      this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus, this.wfstatus);
                    });
                }
              });
              this.approveClick = false;
              this.router.navigate([
                Constants.uiRoutes.approver
              ]);
            }, () => {
              this.translateService.get('ErrorMessage.PipSheetApproved').subscribe(msg => {
                this.messageService.add({ severity: 'error', detail: msg });
              });
            });
          },
          reject: () => {
            this.msgs = [{ severity: 'info', summary: 'Rejected', detail: 'You have rejected' }];
            this.approveClick = false;
            this.approverComments = '';
            this.isValid = false;
          }
        });
      }
      else {
        this.approveClick = false;
        this.alreadyApproved = true;
        this.approversName = response.approverName;
        this.sfProjectId = response.sfProjectId;
        this.getPopUpMessage(response);
      }
    });
  }

  confirmResend() {
    this.resendClick = true;
    this.pipSheet = this.GetPipSheetSubmitInfo();
    this.pipSheet.pipSheetStatusId = 1;
    this.pipSheet.isResend = true;
    this.summaryService.getPIPSheetStatus(this.pipSheet).subscribe(response => {
      if (!response.isAlreadyResend && !response.isAlreadyApproved) {
        this.confirmationService.confirm({
          message: this.confirmMessage,
          header: this.confirmResendMessageHeader,
          accept: () => {
            this.pipSheet = this.GetPipSheetSubmitInfo();
            this.pipSheet.pipSheetStatusId = 1;
            this.pipSheet.isResend = true;
            this.summaryService.submitPipSheet(this.pipSheet).subscribe(success => {
              this.translateService.get('SuccessMessage.PipSheetResend').subscribe(msg => {
                this.notificationService.notifyProjectNotSubmitted(true);
                this.messageService.add({ severity: 'success', detail: msg });
                if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
                  this.projectService.getWorkflowStatusAccountRole(this.pipSheetId, this.accountId)
                    .subscribe(roleAndWfStatus => {
                      this.pipSheetWorkflowStatus = roleAndWfStatus.pipSheetWorkflowStatus;
                      this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus, this.wfstatus);
                    });
                }
              });
            }, () => {
              this.translateService.get('ErrorMessage.PipSheetResend').subscribe(msg => {
                this.messageService.add({ severity: 'error', detail: msg });
              });
            });
            this.resendClick = false;
            this.notificationService.notifyResendClick(!this.resendClick);
          },
          reject: () => {
            this.msgs = [{ severity: 'info', summary: 'Rejected', detail: 'You have rejected' }];
            this.resendClick = false;
            this.resendComments = '';
            this.isValid = false;
          }
        });
      }
      else {
        this.resendClick = false;
        this.alreadyResend = true;
        this.approversName = response.approverName;
        this.sfProjectId = response.sfProjectId;
        this.getPopUpMessage(response);
      }
    });
  }

  confirmRevise() {
    this.reviseClick = true;
    this.pipSheet = this.GetPipSheetSubmitInfo();
    this.pipSheet.pipSheetStatusId = 1;
    this.pipSheet.isRevise = true;
    this.summaryService.getPIPSheetStatus(this.pipSheet).subscribe(response => {
      if (!response.isAlreadyRevised) {
        this.confirmationService.confirm({
          message: this.confirmMessage,
          header: this.confirmReviseMessageHeader,
          accept: () => {
            this.pipSheet = this.GetPipSheetSubmitInfo();
            this.pipSheet.pipSheetStatusId = 1;
            this.pipSheet.isRevise = true;
            this.summaryService.submitPipSheet(this.pipSheet).subscribe(success => {
              this.translateService.get('SuccessMessage.PipSheetRevise').subscribe(msg => {
                this.messageService.add({ severity: 'success', detail: msg });
                if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
                  this.projectService.getWorkflowStatusAccountRole(this.pipSheetId, this.accountId)
                    .subscribe(roleAndWfStatus => {
                      this.pipSheetWorkflowStatus = roleAndWfStatus.pipSheetWorkflowStatus;
                      this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus, this.wfstatus);
                    });
                }
              });
            }, () => {
              this.translateService.get('ErrorMessage.PipSheetRevise').subscribe(msg => {
                this.messageService.add({ severity: 'error', detail: msg });
              });
            });
            this.reviseClick = false;
          },
          reject: () => {
            this.msgs = [{ severity: 'info', summary: 'Rejected', detail: 'You have rejected' }];
            this.reviseClick = false;
            this.resendComments = '';
            this.isValid = false;
          }
        });
      }
      else {
        this.reviseClick = false;
        this.alreadyRevise = true;
        this.approversName = response.approverName;
        this.sfProjectId = response.sfProjectId;
        this.getPopUpMessage(response);
      }
    });
  }

  editPip(): void {
    this.router.navigate([
      Constants.uiRoutes.projects,
      this.projectId,
      this.pipSheetId,
      this.accountId,
      this.dashboardId,
      Constants.uiRoutes.staff
    ]);

    const pipCheckIn: IPipCheckIn = {
      pipSheetId: this.pipSheetId,
      isCheckedOut: false,
      checkedInOutBy: null
    };
    this.projectService.updatePIPSheetCheckIn(pipCheckIn).subscribe((res: number) => { });
    this.pipSheet = this.GetPipSheetSubmitInfo();
    this.pipSheet.pipSheetStatusId = 1;
    this.pipSheet.isEdit = true;
    this.summaryService.submitPipSheet(this.pipSheet).subscribe(success => { }, () => { });
  }

  // Share Form Logic start
  initializeFrom() {
    this.sharePIPSheetFrom = this.fb.group({
      userId: [],
      name: [],
      roleId: [],
      accountId: [],
      uid: ['', [Validators.required]],
      isEditor: [],
      isReadOnly: [],
      isReviewer: [],
      comment: [],
    });
  }

  onShareIconClick() {
    const str = this.router.url;
    const splitted = str.split('/', 7);
    splitted[splitted.length - 1] = 'Staff';
    const url = splitted.join('/');
    this.href = location.origin + url;

    this.defineAdminService.getUsers().subscribe(users => {
      this.userList = users;
    });
    this.sharePIPSheetFrom.reset();
    this.errorMessege = '';
  }

  onUIDInput() {
    this.user = this.userList.find(uId => uId.uid === this.sharePIPSheetFrom.controls.uid.value.toLocaleUpperCase());
    this.sharePIPSheetFrom.controls.name.setValue(this.user ? this.user.firstName + ' ' + this.user.lastName : '');
    this.validateUId();
  }

  validateUId() {
    if (this.sharePIPSheetFrom.controls.uid.value) {
      if (!this.user) {
        this.sharePIPSheetFrom.controls.uid.setErrors({ 'invalid': true });
        this.errorMessege = 'User does not exists.';
      }
      else {
        this.sharePIPSheetFrom.controls.uid.setErrors(null);
        this.errorMessege = '';
      }
    }
    else {
      this.sharePIPSheetFrom.controls.uid.setErrors({ 'invalid': true });
      this.errorMessege = '';
    }
  }

  onShareClick() {
    const sharedPipRole: ISharedPipRole = {
      userId: this.user.userId,
      uid: this.sharePIPSheetFrom.value.uid.toLocaleUpperCase(),
      pipSheetId: this.pipSheetId,
      isEditor: this.sharePIPSheetFrom.value.isEditor ? this.sharePIPSheetFrom.value.isEditor : false,
      isReadOnly: this.sharePIPSheetFrom.value.isReadOnly ? this.sharePIPSheetFrom.value.isReadOnly : false,
    };
    this.userRoleService.saveSharedPipRole(sharedPipRole).subscribe(data => {
      this.translateService.get('SuccessMessage.PIPSheetShare').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
    });
    this.sharePIPSheetFrom.reset();
  }

  onExitClick(): void {
    if (this.dashboardId === 2) {
      this.router.navigate([
        Constants.uiRoutes.approver,
      ]);
    }
    else if (this.dashboardId === 1 || this.dashboardId === 3) {
      if (this.isDummy) {
        this.router.navigate([
          Constants.uiRoutes.sample
        ]);
      }
      else {
        this.router.navigate([
          Constants.uiRoutes.projects
        ]);
      }
    }
  }

  onButtonClickUpdateWfStatus() {
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.projectService.getWorkflowStatusAccountRole(this.pipSheetId, this.accountId)
        .subscribe(roleAndWfStatus => {
          this.pipSheetWorkflowStatus = roleAndWfStatus.pipSheetWorkflowStatus;
          this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus, this.wfstatus);
        });
    }
    this.alreadyApproved = false;
    this.alreadyResend = false;
    this.alreadyRevise = false;
  }

  getPopUpMessage(success: ISubmitPipSheetReturnType) {
    if (success.isAlreadyApproved) {
      this.repeatedActionMessage = 'Project ' + this.sfProjectId + ' is already Approved by ' + this.approversName;
    }
    else if (success.isAlreadyResend) {
      this.repeatedActionMessage = 'Project ' + this.sfProjectId + ' is already Resend by ' + this.approversName;
    }
    else if (success.isAlreadyRevised) {
      this.repeatedActionMessage = 'Project ' + this.sfProjectId + ' is already Revised by ' + this.approversName;
    }
  }

  onCopyButtonClick() {
    this.translateService.get('CopyLink').subscribe(msg => {
      this.messageService.add({ severity: 'success', detail: msg });
    });
  }
  validateComments(comments) {
    // Comments Should be mandatory
    if (comments.target.value) {
      this.isValid = true;
    }
    else {
      this.isValid = false;
    }
  }
}
