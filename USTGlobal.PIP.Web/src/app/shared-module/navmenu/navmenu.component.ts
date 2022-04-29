import { SharedData } from './../../global-module/services/shared-data';
import { IRoleAndAccount } from './../domain/IRoleAndAccount';
import { ICheckRole } from './../domain/ICheckRole';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService, NotificationService } from '@global';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-navmenu',
  templateUrl: './navmenu.component.html'
})
export class NavmenuComponent implements OnInit {
  public collapsed = true;
  checkRole: ICheckRole;
  roleAndAccount: IRoleAndAccount[];
  isDirty: boolean;
  @Output() isCollapsed = new EventEmitter<any>();
  hasAccountLevelAccess = false;
  hasSharePipAccess = false;
  hasDummyPipAccess = false;
  hasAccountLevelEditorAccess = false;

  constructor(
    private router: Router,
    private translateService: TranslateService,
    private sharedData: SharedDataService,
    private notificationService: NotificationService) { }

  items: MenuItem[];

  ngOnInit() {
    $('.tab').on('click', function () {
      $('.tab').removeClass('active');
      $(this).addClass('active');
    });

    this.roleAndAccount = this.sharedData.sharedData.userRoleAccountDTO.roleAndAccountDTO;
    this.hasAccountLevelAccess = this.sharedData.sharedData.hasAccountLevelAccess;
    this.hasSharePipAccess = this.sharedData.sharedData.hasSharePipAccess;
    this.hasDummyPipAccess = this.sharedData.sharedData.hasDummyPipAccess;
    this.hasAccountLevelEditorAccess = this.sharedData.sharedData.hasAccountLevelEditorAccess;

    if (this.roleAndAccount != null) {
      this.checkRole = this.getUserSpecificRoles(this.roleAndAccount);
    }

    this.notificationService.isFormDirty.subscribe(isDirty => {
      this.isDirty = isDirty;
    });
  }

  isThisTabActive(routerLink: string): boolean {
    return this.router.url === routerLink;
  }

  onNavigate() {
    if (this.isDirty) {
      this.notificationService.showDialog();
    }
  }

  getUserSpecificRoles(roleAndAccount: IRoleAndAccount[]): ICheckRole {
    const checkRole: ICheckRole = {
      isAdmin: false, isEditor: false, isFinanaceApprover: false,
      isReviewer: false, isReadOnly: false
    };

    return this.sharedData.getUserSpecificRoles(roleAndAccount);

    // roleAndAccount.forEach(roleAcc => {

    //   if (roleAcc.roleName === 'Admin') {
    //     checkRole.isAdmin = true;
    //   }
    //   else if (roleAcc.roleName === 'Editor') {
    //     checkRole.isEditor = true;
    //   }
    //   else if (roleAcc.roleName === 'Finance Approver') {
    //     checkRole.isFinanaceApprover = true;
    //   }
    //   else if (roleAcc.roleName === 'Reviewer') {
    //     checkRole.isReviewer = true;
    //   }
    //   else if (roleAcc.roleName === 'Readonly') {
    //     checkRole.isReadOnly = true;
    //   }
    // });

    // return checkRole;
  }

  onExpandCollapse() {
    this.collapsed = !this.collapsed;
    this.isCollapsed.emit(this.collapsed);
  }
}


