import { SharedDataService } from '@global';
import { IRoleAndAccount } from './../../shared-module/domain/IRoleAndAccount';
import { ICheckRole } from './../../shared-module/domain/ICheckRole';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-administration',
  templateUrl: './administration.component.html'
})
export class AdministrationComponent implements OnInit {

  checkRole: ICheckRole;
  roleAndAccount: IRoleAndAccount[];

  constructor(private sharedData: SharedDataService) { }

  ngOnInit() {
    this.roleAndAccount = this.sharedData.sharedData.userRoleAccountDTO.roleAndAccountDTO;
    if (this.roleAndAccount != null) {
      this.checkRole = this.getUserSpecificRoles(this.roleAndAccount);
    }
  }
  getUserSpecificRoles(roleAndAccount: IRoleAndAccount[]): ICheckRole {
    const checkRole: ICheckRole = {
      isAdmin: false, isEditor: false, isFinanaceApprover: false,
      isReviewer: false, isReadOnly: false
    };

    roleAndAccount.forEach(roleAcc => {

      if (roleAcc.roleName === 'Admin') {
        checkRole.isAdmin = true;
      }
      else if (roleAcc.roleName === 'Editor') {
        checkRole.isEditor = true;
      }
      else if (roleAcc.roleName === 'Finance Approver') {
        checkRole.isFinanaceApprover = true;
      }
      else if (roleAcc.roleName === 'Reviewer') {
        checkRole.isReviewer = true;
      }
      else if (roleAcc.roleName === 'Readonly') {
        checkRole.isReadOnly = true;
      }
    });

    return checkRole;
  }
}
