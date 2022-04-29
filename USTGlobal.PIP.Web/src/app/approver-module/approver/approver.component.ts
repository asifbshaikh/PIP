import { NotificationService } from '@global';
import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ApproverService } from '@shared/services/approver.service';
import { IApprover } from '@shared/domain/IApprover';
import { SelectItem } from 'primeng/api';
import { Router } from '@angular/router';
import { Constants } from '@shared';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-approver',
  templateUrl: './approver.component.html',
})
export class ApproverComponent implements OnInit {
  cols: any[];
  showSearch = false;
  approverList: IApprover[];
  pipSheetStatusDropdown: SelectItem[];
  filteredList: IApprover[];
  defaultDate = '01-01 12:00';

  @ViewChild('dt')
  private table: Table;
  dashboardId = 2; // 2 represents approver and reviewers grid

  constructor(
    private translateService: TranslateService,
    private router: Router,
    private approverService: ApproverService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.translateService.get('PipSheetStatusDropdown.statusFlag').subscribe((data) => {
      this.pipSheetStatusDropdown = data;
    });

    this.translateService.get('Approver.ApproverLisColumns').subscribe(col => {
      this.cols = col;
    });
    this.notificationService.notifyVersionApproved(false);
    this.getApproverData();
  }

  getApproverData() {
    this.approverService.getApproversData().subscribe(approverList => {
      this.approverList = approverList;
      this.filteredList = this.approverList;
      this.notificationService.notifyProjectApprovalPending(true);
      this.onDropdownChange(status = 'Waiting for Approval');

    });
  }

  onDropdownChange(status: any) {
    if (status === 4) {
      this.approverList = this.filteredList;
    }
    else {
      this.approverList = this.filteredList.filter(flag => flag.pipSheetStatus === status);
    }


    if (status === 'Waiting for Approval') {
      this.notificationService.notifyProjectApprovalPending(true);
    } else if (status === 'Approved') {
      this.notificationService.notifyProjectApproved(true);
    } else {
      this.notificationService.notifyProjectNotSubmitted(true);
    }
  }

  navigateToProject(projectId: number, pipSheetId: number, accountId: number): void {
    this.router.navigate([
      Constants.uiRoutes.projects,
      projectId,
      pipSheetId,
      accountId,
      this.dashboardId,
      Constants.uiRoutes.staff
    ]);
  }

  onSearchFilters() {
    this.showSearch = !this.showSearch;
    this.table.reset();

  }

}
