import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IUsers, IUserListResult } from '@shared';
import { AddNewUserService } from '@shared/services/add-new-user.service';
import { MessageService, MenuItem } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { Constants } from '@shared';
import { NavigationEnd, Router } from '@angular/router';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-add-new-user',
  templateUrl: './add-new-user.component.html',
})
export class AddNewUserComponent implements OnInit {
  @ViewChild('fileInput') fileInput;
  addUserFrom: FormGroup;
  userList: IUsers[];
  userListResult: IUserListResult[];
  errorMessege: string;
  emailError: string;
  tabMenuItems: MenuItem[];
  activeTabMenuItem: MenuItem;
  activeTabIndex = 0;
  fileToUpload: File = null;
  uploadedFiles: any[] = [];
  contents: any = null;
  filename: string;
  file: any;
  returnData: any;
  isDataAvailable: boolean;
  fileModel: any = {
    fileContent: ''
  };
  totalRecordsProcessed = 0;
  totalRecordsFailed = 0;
  totalRecordsPassed = 0;
  showSearch = false;

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private messageService: MessageService,
    private translateService: TranslateService,
    private addNewUserService: AddNewUserService,
  ) { }

  ngOnInit() {
    this.addNewUserService.getUsers().subscribe(users => {
      this.userList = users;
    });
    this.initializeFrom();
    this.tabMenuItems = [
      {
        label: Constants.userTabMenuItems.addUser,
        command: this.navigateTo(Constants.uiRoutes.addNewUser)
      },
      {
        label: Constants.userTabMenuItems.userList,
        command: this.navigateTo(Constants.uiRoutes.listUsers)
      }
    ];
    if (event instanceof NavigationEnd) {
      this.setActiveTabMenuItem();
    }
    this.setActiveTabMenuItem();

  }

  navigateTo(path): () => void {
    return (): void => {
      switch (path) {
        case Constants.uiRoutes.addNewUser:
          this.activeTabIndex = 0;
          break;
        case Constants.uiRoutes.listUsers:
          this.activeTabIndex = 1;
          break;
      }
      this.router.navigate([Constants.uiRoutes.administration, path]);
    };
  }
  onSearchFilters() {
    this.showSearch = !this.showSearch;
  }

  setActiveTabMenuItem(): void {
    if (this.router.url.match('addNewUser')) {
      this.activeTabMenuItem = this.tabMenuItems[0];
      this.activeTabIndex = 0;
    } else if (this.router.url.match('listUsers')) {
      this.activeTabMenuItem = this.tabMenuItems[1];
      this.activeTabIndex = 1;
    }
  }

  initializeFrom() {
    this.addUserFrom = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      uid: ['', [Validators.required, Validators.maxLength(10)]],
      email: ['', [Validators.required, Validators.pattern(Constants.regExType.email)]],
    });
  }

  onAddUserClick() {
    const user: IUsers = {
      userId: 0,
      uid: this.addUserFrom.controls.uid.value.toLocaleUpperCase(),
      email: this.addUserFrom.controls.email.value,
      firstName: this.addUserFrom.controls.firstName.value,
      lastName: this.addUserFrom.controls.lastName.value,
      isActive: true,
      roleId: null,
      accountId: null,
    };
    this.addNewUserService.saveUserData(user).subscribe(data => {
      if (isNullOrUndefined(data)) {
          this.translateService.get('SuccessMessage.AddNewUser').subscribe(msg => {
          this.messageService.add({ severity: 'success', detail: msg });
        });
      }
      else {
        this.messageService.add({ severity: 'error', detail: 'User with Email : ' + data + ' already exists.' });
      }
    });
  }

  onCancel() {
    this.emailError = '';
    this.addUserFrom.reset();
  }

  validateUId() {
    const user = this.userList.find(uId => uId.uid === this.addUserFrom.controls.uid.value.toLocaleUpperCase());
    if (user) {                                                       // user already there in User table
      this.addUserFrom.controls.uid.setErrors({ 'invalid': true });
      this.errorMessege = 'User with given UID already exists!';
    }
    else {
      this.errorMessege = '';
    }
  }

  validateEmail() {
    if (this.addUserFrom.controls.email.invalid) {
      this.emailError = 'Invalid Email Format!';
    }
    else {
      this.emailError = '';
    }
    if (this.addUserFrom.controls.email.value === '') {
      this.emailError = '';
    }
  }

  onTemplateUpload() {
    const files = this.fileInput.files;
    this.fileModel.fileContent = files;
    const formData = new FormData();

    for (const file of files) {
      if (file.name.startsWith('AddMultipleUser')) {
          formData.append(file.name, file);
      } else {
        this.messageService.add({ severity: 'error', detail: 'Invalid File. Expected file is : AddMultipleUser.xlsx' });
        return;
      }

    }
    if (this.fileModel.fileContent.length !== 0) {
      this.addNewUserService.UploadMultipleUserData(formData).subscribe(result => {
        this.userListResult = null;
        this.totalRecordsProcessed = 0;
        this.totalRecordsFailed = 0;
        this.totalRecordsPassed = 0;
        if (result[0].status === 'Failed' && result[0].message === 'No Content') {
          this.messageService.add({ severity: 'error', detail: 'No Data in File to upload' });
        }
        else {
          this.userListResult = result;
          this.getUploadedRecordsSummary();
          this.translateService.get('SuccessMessage.AddMultipleUser').subscribe(msg => {
            this.messageService.add({ severity: 'success', detail: msg });
          });
        }
      });
    }
  }

  getUploadedRecordsSummary() {
    if (!isNullOrUndefined(this.userListResult)) {
      this.userListResult.forEach(userList => {
        ++this.totalRecordsProcessed;
        if (userList.status === 'Failed') {
          ++this.totalRecordsFailed;
        }
        else {
          ++this.totalRecordsPassed;
        }
      });
    }
  }

  downloadAddUserTemplate(): boolean {
    const link = document.createElement('a');
    link.setAttribute('target', '_blank');
    link.setAttribute('href', 'assets/AddMultipleUser.xlsx');
    link.setAttribute('download', `AddMultipleUser.xlsx`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    return false;
  }
}
