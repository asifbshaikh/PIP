import { Component, OnInit } from '@angular/core';
import { UserRoleService } from '@shared/services/user-role.service';
import { IUserRole } from '@shared/domain/IUserRole';
import { SharedDataService } from '@global';
import { SelectItem } from 'primeng/api';
import { Mastermapper } from '@shared';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'account-roles',
  templateUrl: './account-roles.component.html'
})
export class AccountRolesComponent implements OnInit {
  userRoles: IUserRole[];
  accounts: SelectItem[];



  constructor(private userRoleService: UserRoleService,
    private sharedDataService: SharedDataService) { }

  ngOnInit() {
    this.userRoleService.getAllUsersAndAssociatedRoles().subscribe(data => {
      this.userRoles = data;
      this.composeData();
      this.extractAllAccountUsers();

    });

    // binding accounts

    this.accounts = new Mastermapper().getAccountComboItemsForUserList(this.sharedDataService.sharedData.accountDTO);
  }

  getRoles(userRole: IUserRole) {
    let roles = '';

    if (userRole.isAdmin) {
      roles += ' Admin |';
    }

    if (userRole.isEditor) {
      roles += ' Editor |';
    }

    if (userRole.isFinanceApprover) {
      roles += ' Finance Approver |';
    }

    if (userRole.isReviewer) {
      roles += ' Reviewer |';
    }

    if (userRole.isReadOnly) {
      roles += ' Read Only |';
    }

    return roles.substring(0, roles.length - 1);

  }

  getAccount(accountID) {
    return this.accounts.find(acc => acc.value.id === accountID).label;
  }

  extractAllAccountUsers() {
    const allAccountsUser = this.userRoles.filter((user, index, users) =>
      users.findIndex(u => u.isAllAccountReadOnly === true && u.userId === user.userId) === index);

    if (allAccountsUser.length > 0) {
      allAccountsUser.forEach(userSelected => {
        this.userRoles.push({
          accountId: userSelected.accountId,
          email: userSelected.email,
          firstName: userSelected.firstName,
          isDisabled: userSelected.isDisabled,
          isEditor: userSelected.isEditor,
          isReadOnly: userSelected.isReadOnly,
          isReviewer: userSelected.isReviewer,
          isAdmin: userSelected.isAdmin,
          isFinanceApprover: userSelected.isFinanceApprover,
          isAllAccountReadOnly: userSelected.isAllAccountReadOnly,
          lastName: userSelected.lastName,
          name: userSelected.firstName + ' ' + userSelected.lastName,
          account: 'All Accounts',
          roles: 'Read Only',
          roleId: userSelected.roleId,
          uid: userSelected.uid,
          userId: userSelected.userId,
          isDataToBeSaved: userSelected.isDataToBeSaved
        });

      });
    }
  }

  composeData() {
    if (!isNullOrUndefined(this.userRoles)) {

      this.userRoles.forEach(user => {
        user.name = user.firstName + ' ' + user.lastName;
        user.roles = this.getRoles(user);
        user.account = this.getAccount(user.accountId);
      });
    }
  }



}
