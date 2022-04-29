import { Mastermapper } from './../../../shared-module/mapper/master/mastermapper';
import { SelectItem, MessageService } from 'primeng/api';
import { ISharePipVersion } from './../../../shared-module/domain/ISharePipVersion';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedDataService } from '@global';
import { SharePipService } from './../../../shared-module/services/share-pip.service';
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { ISharePIP } from '@shared/domain/ISharePIP';
import { Location } from '@angular/common';

import { Constants } from '@shared';
@Component({
  selector: 'app-share-pip',
  templateUrl: './share-pip.component.html'
})
export class SharePipComponent implements OnInit {
  sharePIP: FormGroup;
  SharePIPCols: any[] = [];
  SharePIP: any[] = [];
  sharePipArray: ISharePIP[];
  projectId: number;
  sharePipVersion: ISharePipVersion;
  isDataAvailable = false;
  sharePipVersionForm: FormGroup;
  version: string;
  users: SelectItem[];
  isDummy = false;
  isSharePipList = false;
  isValid = false;

  constructor(
    private fb: FormBuilder,
    private translateService: TranslateService,
    private _location: Location,
    private router: Router,
    private sharePipService: SharePipService,
    private route: ActivatedRoute,
    private messageService: MessageService,
  ) { }

  ngOnInit() {
    this.isDummy = this.router.url.includes('samples') ? true : false;
    this.sharePipVersionForm = this.fb.group({
      sfProjectId: [''],
      user: ['', [Validators.required]],
      selectedUser: this.fb.array([]),
      versionList: this.fb.array([]),
      comments: ['', [Validators.required]]
    });

    this.route.paramMap.subscribe(
      params => {
        this.projectId = parseInt(params.get(Constants.uiRoutes.routeParams.projectId), 10);
      });
    this.translateService.get('SharePIP').subscribe(fields => {
      this.SharePIP = fields;
    });
    this.translateService.get('SharePIP.SharePIPColumns').subscribe(cols => {
      this.SharePIPCols = cols;
    });
    this.translateService.get('VERSION').subscribe(version => {
      this.version = version;
    });

    this.sharePipService.getSharePipVersionData(this.projectId).subscribe(sharePipVersionData => {
      this.sharePipVersion = sharePipVersionData;
      this.users = new Mastermapper().getUserComboItems(sharePipVersionData.userDTO);
      this.bindControls(sharePipVersionData);
      this.isDataAvailable = true;
    });
  }

  backClicked() {
    this._location.back();
  }

  bindControls(sharePipVersionData: ISharePipVersion) {
    this.sharePipVersionForm.get('sfProjectId').setValue(sharePipVersionData.projectDTO.sfProjectId);

    if (sharePipVersionData.pipSheetDTO && sharePipVersionData.pipSheetDTO.length > 0) {
      const versionListArray = this.sharePipVersionForm.get('versionList') as FormArray;

      for (const pipSheetVersionData of sharePipVersionData.pipSheetDTO) {
        versionListArray.push(this.fb.group({
          projectId: pipSheetVersionData.projectId,
          pipSheetId: pipSheetVersionData.pipSheetId,
          versionName: [this.version + ' ' + pipSheetVersionData.versionNumber, []],
          versionNumber: pipSheetVersionData.versionNumber,
          editor: [false],
          readOnly: [false],
        }));
      }
    }
  }

  onSwitchChange(rowIndex: number, role: string): void {
    const versionList = this.sharePipVersionForm.controls.versionList as FormArray;
    const singleVersionControls = versionList.controls[rowIndex];
    if (role === 'editor') {
      singleVersionControls.get('readOnly').setValue(false);
    }
    else if (role === 'readOnly') {
      singleVersionControls.get('editor').setValue(false);
    }
    if (versionList.value.find(vL => vL.editor === true || vL.readOnly === true)) {
      this.isValid = true;
    }
    else {
      this.isValid = false;
    }
  }

  onSelectedUser(item: any) {
    const userArray: FormArray = new FormArray([]);
    item.forEach(element => {
      userArray.push(new FormGroup({
        userId: new FormControl(element.userId),
        userName: new FormControl(element.userName)
      }));
    });
    this.sharePipVersionForm.setControl('selectedUser', userArray);
  }

  onShareClick() {
    const sharePIPArray: ISharePIP[] = [];

    if (this.sharePipVersionForm.value.selectedUser.length > 0) {
      this.sharePipVersionForm.value.versionList.forEach(singleVersion => {
        this.sharePipVersionForm.value.selectedUser.forEach(singleUser => {
          if (singleVersion.editor === true || singleVersion.readOnly === true) {
            sharePIPArray.push({
              versions: 0,
              readonly: '',
              editor: '',
              accountId: this.sharePipVersion.projectDTO.accountId,
              roleId: (singleVersion.editor) ? 3 : (singleVersion.readOnly) ? 5 : 0,
              pipSheetId: singleVersion.pipSheetId,
              projectId: singleVersion.projectId,
              versionNumber: singleVersion.versionNumber,
              sharedWithUserId: singleUser.userId,
              sharedWithUserName: '',
              shareComments: this.sharePipVersionForm.value.comments,
              sharedWithUId: '',
              isEditClicked: false,
              versionName: null
            });
          }
        });
      });
    }
    if (sharePIPArray.length > 0) {
      this.sharePipService.saveSharedPipData(sharePIPArray).subscribe(sharePipVersionData => {
        if (sharePipVersionData) {
          this.translateService.get('ErrorMessage.SharePipErrorMsg').subscribe(msg => {
            this.messageService.add({ severity: 'error', detail: msg });
          });
          this.resetFormControls();
        }
        else {
          this.translateService.get('SuccessMessage.SharePipSuccess').subscribe(msg => {
            this.messageService.add({ severity: 'success', detail: msg });
          });
          this.resetFormControls();
        }
      });
    }
    else {
      this.translateService.get('ErrorMessage.SelectRoleForAtLeastOneRoleAndUser').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg, life: 5000 });
      });
    }
  }

  resetFormControls() {
    const versionList = this.sharePipVersionForm.controls.versionList as FormArray;
    const singleVersionControls = versionList.controls;
    for (const element of singleVersionControls) {
      element.get('editor').setValue(false);
      element.get('readOnly').setValue(false);
    }
    this.sharePipVersionForm.controls.user.reset();
    this.sharePipVersionForm.controls.comments.reset();
    this.sharePipVersionForm.controls.selectedUser = new FormArray([]);

  }

  navigateToSharePip(event: any) {
    if (event.index === 0) {
      this.isSharePipList = false;
    } else {
      this.isSharePipList = true;
    }
  }
}

