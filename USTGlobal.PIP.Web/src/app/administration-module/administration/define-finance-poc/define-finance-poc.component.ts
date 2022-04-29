import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';
import { IAdminList } from '@shared/domain/IAdminList';
import { IUsers } from '@shared/domain/IUsers';
import { MessageService } from 'primeng/api';
import { ComboItems, Mastermapper, Constants } from '@shared';
import { DefineFinancePocService } from '@shared/services/define-finance-poc.service';

@Component({
  selector: 'app-define-finance-poc',
  templateUrl: './define-finance-poc.component.html',
})
export class DefineFinancePocComponent implements OnInit {
  account: ComboItems[];
  assignRoleForm: FormGroup;
  cols: any = [];
  display = false;
  adminList: IAdminList[];
  accountId: number;
  userList: IUsers[];
  user: IUsers;
  errorMessege: string;
  isValid = false;
  isAdmin = false;
  displayDelete = false;
  userId: number;

  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private defineFinancePocService: DefineFinancePocService,
    private messageService: MessageService,
    private sharedData: SharedDataService,
  ) { }

  ngOnInit() {
    this.translateService.get('Admin.DefineFinancePOC').subscribe((data) => {
      this.cols = data;
    });
    this.getAccountDropdown();
    this.initializeFrom();
    this.getFinancePOC();
  }


  initializeFrom() {
    this.assignRoleForm = this.fb.group({
      userId: [],
      name: [],
      roleId: [],
      accountId: [],
      uid: [],
    });
  }

  getAccountDropdown() {
    this.sharedData.sharedData.userRoleAccountDTO.roleAndAccountDTO.forEach(admin => {
      if (admin.roleId === 1) {
        this.isAdmin = true;
      }
    });
    if (this.isAdmin) {
      this.account = new Mastermapper().getAccountComboItems(this.sharedData.sharedData.accountDTO, false);
    }
    else {
      this.account = new Mastermapper().getAccountNameEntityComboItems(this.sharedData.sharedData.accountDTO,
        this.sharedData.sharedData.userRoleAccountDTO.roleAndAccountDTO);
    }
    this.accountId = this.account[0].value.id;        // Default Account
  }

  getFinancePOC() {
    this.defineFinancePocService.getFinancePOC(this.accountId).subscribe(adminList => {
      this.adminList = adminList;
    });
  }

  onAccountChange(event) {
    this.accountId = event.value.id;
    this.getFinancePOC();
  }

  onDeleteClick(userId) {
    this.displayDelete = true;
    this.userId = userId;
  }
  deleteRole() {
    this.defineFinancePocService.deleteUserRole(this.userId, this.accountId).subscribe(role => {
      this.translateService.get('SuccessMessage.DeleteRole').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
        this.getFinancePOC();
      });
    });
  }

  onAssignRoleClick() {
    this.display = true;
    this.defineFinancePocService.getUsers().subscribe(users => {
      this.userList = users;
    });
    this.assignRoleForm.reset();
    this.errorMessege = '';
  }

  onUIDInput() {
    this.user = this.userList.find(uId => uId.uid === this.assignRoleForm.controls.uid.value.toLocaleUpperCase());
    this.assignRoleForm.controls.name.setValue(this.user ? this.user.firstName + ' ' + this.user.lastName : '');
    this.validateUId();
  }

  onAssignClick() {
    const user: IUsers = {
      userId: this.user.userId,
      uid: this.assignRoleForm.value.uid,
      roleId: 2,
      accountId: this.accountId,
      email: '',
      firstName: '',
      lastName: '',
      isActive: true,
    };
    (user as any).fromAdminScreen = false;
    this.defineFinancePocService.assignFinancePOCRole(user).subscribe(data => {
      this.translateService.get('SuccessMessage.AssignRole').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
        this.getFinancePOC();
      });
    });
    this.assignRoleForm.reset();
    this.isValid = false;
  }

  validateUId() {
    this.isValid = false;
    const adminUser = this.adminList.find(uId => uId.uid === this.assignRoleForm.controls.uid.value.toLocaleUpperCase());
    this.user = this.userList.find(uId => uId.uid === this.assignRoleForm.controls.uid.value.toLocaleUpperCase());
    if (this.assignRoleForm.controls.uid.value) {
      if (!this.user) {                                                           // user is not listed in user table
        this.assignRoleForm.controls.uid.setErrors({ 'invalid': true });
        this.errorMessege = 'User does not exists.';
      }
      else if (adminUser) {                                                       // user already there in Admin Role table
        this.assignRoleForm.controls.uid.setErrors({ 'invalid': true });
        this.errorMessege = 'User already exists.';
      }
      else {
        this.assignRoleForm.controls.uid.setErrors(null);
        this.errorMessege = '';
        this.isValid = true;
      }
    }
    else {
      this.assignRoleForm.controls.uid.setErrors(null);
      this.errorMessege = '';
    }
  }

}
