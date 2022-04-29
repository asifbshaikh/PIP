import { NotificationService } from '@global';
import { IPipMainVersion } from './../../../shared-module/domain/IPipMainVersion';
import { CheckOutDialogComponent } from './../CheckOutDialog/CheckOutDialog.component';
import { SharedDataService } from './../../../global-module/services/shared-data.service';
import { UserWorkflowService } from './../../../shared-module/services/user-workflow.service';
import { ICheckRole } from './../../../shared-module/domain/ICheckRole';
import { IWorkflowFlag } from '@shared/domain/IWorkflowFlag';
import { IPipCheckIn } from './../../../shared-module/domain/IPipCheckIn';
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { IPipVersion } from '@shared';
import { PipVersionService } from '@shared/services/pip-version.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MenuItem, MessageService, DialogService, ConfirmationService } from 'primeng/api';
import { Constants } from '@shared/infrastructure/constants';
import { isNull } from 'util';


@Component({
  selector: 'app-version',
  templateUrl: './pip-version.component.html',
})
export class PipVersionComponent implements OnInit {
  versionCols: any[] = [];
  selectedProjectId: string;
  versionMainList: IPipMainVersion;
  versionMenuItems: MenuItem[];
  versionForLastShownVersionMenu: IPipVersion;
  pipSheetId: number;
  workflowStatus: any;
  workflowFlag: IWorkflowFlag;
  checkRole: ICheckRole;
  loggedInUserId: number;
  isAnyVersionSubmitted: boolean;
  isAnyVersionApproved: boolean;
  displayDelete = false;
  replicateNew = false;
  replicateSame = false;
  replicateExisting = false;
  replicateNewInMyPips = false;
  isDataAvailable = false;
  isDummy: boolean;
  replicate = false;
  replicationTypeId: number;
  replicateObject: IPipVersion[];
  selectedVersionId: number;

  constructor(
    private translateService: TranslateService,
    private versionService: PipVersionService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private router: Router,
    private userWorkflowService: UserWorkflowService,
    private sharedData: SharedDataService,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private notificationService: NotificationService
  ) {
  }
  ngOnInit() {

    this.translateService.get('PROJECTS.Versions.VersionColumns').subscribe(cols => {
      this.versionCols = cols;
    });

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfStatus => {
      this.workflowStatus = wfStatus;
    });
    this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
    this.isDummy = this.router.url.includes('samples') ? true : false;

    this.route.paramMap.subscribe(
      params => {
        this.selectedProjectId = params.get('projectId');
        if (parseInt(this.selectedProjectId, 10) > 0) {
          this.getVersions(this.selectedProjectId);
        }
      });
  }

  navigateToSharePipScreen(): void {
    this.router.navigate([
      this.isDummy ? Constants.uiRoutes.sample : Constants.uiRoutes.projects,
      this.selectedProjectId,
      'versions',
      'sharepip',
    ]);
  }

  getVersions(projectId: string) {
    this.versionService.getPipVersions(projectId).subscribe(data => {
      this.replicateObject = data.pipSheetVersionDTO;
      this.versionMainList = data;
      this.isAnyVersionSubmitted = this.checkIfAnyVersionSubmitted();
      this.isAnyVersionApproved = this.checkIfAnyVersionApproved();
      this.notificationService.notifyVersionApproved(this.isAnyVersionApproved);
      this.isDataAvailable = true;
    });
  }

  editThisVersion(): () => void {
    return () => {
      let dashboardId = 0;
      let statusApprovalPending;
      let statusApproved: string;

      const singlePipVersionData = this.versionMainList.pipSheetVersionDTO.find(version => version.pipSheetId === this.pipSheetId);
      let checkInOutFlag = false;
      if (!singlePipVersionData.isCheckedOut && !(singlePipVersionData.checkedInOutBy === this.loggedInUserId)) {

        const ref = this.dialogService.open(CheckOutDialogComponent, {
          data: {
            projectId: this.selectedProjectId,
            pipSheetId: this.pipSheetId,
            accountId: singlePipVersionData.accountId,
            dashboardId: dashboardId,
            isCheckedOut: singlePipVersionData.isCheckedOut,
            checkedInOutByName: singlePipVersionData.checkedInOutByName
          },
          header: 'PIP Checked Out',
          height: '15%',
          width: '50%',
        });
      }
      else {
        this.translateService.get('SHARED.WORKFLOWSTATUS.Approved').subscribe(cols => {
          statusApproved = cols;
        });

        this.translateService.get('SHARED.WORKFLOWSTATUS.ApprovalPending').subscribe(cols => {
          statusApprovalPending = cols;
        });

        if (this.versionMainList.pipSheetVersionDTO.find(x => x.status === statusApproved || x.status === statusApprovalPending)) {
          dashboardId = 3;        // Make all pages readonly if one of the versions has status = Approval pending or Approved
        }
        else {
          dashboardId = 1;        // Editable
        }

        const pipCheckIn: IPipCheckIn = {
          pipSheetId: this.pipSheetId,
          isCheckedOut: false,
          checkedInOutBy: null
        };
        checkInOutFlag = singlePipVersionData ? (singlePipVersionData.status === 'Not Submitted'
          && this.userWorkflowService.returnRoleCheckForEditor(singlePipVersionData.roleName) &&
          singlePipVersionData.isCheckedOut && dashboardId === 1) : false;
        if (checkInOutFlag) {
          this.versionService.updatePIPSheetCheckIn(pipCheckIn).subscribe((res: number) => { });
        }

        this.router.navigate([
          this.isDummy ? Constants.uiRoutes.sample : Constants.uiRoutes.projects,
          this.selectedProjectId,
          this.versionForLastShownVersionMenu.pipSheetId,
          this.versionMainList.pipSheetVersionDTO[0].accountId,
          dashboardId,      // Coming from Version Screen
          Constants.uiRoutes.staff
        ]);
      }
    };
  }

  onDeleteClick(): () => void {
    return () => {
      this.displayDelete = true;
    };
  }
  onReplicateInNewProjectClick(id: number): () => void {
    return () => {
      this.replicationTypeId = id;
      this.replicateNew = true;
      this.replicate = true;
    };
  }
  onReplicateInNewProjectMyPipsClick(id: number): () => void {
    return () => {
      this.replicationTypeId = id;
      this.replicateNewInMyPips = true;
      this.replicate = true;
    };
  }
  onReplicateExistingClick(id: number): () => void {
    return () => {
      this.replicationTypeId = id;
      this.replicateExisting = true;
      this.replicate = true;
    };
  }
  onReplicateSameClick(id: number): () => void {
    return () => {
      this.replicationTypeId = id;
      this.replicateSame = true;
      this.replicate = true;
    };
  }

  replicationAction(event) {
    if (isNull(event)) {
      this.replicateNew = false;
      this.replicateExisting = false;
      this.replicateSame = false;
      this.replicateNewInMyPips = false;
      this.replicate = false;
    }
  }

  deleteThisVersion() {
    this.versionService.deletePipSheet(this.pipSheetId, this.selectedProjectId).subscribe(pipV => {
      this.translateService.get('SuccessMessage.DeletePipVersion').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      if (this.versionMainList.pipSheetVersionDTO.length !== 1) {
        this.getVersions(this.selectedProjectId);
      } else {
        this.router.navigate([
          this.isDummy ? Constants.uiRoutes.sample : Constants.uiRoutes.projects,
        ]);
      }

    });
  }

  setVersionForShownVersionMenu(version: IPipVersion) {
    this.versionForLastShownVersionMenu = version;
    this.pipSheetId = version.pipSheetId;
    this.selectedVersionId = version.versionNumber;
    this.versionMenuItems = this.isAnyVersionSubmitted ? [
      { label: 'Replicate in new project', icon: 'pi pi-fw pi-copy', command: this.onReplicateInNewProjectClick(1) },
      { label: 'Replicate in existing project', icon: 'pi pi-fw pi-clone', command: this.onReplicateExistingClick(2) },
    ] : this.isDummy ? [
      { label: 'Edit this version', icon: 'pi pi-fw pi-pencil', command: this.editThisVersion() },
      {
        label: 'Delete this Version', icon: 'pi pi-fw pi-trash', command: this.onDeleteClick(),
        disabled: this.enableDisableOption(version)
      },
      {
        label: this.isDummy ? 'Replicate in New Dummy Project' : 'Replicate in New Project',
        icon: 'pi pi-fw pi-copy', command: this.onReplicateInNewProjectClick(1),
        disabled: this.enableDisableOption(version)
      },
      {
        label: this.isDummy ? 'Replicate in New Project My Pips' : '',
        icon: 'pi pi-fw pi-copy', command: this.onReplicateInNewProjectMyPipsClick(1),
        disabled: this.enableDisableOption(version)
      },
      {
        label: this.isDummy ? 'Replicate in Existing Project My Pips' : 'Replicate in existing project'
        , icon: 'pi pi-fw pi-clone', command: this.onReplicateExistingClick(2),
        disabled: this.enableDisableOption(version)
      },
      {
        label: 'Replicate in same project', icon: 'pi pi-fw pi-clone', command: this.onReplicateSameClick(2),
        disabled: this.enableDisableOption(version)
      }
    ] : [
          { label: 'Edit this version', icon: 'pi pi-fw pi-pencil', command: this.editThisVersion() },
          {
            label: 'Delete this Version', icon: 'pi pi-fw pi-trash', command: this.onDeleteClick(),
            disabled: this.enableDisableOption(version)
          },
          {
            label: 'Replicate in new project', icon: 'pi pi-fw pi-copy', command: this.onReplicateInNewProjectClick(1),
            disabled: this.enableDisableOption(version)
          },
          {
            label: 'Replicate in existing project', icon: 'pi pi-fw pi-clone', command: this.onReplicateExistingClick(2),
            disabled: this.enableDisableOption(version)
          },
          {
            label: 'Replicate in same project', icon: 'pi pi-fw pi-clone', command: this.onReplicateSameClick(2),
            disabled: this.enableDisableOption(version)
          }
        ];
  }
  navigateToPipSheet(projectId: number, pipSheetId: number, accountId: number) {
    this.router.navigate([
      this.isDummy ? Constants.uiRoutes.sample : Constants.uiRoutes.projects,
      projectId,
      pipSheetId,
      accountId,
      3,      // Should be readonly when clicked in VersionId
      Constants.uiRoutes.staff
    ]);
  }

  returnRoleCheckForKebabMenu(rowData: any): boolean {
    if (this.userWorkflowService.returnRoleCheckForEditor(rowData.roleName) && rowData.hasAccountLevelAccess
      && this.isAnyVersionSubmitted) {
      return true;
    }
    else if (this.userWorkflowService.returnRoleCheckForEditor(rowData.roleName) && !this.isAnyVersionSubmitted) {
      return true;
    }
    else {
      return false;
    }
  }

  returnRoleCheckForCheckinCheckOutIcon(roleName: string[]): boolean {
    return this.userWorkflowService.returnRoleCheckForCheckinCheckOutIcon(roleName);
  }

  enableDisableOption(version: IPipVersion): boolean {
    if ((version.roleName.indexOf('Editor') >= 0 ? true : false) && version.status === 'Not Submitted' &&
      (version.isCheckedOut ? true : (this.loggedInUserId === version.checkedInOutBy ? true : false)) && version.hasAccountLevelAccess) {
      return false;
    }
    else {
      return true;
    }
  }

  checkIfAnyVersionApproved(): boolean {
    if (this.versionMainList.pipSheetVersionDTO.find(x => x.status === this.workflowStatus.Approved)) {
      return true;
    }
    return false;
  }

  checkIfAnyVersionSubmitted(): boolean {
    if (this.versionMainList.pipSheetVersionDTO.find(x => x.status === this.workflowStatus.Approved ||
      x.status === this.workflowStatus.ApprovalPending)) {
      return true;
    }
    else if (this.versionMainList.projectWorkflowStatus === 2 || this.versionMainList.projectWorkflowStatus === 3) {
      if (this.versionMainList.projectWorkflowStatus === 2) {
        this.notificationService.notifyProjectApproved(true);
      } else {
        this.notificationService.notifyProjectApprovalPending(true);
      }
      return true;
    }
    else {
      this.notificationService.notifyProjectNotSubmitted(true);
      return false;
    }
  }

  getToolTipMessage(roleName: string[], checkedInOutByName: string, checkedInOutBy: number): string {
    if (roleName.indexOf('Editor') >= 0) {
      return this.userWorkflowService.getToolTipMessageForEditor(checkedInOutByName, checkedInOutBy, this.loggedInUserId);
    }
    else if (roleName.indexOf('Editor') >= 0 && roleName.indexOf('Readonly') >= 0) {
      return this.userWorkflowService.getToolTipMessageForEditor(checkedInOutByName, checkedInOutBy, this.loggedInUserId);
    }
    else if (roleName.indexOf('Readonly') >= 0) {
      return this.userWorkflowService.getToolTipMessageForReadonly(checkedInOutByName, checkedInOutBy, this.loggedInUserId);
    }
  }
}
