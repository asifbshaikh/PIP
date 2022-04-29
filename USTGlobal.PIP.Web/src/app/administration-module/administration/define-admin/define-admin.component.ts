import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';
import { DefineAdminService } from '@shared/services/define-admin.service';
import { IAdminList } from '@shared/domain/IAdminList';
import { MessageService } from 'primeng/api';
import { IUsers } from '@shared/domain/IUsers';

@Component({
  selector: 'app-define-admin',
  templateUrl: './define-admin.component.html',
})
export class DefineAdminComponent implements OnInit {
  assignRoleForm: FormGroup;
  cols: any = [];
  display = false;
  adminList: IAdminList[];
  accountId: number;
  userList: IUsers[];
  user: IUsers;
  errorMessege: string;
  isValid = false;
  displayDelete = false;
  userId: number;

  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private defineAdminService: DefineAdminService,
    private messageService: MessageService,
  ) { }

  ngOnInit() {
    this.translateService.get('Admin.DefineFinancePOC').subscribe((data) => {
      this.cols = data;
    });
    this.initializeFrom();
    this.getAdminData();
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

  getAdminData() {
    this.defineAdminService.getAdminData(this.accountId = 0).subscribe(adminList => {
      this.adminList = adminList;
    });
  }

  onDeleteClick(userId) {
    this.displayDelete = true;
    this.userId = userId;
  }

  deleteRole() {
    this.defineAdminService.deleteUserRole(this.userId, 0).subscribe(role => {
      this.translateService.get('SuccessMessage.DeleteRole').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
        this.getAdminData();
      });
    });
  }
  onAssignRoleClick() {
    this.display = true;
    this.defineAdminService.getUsers().subscribe(users => {
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
      roleId: 1,
      accountId: 0,
      email: '',
      firstName: '',
      lastName: '',
      isActive: true
    };
    (user as any).fromAdminScreen = true;
    this.defineAdminService.assignAdminRole(user).subscribe(data => {
      this.translateService.get('SuccessMessage.AssignRole').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
        this.getAdminData();
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
        this.errorMessege = 'User with given UID does not exists!';
      }
      else if (adminUser) {                                                       // user already there in Admin Role table
        this.assignRoleForm.controls.uid.setErrors({ 'invalid': true });
        this.errorMessege = 'User with given UID already exists!';
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
