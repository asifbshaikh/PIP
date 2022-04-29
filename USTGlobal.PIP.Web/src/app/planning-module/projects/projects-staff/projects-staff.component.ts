import { Location } from '@angular/common';
import { SharedDataService } from './../../../global-module/services/shared-data.service';
import { ICheckRole } from './../../../shared-module/domain/ICheckRole';
import { IWorkflowFlag } from '@shared/domain/IWorkflowFlag';
import { Component, OnInit, Input, HostListener } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { StaffSteps, Constants } from '@shared';
import { NotificationService } from '@global';
import { ProjectService } from '@shared/services/project.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-projects-staff',
  templateUrl: './projects-staff.component.html'
})
export class ProjectsStaffComponent implements OnInit {

  stepPageIndex: any;
  stepLabels = [];
  totalVersions: number;
  projectId: number;
  pipsheetId: number;
  dashboardId: number;
  accountId: number;
  workflowFlag: IWorkflowFlag;
  checkRole: ICheckRole;
  loggedInUserId: number;
  display = false;
  isDirty: boolean;
  hasAccountLevelAccess: boolean;
  isDummy: boolean;
  isLoading = false;

  constructor(public translateService: TranslateService,
    private notificationService: NotificationService,
    private projectService: ProjectService,
    private router: Router,
    private messageService: MessageService,
    private route: ActivatedRoute,
    private sharedData: SharedDataService,
    private location: Location
  ) {
    this.route.paramMap.subscribe(
      params => {
        this.projectId = parseInt(params.get(Constants.uiRoutes.routeParams.projectId), 10);
        this.pipsheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
        this.accountId = parseInt(params.get(Constants.uiRoutes.routeParams.accountId), 10);
        this.dashboardId = parseInt(params.get(Constants.uiRoutes.routeParams.dashboardId), 10);
      });
    this.isLoading = true;
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.sharedData.populateCommonData(this.pipsheetId).then(x => {
        this.isLoading = false;
        this.stepPageIndex = +params.get('tabIndex');
        this.location.replaceState(this.location.path().split(';')[0], '');
      });
    });
    this.translateService.get('PLANNING.Staff.StaffSteps').subscribe((resource) => {
      this.stepLabels = resource;
    });
    this.notificationService.totalVersionExistsNotification.subscribe((totalVersion) => {
      this.totalVersions = totalVersion;
    });
    this.notificationService.workflowFlagExistsNotification.subscribe((workflowFlag) => {
      this.workflowFlag = workflowFlag;
    });
    this.notificationService.roleExistsNotification.subscribe((role) => {
      this.checkRole = role;
    });
    this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
    this.notificationService.isFormDirty.subscribe(isDirty => {
      this.isDirty = isDirty;
    });
    this.notificationService.hasAccountLevelAccessNotification.subscribe(hasAccountLevelAccess => {
      this.hasAccountLevelAccess = hasAccountLevelAccess;
    });

    this.isDummy = this.router.url.includes('samples') ? true : false;
  }

  onNewVersionButtonClick() {
    if (!this.isDirty) {
      this.display = true;
    } else {
      this.notificationService.showDialog();
    }
  }
  onNewVersionClick() {
    this.projectService.createNewPipVersion(this.projectId, this.pipsheetId).subscribe(data => {
      this.translateService.get('Version' + ' ' + data + ' ' + 'Created Successfully!').subscribe(msg => {
        const newPipSheetId = parseFloat(data.toString());
        this.router.navigate([
          this.isDummy ? Constants.uiRoutes.sample : Constants.uiRoutes.projects,
          this.projectId,
          'versions'
        ]);
        this.messageService.add({ severity: 'success', detail: msg });
      },
        () => {
          this.translateService.get('ErrorMessage.NewVersion').subscribe(msg => {
            this.messageService.add({ severity: 'error', detail: msg });
          });
        });
    });
  }
}
