import { Component, OnInit } from '@angular/core';
import { UserRoleService } from '@shared/services/user-role.service';
import { IUserRole } from '@shared/domain/IUserRole';
import { IRole } from '@shared/domain/IRole';
import { SharedDataService, IUser } from '@global';
import { SelectItem, MessageService, ConfirmationService } from 'primeng/api';
import { Mastermapper } from '@shared';
import { TranslateService } from '@ngx-translate/core';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-userroles',
  templateUrl: './userroles.component.html'
})
export class UserRolesComponent implements OnInit {

  userRoles: IUserRole[];
  selectedAccountUserRoles: IUserRole[];
  masterRoles: IRole[];
  accounts: SelectItem[];
  cols: any = [];
  selectedAccountItem: any;
  checked = true;
  users: SelectItem[];
  selectedUsers: IUserRole[] = [];
  selectedUser: any;
  isDataSaved: boolean;
  showDeleteDialog = false;
  showSaveDialog = false;
  userId: number;
  userToDelete: IUserRole;
  accountId: -1;
  headerMessage: string;
  isEditorCheckedYes: boolean;
  isEditorCheckedNo: boolean;

  constructor(private userRoleService: UserRoleService,
    private sharedDataService: SharedDataService,
    private translateService: TranslateService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.Account').subscribe((data) => {
      this.cols = data;
    });
    this.isDataSaved = true;
    this.masterRoles = this.sharedDataService.sharedData.roleDTO;
    this.getAccountDropdown();
    this.getUserRoles();
  }

  getAccountDropdown() {
    let isAdmin = false;
    this.sharedDataService.sharedData.userRoleAccountDTO.roleAndAccountDTO.forEach(admin => {
      if (admin.roleId === 1) {
        isAdmin = true;
      }
    });
    if (isAdmin) {
      this.accounts = new Mastermapper().getAccountComboItems(this.sharedDataService.sharedData.accountDTO, false);
    }
    else {
      this.accounts = new Mastermapper().getAccountNameEntityComboItems(this.sharedDataService.sharedData.accountDTO,
        this.sharedDataService.sharedData.userRoleAccountDTO.roleAndAccountDTO);
    }
    this.accountId = this.accounts[0].value.id;
  }

  getUserRoles() {
    this.userRoleService.getUsersRoles(this.accountId).subscribe(userRoles => {
      this.userRoles = userRoles;
      this.selectedAccountUserRoles = userRoles.filter(acc => acc.accountId === this.accountId);
      this.users = new Mastermapper().getNewUserUIDsComboItems(this.getNewUsersId());
      this.initialiseAddUserGrid();
    });
  }

  onAccountChange(event) {
    this.accountId = event.value.id;
    this.getUserRoles();
  }

  onSave(rowData, isAdd: boolean) {
    rowData.accountId = this.accountId;
    this.userRoleService.saveUserRoles(rowData).subscribe(success => {
      this.translateService.get('SuccessMessage.UpdateUser').subscribe(msg => {
        rowData.isDisabled = false;
        if (isAdd) {
          msg = 'User Added Successfully!';
          this.assignUserToAccount(rowData);
        } else {
          if (!rowData.isEditor && !rowData.isReviewer && !rowData.isReadOnly) {
            this.users = new Mastermapper().getNewUserUIDsComboItems(this.getNewUsersId());
            msg = 'User Removed successfully!';
          }
        }
        rowData.isDataToBeSaved = false;
        this.messageService.add({ severity: 'success', detail: msg });
      });
    }, () => {
      this.translateService.get('ErrorMessage.UpdateUser').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
    });
    this.isEditorCheckedYes = null;
    this.isEditorCheckedNo = null;
  }

  getNewUsersId(): IUserRole[] {
    return this.userRoles.filter(user => (!user.isEditor && !user.isReviewer && !user.isReadOnly));
  }

  // add new user

  onUIDChange(item) {
    const userSelected = this.userRoles.find(user => user.uid === item.id);
    this.selectedUsers.shift();

    if (!isNullOrUndefined(userSelected)) {

      //  this object is  push as an independent object to act like a clone of found user.
      this.selectedUsers.push({
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
        account: userSelected.account,
        roles: '',
        roleId: userSelected.roleId,
        uid: userSelected.uid,
        userId: userSelected.userId,
        isDataToBeSaved: userSelected.isDataToBeSaved
      });

      this.selectedUser = item;
    } else {
      this.initialiseAddUserGrid();
    }
  }

  initialiseAddUserGrid() {
    this.selectedUsers = [];
    this.selectedUser = null;
    this.selectedUsers.push({
      accountId: this.accountId,
      email: '',
      firstName: '',
      isDisabled: false,
      isEditor: false,
      isReadOnly: false,
      isReviewer: false,
      isAllAccountReadOnly: false,
      isAdmin: false,
      isFinanceApprover: false,
      lastName: '',
      roleId: -1,
      uid: '',
      userId: -1,
      isDataToBeSaved: false,
      name: '',
      roles: '',
      account: ''
    });

  }

  assignUserToAccount(rowData: IUserRole) {
    //  find the user and set the roles as assigned by the user
    const user = this.userRoles.find(u => u.userId === rowData.userId);
    user.isEditor = rowData.isEditor;
    user.isReviewer = rowData.isReviewer;
    user.isReadOnly = rowData.isReadOnly;

    // bring that item to top
    const index = this.userRoles.findIndex(u => u.userId === rowData.userId);
    this.userRoles[index].accountId = rowData.accountId;
    this.userRoles.unshift(this.userRoles.splice(index, 1)[0]);
    this.selectedAccountUserRoles = this.userRoles.filter(acc => acc.accountId === this.accountId);
    // reset everything else
    this.selectedUsers = [];
    this.users = new Mastermapper().getNewUserUIDsComboItems(this.getNewUsersId());
    this.initialiseAddUserGrid();
  }

  doesUsersExist(): boolean {
    return this.userRoles.filter(user => (user.isEditor === true || user.isReadOnly === true ||
      user.isReviewer === true) || user.isDataToBeSaved === true).length > 0;
  }


  onSaveRequest(rowData) {
    if (!rowData.isEditor && !isNullOrUndefined(this.isEditorCheckedNo)) {
      if (!this.isEditorCheckedNo && isNullOrUndefined(this.isEditorCheckedYes)) {
        this.headerMessage = this.translateService.instant('Admin.DeleteEditRoleDialogHeader');
        this.confirmationService.confirm({
          key: 'isSaveRole',
          header: this.headerMessage,
          accept: () => {
            this.showSaveDialog = false;
            this.onSave(rowData, false);
          },
          reject: () => {
            this.showSaveDialog = false;
          }
        });
      }
      else {
        this.onSave(rowData, false);
      }
    }
    else {
      this.onSave(rowData, false);
    }
  }

  onDeleteRequest(rowData) {
    if (rowData.isEditor) {
      this.headerMessage = this.translateService.instant('Admin.DeleteEditRoleDialogHeader');
    }
    else {
      this.headerMessage = this.translateService.instant('Admin.DeleteDialogHeader');
    }
    this.confirmationService.confirm({
      key: 'isDeleteRole',
      header: this.headerMessage,
      accept: () => {
        this.showDeleteDialog = false;
        this.userToDelete = rowData;
        this.deleteUser();
      },
      reject: () => {
        this.showDeleteDialog = false;
      }
    });
  }

  onEditorValueChange(event) {
    const isChecked = event.checked;
    if (isChecked) {
      this.isEditorCheckedYes = isChecked;
      this.isEditorCheckedNo = null;
    }
    else {
      this.isEditorCheckedNo = isChecked;
    }
  }

  deleteUser() {
    this.userToDelete.isEditor = false;
    this.userToDelete.isReadOnly = false;
    this.userToDelete.isReviewer = false;

    this.onSave(this.userToDelete, false);
  }
}
