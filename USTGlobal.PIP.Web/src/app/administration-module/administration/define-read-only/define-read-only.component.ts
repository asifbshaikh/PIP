import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IUsers } from '@shared/domain/IUsers';
import { TranslateService } from '@ngx-translate/core';
import { MessageService } from 'primeng/api';
import { DefineReadOnlyService } from '@shared/services/define-read-only.service';
import { IAdminList } from '@shared/domain/IAdminList';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-define-read-only',
  templateUrl: './define-read-only.component.html',
})
export class DefineReadOnlyComponent implements OnInit {
  assignRoleForm: FormGroup;
  cols: any = [];
  display = false;
  readOnlyUserList: IAdminList[];
  accountId: number;
  userList: IUsers[];
  user: IUsers;
  errorMessege: string;
  isValid = false;
  displayDelete = false;
  userId: number;
  showSearch = false;

  @ViewChild('dt')
  private table: Table;
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private defineReadOnlyService: DefineReadOnlyService,
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
    this.defineReadOnlyService.getReadOnlyData().subscribe(readOnlyUserList => {
      this.readOnlyUserList = readOnlyUserList;
    });
  }

  onDeleteClick(userId) {
    this.displayDelete = true;
    this.userId = userId;
  }

  deleteRole() {
    this.defineReadOnlyService.deleteReadOnlyRole(this.userId).subscribe(role => {
      this.translateService.get('SuccessMessage.DeleteRole').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
        this.getAdminData();
      });
    });
  }
  onAssignRoleClick() {
    this.display = true;
    this.defineReadOnlyService.getUsers().subscribe(users => {
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
    this.defineReadOnlyService.assignAdminRole(user.userId).subscribe(data => {
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
    const adminUser = this.readOnlyUserList.find(uId => uId.uid === this.assignRoleForm.controls.uid.value.toLocaleUpperCase());
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
  onSearchFilters() {
    this.showSearch = !this.showSearch;
    this.table.reset();
  }
}
